using Personnel.Repository.Additional;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Linq;
using System.ComponentModel;

namespace Personnel.Repository.Model
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HistoryResolverAttribute : Attribute
    {

    }

    /// <summary>
    /// Вид изменения
    /// </summary>
    public enum HistoryChangeType
    {
        [ResourceDescription("MODEL_HISTORYCHANGETYPE_Add")]
        Add,
        [ResourceDescription("MODEL_HISTORYCHANGETYPE_Change")]
        Change,
        [ResourceDescription("MODEL_HISTORYCHANGETYPE_Remove")]
        Remove
    }

    public partial class RepositoryContext
    {
        /// <summary>
        /// Хранилище изменений
        /// </summary>
        public DbSet<History> History { get; set; }
    }

    /// <summary>
    /// История изменений
    /// </summary>
    [Table("history")]
    public partial class History
    {
        /// <summary>
        /// Идентификатор записи истории
        /// </summary>
        [Key, Column("history_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HistoryId { get; set; }

        /// <summary>
        /// Идентификатор записи источника
        /// </summary>
        [Column("source_id"), MaxLength(100), Required]
        public string SourceId { get; set; }

        /// <summary>
        /// Дата добавления записи
        /// </summary>
        [Column("date"), Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// Source type name
        /// </summary>
        [Column("sourcetype"), Required]
        public string Source { get; set; }

        #region Change type

        /// <summary>
        /// Change system type name
        /// </summary>
        [Column("changetype"), Required]
        [Obsolete("Use ChangeType property instead")]
        public string ChangeTypeSystemName { get; set; }

        /// <summary>
        /// Get change type name
        /// </summary>
        [NotMapped]
        public string ChangeTypeName => ChangeType.GetResourceDescription();

        /// <summary>
        /// Change type
        /// </summary>
        [NotMapped]
        public HistoryChangeType ChangeType
        {
#pragma warning disable 618
            get { return Extensions.GetEnumValueByName<HistoryChangeType>(ChangeTypeSystemName); }
            set { ChangeTypeSystemName = value.ToString().ToUpper(); }
#pragma warning restore 618
        }

        #endregion
    }
}
