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
        /// New employee without any link to database
        /// </summary>
        /// <returns>Employee instance</returns>
        public Employee NewEmployee(string name, string surname, string patronymic = null, DateTime? birthday = null, string email = null, string phone = null)
        {
            return New<Employee>((e) =>
            {
                e.Name = name;
                e.Surname = surname;
                e.Patronymic = patronymic;
                e.Birthday = birthday;
                e.Email = email;
                e.Phone = phone;
            });
        }
    }
}
