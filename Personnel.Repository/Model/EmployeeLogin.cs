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
        /// Хранилище доменных логинов сотрудников
        /// </summary>
        public DbSet<EmployeeLogin> EmployeeLogins { get; set; }
    }

    [Table("employee_login")]
    public class EmployeeLogin : HistoryAbstractBase<long, EmployeeLogin>
    {
        [Key,Column("employee_login_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeLoginId { get; set; }

        #region Employee
        /// <summary>
        /// Идентификатор сотрудника, которому принадлежат логины
        /// </summary>
        [ForeignKey("Employee"), Column("employee_id"), Required]
        [Index("UIX_EMPLOYEE_LOGIN", IsUnique = true, Order = 1)]
        public long EmployeeId { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual Employee Employee { get; set; }
        #endregion

        /// <summary>
        /// Доменный логин сотрудника вида ДОМЕН\ИМЯ_ПОЛЬЗОВАТЕЛЯ
        /// <example>
        /// DOMAIN\USERNAME or WORKGROUP\User1
        /// </example>
        /// </summary>
        [Column("domain_login")]
        [Index("UIX_EMPLOYEE_LOGIN", IsUnique = true, Order = 2)]
        [Required(ErrorMessageResourceName = "MODEL_EMPLOYEELOGIN_LoginRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MODEL_EMPLOYEELOGIN_LoginMaxLength")]
        public string DomainLogin { get; set; }

        /// <summary>
        /// User domain
        /// </summary>
        [NotMapped]
        public string Domain
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DomainLogin))
                    return string.Empty;

                var index = DomainLogin.IndexOf("\\");
                return (index >= 0) 
                    ? DomainLogin.Remove(index)
                    : string.Empty;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    DomainLogin = Login; 
                    return;
                }
                DomainLogin = value + "\\" + Login;
            }
        }

        /// <summary>
        /// User login
        /// </summary>
        [NotMapped]
        public string Login
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DomainLogin))
                    return string.Empty;

                var index = DomainLogin.IndexOf("\\");

                if (index >= 0)
                {
                    return (index < DomainLogin.Length)
                        ? DomainLogin.Substring(index + 1)
                        : string.Empty;
                }
                else
                    return DomainLogin;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    DomainLogin = Domain.Length > 0 ? Domain + "\\" : string.Empty;
                    return;
                }
                DomainLogin = (Domain.Length > 0 ? Domain + "\\" : string.Empty) + value;
            }
        }
    }
}
