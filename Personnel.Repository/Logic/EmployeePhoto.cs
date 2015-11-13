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
        /// New employee photo without any link to database
        /// </summary>
        /// <returns>Employee photo instance</returns>
        public EmployeePhoto NewEmployeePhoto(Employee employee, Picture picture)
        {
            return New<EmployeePhoto>((e) =>
            {
                e.Employee = employee;
                e.Picture = picture;
                employee.Photos.Add(e);
            });
        }
    }
}
