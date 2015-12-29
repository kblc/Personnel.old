using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Repository.Model
{
    public partial class RepositoryContext
    {
        /// <summary>
        /// Хранилище остатков отпусков
        /// </summary>
        public DbSet<VacationBalance> VacationBalances { get; set; }
    }

    /// <summary>
    /// Остатки отпусков
    /// </summary>
    [Table("vacation_balance")]
    public class VacationBalance : HistoryAbstractBase<long, VacationBalance>
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("vacation_balance_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VacationBalanceId { get; set; }

        #region Employee
        /// <summary>
        /// Идентификатор сотрудника, которому принадлежат логины
        /// </summary>
        [ForeignKey("Employee"), Column("employee_id"), Required]
        [Index("UIX_EMPLOYEE_VACATION", IsUnique = false, Order = 1)]
        public long EmployeeId { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual Employee Employee { get; set; }
        #endregion
        
        /// <summary>
        /// Количество дней основного отпуска
        /// </summary>
        [Column("day_count_main"), Required]
        public long DayCountMain { get; set; }

        /// <summary>
        /// Количество дней доп. отпуска
        /// </summary>
        [Column("day_count_additional"), Required]
        public long DayCountAdditional { get; set; }

        /// <summary>
        /// Дата обновления записи
        /// </summary>
        [Column("record_updated"), Required]
        public DateTime Updated { get; set; }
    }
}
