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
        /// Хранилище фотографий сотрудников
        /// </summary>
        public DbSet<EmployeePhoto> EmployeePhotos { get; set; }
    }

    /// <summary>
    /// Фотографии сорудника
    /// </summary>
    [Table("employee_photo")]
    public class EmployeePhoto : HistoryAbstractBase<Guid, EmployeePhoto>
    {
        #region File
        /// <summary>
        /// Идентификатор файла-фотографии
        /// </summary>
        [Key, ForeignKey("Picture"), Column("file_uid"), Required]
        public Guid FileId { get; set; }
        /// <summary>
        /// Фотография
        /// </summary>
        public virtual Picture Picture { get; set; }
        #endregion        
        #region Employee
        /// <summary>
        /// Идентификатор сотрудника, которому принадлежат фото
        /// </summary>
        [ForeignKey("Employee"), Column("employee_id"), Required]
        public long EmployeeId { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual Employee Employee { get; set; }
        #endregion
    }
}
