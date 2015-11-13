using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Linq;
using Personnel.Repository.Additional;
using System.ComponentModel;

namespace Personnel.Repository.Model
{
    public partial class RepositoryContext
    {
        /// <summary>
        /// Хранилище прав сотрудника
        /// </summary>
        public DbSet<EmployeeRight> EmployeeRights { get; set; }
    }

    /// <summary>
    /// Право сотрудника
    /// </summary>
    [Table("employee_right")]
    public class EmployeeRight : HistoryAbstractBase<long, EmployeeRight>
    {
        [Key, Column("employee_right_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeRightId { get; set; }

        #region Employee
        /// <summary>
        /// Идентификатор сотрудника, которому принадлежит право
        /// </summary>
        [ForeignKey("Employee"), Column("employee_id", Order = 1), Required]
        [Index("UIX_EMPLOYEE_RIGHT", IsUnique = true, Order = 1)]
        public long EmployeeId { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual Employee Employee { get; set; }
        #endregion
        #region Right
        /// <summary>
        /// Идентификатор права
        /// </summary>
        [ForeignKey("Right"), Column("right_id"), Required]
        [Index("UIX_EMPLOYEE_RIGHT", IsUnique = true, Order = 2)]
        public long RightId { get; set; }
        /// <summary>
        /// Право
        /// </summary>
        public virtual Right Right { get; set; }
        #endregion
    }
}
