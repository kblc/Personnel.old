using Personnel.Repository.Additional;
using Personnel.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace Personnel.Repository.Logic
{
    public partial class Repository
    {
        /// <summary>
        /// New employee right without any link to database
        /// </summary>
        /// <returns>Employee right instance</returns>
        public EmployeeRight NewEmployeeRight(Employee employee, Right right)
        {
            return New<EmployeeRight>((e) =>
            {
                e.Employee = employee;
                e.Right = right;
                employee.Rights.Add(e);
            });
        }

        /// <summary>
        /// New employee right without any link to database
        /// </summary>
        /// <returns>Employee right instance</returns>
        public EmployeeRight NewEmployeeRight(Employee employee, RightType rightType)
        {
            try
            {
                var right = Get(rightType);
                return NewEmployeeRight(employee, right);
            }
            catch (Exception ex)
            {
                RaiseDatabaseLog(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(EmployeeRight).Name}>()"));
                throw;
            }
        }

    }
}
