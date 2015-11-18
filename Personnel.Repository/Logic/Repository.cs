using Helpers.Linq;
using Personnel.Repository.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Repository.Logic
{
    public partial class Repository : IDisposable
    {
        #region Context
#if DEBUG
        public RepositoryContext Context { get; private set; }
#else
        internal RepositoryContext Context { get; private set; }
#endif
        #endregion

        public Repository()
        {
            Context = new RepositoryContext();
            InitLogEvent();
        }
        public Repository(string connectionStringName)
        {
            Context = new RepositoryContext(connectionStringName);
            InitLogEvent();
        }
        public Repository(string connectionString, string connectionProviderName)
        {
            Context = new RepositoryContext(connectionString, connectionProviderName);
            InitLogEvent();
        }

        public IDisposable BeginTransaction(bool commitOnDispose = false)
        {
            return Context.BeginTransaction(commitOnDispose);
        }
        public void CommitTransaction()
        {
            Context.CommitTransaction();
        }
        public void RollbackTransaction()
        {
            Context.RollbackTransaction();
        }

        public void SaveChanges(bool waitUntilSaving = true)
        {
            if (Context == null)
                throw new ArgumentNullException("Context", "Initialize Context first for Repository");
            if (waitUntilSaving)
                Context.SaveChanges();
            else
                Context.SaveChangesAsync();
        }

        #region IDisposable

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing && Context != null)
            {
                Context.Dispose();
                Context = null;
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Initialize log event
        /// </summary>
        private void InitLogEvent() => Context.onDatabaseLog += (e, str) => SqlLog?.Invoke(this, str);

        /// <summary>
        /// Raise database log event
        /// </summary>
        /// <param name="logMessage"></param>
        private void RaiseDatabaseLog(string logMessage) => Log?.Invoke(this, logMessage);

        /// <summary>
        /// Database SQL log event
        /// </summary>
        public event EventHandler<string> SqlLog;

        /// <summary>
        /// Database log event
        /// </summary>
        public event EventHandler<string> Log;

        public object GetHistoryItems(Type returnArrayElementType, Type entityType, IEnumerable<string> sourceIds)
        {
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", false, str => str.ToList().ForEach(s => RaiseDatabaseLog(s))))
                try
                {
                    var resType = returnArrayElementType.MakeArrayType();
                    var ids = sourceIds.ToArray();

                    var methods = entityType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                        .Where(mi => mi.ReturnType == resType)
                        .Where(mi => mi.GetCustomAttribute(typeof(HistoryResolverAttribute)) != null)
                        .Select(mi => new { Method = mi, Prms = mi.GetParameters() })
                        .Select(mi => mi.Method)
                        .ToArray()
                        ;

                    foreach (var method in methods)
                        try
                        {
                            return method.Invoke(null, new object[] { this, ids });
                        }
                        catch(Exception ex)
                        {
                            ex.Data.Add(nameof(method), method.Name);
                            logSession.Add(ex);
                            logSession.Enabled = true;
                        }
                }
                catch(Exception ex)
                {
                    ex.Data.Add(nameof(returnArrayElementType), returnArrayElementType);
                    ex.Data.Add(nameof(entityType), entityType);
                    ex.Data.Add(nameof(sourceIds), sourceIds.Concat(i => i.ToString(), ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                }
            return null;
        }

        protected static System.Reflection.PropertyInfo[] GetKeyProperties(Type entityType)
        {
            return entityType.GetProperties()
                    .Where(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true)?.Length > 0)
                    .ToArray();
        }

        private static object GetDefault(Type type) => (type.IsValueType) ? Activator.CreateInstance(type) : null;

        private static System.Reflection.MethodBase GetGenericMethod(Type type, string name, Type[] typeArgs, Type[] argTypes, System.Reflection.BindingFlags flags)
        {
            int typeArity = typeArgs.Length;
            var methods = type.GetMethods()
                .Where(m => m.Name == name)
                .Where(m => m.GetGenericArguments().Length == typeArity)
                .Select(m => m.MakeGenericMethod(typeArgs));
            return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
        }

        public void AddOrUpdate<T>(T entityElement, bool takeChilds = true, string[] ignoreClollectionNames = null, EntityState state = EntityState.Unchanged, T original = null)
            where T : class
        {
            var dbSet = GetDbSet<T>();

            if (original != null)
            {
                var originalEntry = Context.Entry<T>(original);
                if (originalEntry != null)
                    originalEntry.State = EntityState.Detached;
            }
            dbSet.Attach(entityElement);
            var saveFailed = false;
            do
            {
                saveFailed = false;
                try
                {
                    if (state == EntityState.Unchanged)
                    {
                        Context.Entry(entityElement).State = EntityState.Modified;
                        foreach (var p in GetKeyProperties(typeof(T)))
                        {
                            var val = p.GetValue(entityElement);
                            var defValue = GetDefault(p.PropertyType);
                            var comp = val as IComparable;
                            var isEquals = (comp != null)
                                ? comp.CompareTo(defValue) == 0
                                : val == defValue;
                            if (isEquals)
                            {
                                Context.Entry(entityElement).State = EntityState.Added;
                                break;
                            }
                        }
                    }
                    else
                        Context.Entry(entityElement).State = state;
                    SaveChanges();

                    if (takeChilds)
                        entityElement.GetType()
                            .GetProperties()
                            .Where(p => (ignoreClollectionNames == null || !ignoreClollectionNames.Contains(p.Name))
                                && p.PropertyType.IsGenericType
                                && p.PropertyType.GetInterfaces().Any(i => i == typeof(System.Collections.IEnumerable))
                                && p.PropertyType.GetGenericArguments().Count() == 1)
                            .ToList()
                            .ForEach(p =>
                            {
                                var genericType = p.PropertyType.GetGenericArguments().First();
                                var updateMethod = GetGenericMethod(GetType(), nameof(this.AddOrUpdate), new[] { genericType }, new Type[] { genericType, typeof(bool), typeof(string[]) }, BindingFlags.InvokeMethod);
                                if (updateMethod != null)
                                {
                                    var enums = p.GetValue(entityElement) as System.Collections.IEnumerable;
                                    if (enums != null)
                                    { 
                                        var items = enums.Cast<object>().ToArray();
                                        if (items.Length > 0)
                                        {
                                            var itemsArray = Array.CreateInstance(genericType, items.Length);
                                            Array.Copy(items, itemsArray, items.Length);
                                            foreach (var item in itemsArray)
                                                updateMethod.Invoke(this, new[] { item, true, null });
                                        }
                                    }
                                }
                            });

                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }
            } while (saveFailed);
        }
    }
}
