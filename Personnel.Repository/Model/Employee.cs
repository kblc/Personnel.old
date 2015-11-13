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
        public DbSet<Employee> Employees { get; set; }
    }

    /// <summary>
    /// Сотрудник
    /// </summary>
    [Table("employee")]
    public partial class Employee : HistoryAbstractBase<long, Employee>
    {
        public Employee()
        {
            Logins = new HashSet<EmployeeLogin>();
            Photos = new HashSet<EmployeePhoto>();
            Rights = new HashSet<EmployeeRight>();
        }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("employee_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeId { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Column("surname"), Required(ErrorMessageResourceName = "MODEL_EMPLOYEE_SurnameRequired")]
        [MaxLength(50, ErrorMessageResourceName = "MODEL_EMPLOYEE_SurnameMaxLength")]
        public string Surname { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Column("name"), Required(ErrorMessageResourceName = "MODEL_EMPLOYEE_NameRequired")]
        [MaxLength(50, ErrorMessageResourceName = "MODEL_EMPLOYEE_NameMaxLength")]
        public string Name { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [Column("patronymic")]
        [MaxLength(50, ErrorMessageResourceName = "MODEL_EMPLOYEE_PatronymicMaxLength")]
        public string Patronymic { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        [Column("birthday")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Email сотрудника
        /// </summary>
        [Column("email"), MaxLength(200, ErrorMessageResourceName = "MODEL_EMPLOYEE_EmailMaxLength")]
        public string Email { get; set; }

        /// <summary>
        /// Телефон сотрудника
        /// </summary>
        [Column("phone"), MaxLength(200, ErrorMessageResourceName = "MODEL_EMPLOYEE_PhoneMaxLength")]
        public string Phone { get; set; }

        #region Stuffing

        /// <summary>
        /// Идентификатор записи позиции в штатном расписании
        /// </summary>
        [Column("stuffing_id"), ForeignKey("Stuffing")]
        public long? StuffingId { get; set; }

        /// <summary>
        /// Позиции в штатном расписании
        /// </summary>
        public virtual Staffing Stuffing { get; set; }

        #endregion
        #region Списки

        /// <summary>
        /// Логины для входа в систему
        /// </summary>
        public virtual ICollection<EmployeeLogin> Logins { get; set; }
        
        /// <summary>
        /// Фотографии пользователя
        /// </summary>
        public virtual ICollection<EmployeePhoto> Photos { get; set; }

        /// <summary>
        /// Права пользователя
        /// </summary>
        public virtual ICollection<EmployeeRight> Rights { get; set; }

        #endregion
    }
}
