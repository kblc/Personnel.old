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
        /// New department without any link to database
        /// </summary>
        /// <returns>Department instance</returns>
        public Department NewDepartment(string name)
        {
            return New<Department>(d => d.Name = name);
        }
    }
}
