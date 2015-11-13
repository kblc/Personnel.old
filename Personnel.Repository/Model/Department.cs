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
        /// Хранилище сотрудников
        /// </summary>
        public DbSet<Department> Departments { get; set; }
    }

    /// <summary>
    /// Сотрудник
    /// </summary>
    [Table("department")]
    public partial class Department : HistoryAbstractBase<long, Department>
    {
        /// <summary>
        /// Create instance
        /// </summary>
        public Department()
        {
            Childs = new List<Department>();
        }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("department_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DepartmentId { get; set; }

        /// <summary>
        /// Идентификатор родительской записи
        /// </summary>
        [Column("parent_department_id")]
        public long? ParentDepartmentId { get; set; }

        #region Childs

        /// <summary>
        /// Подчиненная запись
        /// </summary>
        [ForeignKey("ParentDepartmentId")]
        public virtual ICollection<Department> Childs { get; set; }

        #endregion
        #region Parent
        /// <summary>
        /// Родительская запись
        /// </summary>
        [ForeignKey("ParentDepartmentId")]
        public virtual Department Parent { get; set; }

        #endregion

        /// <summary>
        /// Название отдела
        /// </summary>
        [Column("name"), Required(ErrorMessageResourceName = "MODEL_DEPARTMENT_NameRequired")]
        [MaxLength(200, ErrorMessageResourceName = "MODEL_DEPARTMENT_NameMaxLength")]
        public string Name { get; set; }
    }
}
