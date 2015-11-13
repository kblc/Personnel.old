using Personnel.Repository.Additional;
using Personnel.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Repository.Logic
{
    public partial class Repository
    {
        /// <summary>
        /// New employee login without any link to database
        /// </summary>
        /// <returns>Employee login instance</returns>
        public EmployeeLogin NewEmployeeLogin(Employee employee, string login)
        {
            return New<EmployeeLogin>((e) =>
            {
                e.Employee = employee;
                e.Login = login;

                employee.Logins.Add(e);
            });
        }
    }
}
