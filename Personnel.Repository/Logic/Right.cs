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
        /// Get new right by right type
        /// </summary>
        /// <returns>Right</returns>
        public Right Get(RightType rightType)
        {
#pragma warning disable 618
            return Get<Right>(r => string.Compare(r.SystemName, rightType.ToString(), true) == 0).FirstOrDefault();
#pragma warning restore 618
        }
    }
}
