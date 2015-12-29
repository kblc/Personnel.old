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
        /// Хранилище типов отпусков
        /// </summary>
        public DbSet<Vacation> Vacations { get; set; }
    }

    /// <summary>
    /// Отпуска
    /// </summary>
    [Table("vacation")]
    public class Vacation : HistoryAbstractBase<long, Vacation>
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("vacation_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VacationId { get; set; }

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
        #region VacationLevel
        /// <summary>
        /// Идентификатор уровня отпуска
        /// </summary>
        [ForeignKey("VacationLevel"), Column("vacation_level_id"), Required]
        public long VacationLevelId { get; set; }
        /// <summary>
        /// Уровень отпуска
        /// </summary>
        public virtual VacationLevel VacationLevel { get; set; }
        #endregion

        /// <summary>
        /// Дата начала отпуска
        /// </summary>
        [Column("begin"), Required]
        public DateTime Begin { get; set; }

        /// <summary>
        /// На сколько дней в отпуске
        /// </summary>
        [Column("day_count_main"), Required]
        public long DayCount { get; set; }

        /// <summary>
        /// Отпуск небыл использован
        /// </summary>
        [Column("not_used"), Required]
        public bool NotUsed { get; set; }
    }
}
