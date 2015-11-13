using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
//using Personnel.Repository.Migrations;
using Personnel.Repository.Additional;
using System.Data.Common;

namespace Personnel.Repository.Model
{
    public partial class RepositoryContext : DbContext
    {
        #region Static initialization

        static RepositoryContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RepositoryContext, Migrations.Configuration>());
        }

        #endregion
        #region Get Entity connection strings
        private static string GetEntityConnectionString(string connectionString, string connectionProviderName)
        {
            return new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder()
            {
                Provider = connectionProviderName,
                ProviderConnectionString = connectionProviderName,
            }.ToString();
        }
        private static string GetEntityConnectionString(string connectionStringName)
        {
            return new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder()
            {
                Name = connectionStringName,
            }.ToString();
        }

        private static string DefConnectionString = string.Empty;
        #endregion
        #region Constructors

        public RepositoryContext()
            : base()
        {
            if (!string.IsNullOrEmpty(DefConnectionString))
                Database.Connection.ConnectionString = DefConnectionString;
            InitLogEvent();
        }
        public RepositoryContext(string connectionStringName)
            : base(GetEntityConnectionString(connectionStringName))
        {
            DefConnectionString = Database.Connection.ConnectionString;
            InitLogEvent();
        }
        public RepositoryContext(string connectionString, string connectionProviderName)
            : base(GetEntityConnectionString(connectionString, connectionProviderName))
        {
            DefConnectionString = Database.Connection.ConnectionString;
            InitLogEvent();
        }
        public RepositoryContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            InitLogEvent();
        }

        #endregion

        /// <summary>
        /// Transaction
        /// </summary>
        private DbContextTransaction transaction = null;

        /// <summary>
        /// Begin transaction
        /// </summary>
        /// <param name="commitOnDispose">Auto commit transaction on disposing</param>
        /// <returns>Disposable transaction</returns>
        public IDisposable BeginTransaction(bool commitOnDispose)
        {
            if (transaction != null)
                throw new Exception("Transaction already started. Use CommitTransaction() or RollbackTransaction() before");
            transaction = Database.BeginTransaction();
            return new DisposableHelper(() => { if (commitOnDispose) CommitTransaction(); else RollbackTransaction(); });
        }

        /// <summary>
        /// Commit started transaction
        /// </summary>
        public void CommitTransaction()
        {
            if (transaction != null)
                try
                {
                    transaction.Commit();
                }
                finally
                {
                    transaction.Dispose();
                    transaction = null;
                }
        }

        /// <summary>
        /// Rollback started transaction
        /// </summary>
        public void RollbackTransaction()
        {
            if (transaction != null)
                try
                {
                    transaction.Rollback();
                }
                finally
                {
                    transaction.Dispose();
                    transaction = null;
                }
        }

        #region Disposing

        private bool disposed = false;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposed)
                return;

            if (disposing)
                RollbackTransaction();

            disposed = true;
        }

        #endregion

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Entity<AccountSettings>()
        //    //    .HasKey(e => e.AccountUID);
        //    //modelBuilder.Entity<Account>()
        //    //    .HasOptional(s => s.Settings)
        //    //    .WithRequired(ad => ad.Account);
        //}

        private int SaveWithHistory()
        {
            var stateChanger = new Dictionary<EntityState, HistoryChangeType>()
            {
                { EntityState.Added, HistoryChangeType.Add },
                { EntityState.Deleted, HistoryChangeType.Remove },
                { EntityState.Modified, HistoryChangeType.Change },
            };

            var dt = DateTime.UtcNow;

            var historySource = ChangeTracker.Entries()
                    .Where(p => p.State == EntityState.Added || p.State == EntityState.Modified || p.State == EntityState.Deleted)
                    .Where(p => p.Entity is IHistoryRecord)
                    .Select(p => new { Entry = p.Entity as IHistoryRecord, State = stateChanger[p.State] })
                    .ToArray();

            int preSave = base.SaveChanges();

            var historyEntities = historySource
                    .Where(p => p.Entry.SourceId != null)
                    .GroupBy(p => new { SourceId = p.Entry.SourceId, p.Entry.SourceName, p.State })
                    .Select(p => p.Key)
                    .Select(ent => new Model.History() { ChangeType = ent.State, SourceId = ent.SourceId, Source = ent.SourceName, Date = dt })
                    .ToArray();

            if (historyEntities.Length > 0)
            { 
                this.History.AddRange(historyEntities);
                preSave += base.SaveChanges();
            }
            return preSave;
        }

        public override int SaveChanges()
        {
            return SaveWithHistory();
        }

        public override Task<int> SaveChangesAsync()
        {
            return Task.Factory.StartNew<int>(() =>
            {
                return SaveWithHistory();
            });
        }

        /// <summary>
        /// Initialize log event
        /// </summary>
        private void InitLogEvent()
        {
            Database.Log = (str) => 
            {
                var log = onDatabaseLog;
                if (log != null)
                    log(this, str);
            };
        }

        /// <summary>
        /// Database log event
        /// </summary>
        public event EventHandler<string> onDatabaseLog;
    }
}
