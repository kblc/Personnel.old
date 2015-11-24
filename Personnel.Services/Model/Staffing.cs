using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Stuffing
    /// </summary>
    [DataContract]
    [Serializable]
    public class Staffing
    {
        /// <summary>
        /// Stuffing identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Position { get; set; }

        /// <summary>
        /// Department
        /// </summary>
        [DataMember(IsRequired = false)]
        public long DepartmentId { get; set; }

        /// <summary>
        /// Appoint
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Appoint { get; set; }
    }

    [DataContract(Name = "StaffingResult")]
    public class StaffingExecutionResult : BaseExecutionResult<Staffing>
    {
        public StaffingExecutionResult() { }
        public StaffingExecutionResult(Staffing e) : base(e) { }
        public StaffingExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "StaffingResults")]
    public class StaffingExecutionResults : BaseExecutionResults<Staffing>
    {
        public StaffingExecutionResults() { }
        public StaffingExecutionResults(Staffing[] e) : base(e) { }
        public StaffingExecutionResults(Exception ex) : base(ex) { }
    }
}
