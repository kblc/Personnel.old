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

namespace Personnel.Repository.Model
{
    public partial class RepositoryContext
    {
        /// <summary>
        /// Штатное расписание
        /// </summary>
        public DbSet<Staffing> Staffing { get; set; }
    }

    /// <summary>
    /// Штатное расписание
    /// </summary>
    [Table("stuffing")]
    public partial class Staffing : HistoryAbstractBase<long, Staffing>
    {
        public Staffing()
        {
            Employees = new HashSet<Employee>();
        }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("stuffing_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StaffingId { get; set; }

        /// <summary>
        /// Позиция (номер) для сортировки должностей внутри структурного подразделения
        /// </summary>
        [Column("position"), Required]
        public long Position { get; set; }

        #region Department
        /// <summary>
        /// Идентификатор структурного подразделения
        /// </summary>
        [ForeignKey("Department"), Column("department_id"), Required]
        public long DepartmentId { get; set; }
        /// <summary>
        /// Структурное подразделение
        /// </summary>
        public virtual Department Department { get; set; }
        #endregion
        #region Appoint
        /// <summary>
        /// Идентификатор структурного подразделения
        /// </summary>
        [ForeignKey("Appoint"), Column("appoint_id"), Required]
        public long AppointId { get; set; }
        /// <summary>
        /// Структурное подразделение
        /// </summary>
        public virtual Appoint Appoint { get; set; }
        #endregion
        #region Employees
        /// <summary>
        /// Сотрудники на этой должности
        /// </summary>
        public virtual ICollection<Employee> Employees { get; set; }

        #endregion
    }
}
