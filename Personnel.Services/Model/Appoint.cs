using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Appoint
    /// </summary>
    [DataContract]
    [Serializable]
    public class Appoint : BaseModel
    {
        /// <summary>
        /// Appoint identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Appoint name
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Name { get; set; }
    }

    [DataContract(Name = "AppointResult")]
    public class AppointExecutionResult : BaseExecutionResult<Appoint>
    {
        public AppointExecutionResult() { }
        public AppointExecutionResult(Appoint e) : base(e) { }
        public AppointExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "AppointResults")]
    public class AppointExecutionResults : BaseExecutionResults<Appoint>
    {
        public AppointExecutionResults() { }
        public AppointExecutionResults(Appoint[] e) : base(e) { }
        public AppointExecutionResults(Exception ex) : base(ex) { }
    }
}
