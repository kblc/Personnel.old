using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Employee right
    /// </summary>
    [DataContract]
    public class EmployeeRight
    {
        /// <summary>
        /// Employee right identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Employee identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long EmployeeId { get; set; }

        /// <summary>
        /// Right identifier
        /// </summary>
        [DataMember]
        public long RightId { get; set; }
    }
}
