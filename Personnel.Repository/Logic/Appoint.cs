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
        /// New appoint without any link to database
        /// </summary>
        /// <returns>Appoint instance</returns>
        public Appoint NewAppoint(string name)
        {
            return New<Appoint>(a => a.Name = name);
        }
    }
}
