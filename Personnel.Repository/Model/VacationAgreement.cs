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
        /// Хранилище согласования отпусков
        /// </summary>
        public DbSet<VacationAgreement> VacationAgreements { get; set; }
    }

    /// <summary>
    /// Согласование отпусков
    /// </summary>
    [Table("vacation_agreement")]
    public class VacationAgreement : HistoryAbstractBase<long, VacationAgreement>
    {
        [Key, Column("vacation_agreement_id"), Required]
        public long VacationAgreementId { get; set; }

        #region Vacation
        /// <summary>
        /// Идентификатор отпуска
        /// </summary>
        [ForeignKey("Vacation"), Column("vocation_id"), Required]
        [Index("UIX_VOCATION_AGREEMENT_EMPLOYEE_VOCATION", IsUnique = true, Order = 1)]
        public long VacationId { get; set; }
        /// <summary>
        /// Запись об отпуске
        /// </summary>
        public virtual Vacation Vacation { get; set; }
        #endregion
        #region Employee
        /// <summary>
        /// Идентификатор сотрудника, которому принадлежат логины
        /// </summary>
        [ForeignKey("Employee"), Column("employee_id"), Required]
        [Index("UIX_VOCATION_AGREEMENT_EMPLOYEE_VOCATION", IsUnique = true, Order = 2)]
        public long EmployeeId { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual Employee Employee { get; set; }
        #endregion

        /// <summary>
        /// Дата согласования
        /// </summary>
        [Column("date"), Required]
        public DateTime Date { get; set; }
    }
}
