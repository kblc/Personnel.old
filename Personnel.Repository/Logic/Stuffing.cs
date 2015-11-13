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
        /// Get new stuffing without any link to database
        /// </summary>
        /// <returns>Stuffing instance</returns>
        public Staffing NewStuffing(Department department, Appoint appoint, long position)
        {
            return New<Staffing>(s =>
            {
                s.Appoint = appoint;
                s.Department = department;
                s.Position = position;
            });
        }
    }
}
