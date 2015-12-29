using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Vacation level
    /// </summary>
    [DataContract]
    [Serializable]
    public class VacationLevel
    {
        /// <summary>
        /// Stuffing identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Vacation level name
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        /// <summary>
        /// Vacation level system name
        /// </summary>
        [DataMember(IsRequired = false)]
        public string SystemName { get; set; }
    }

    [DataContract(Name = "VacationLevelResult")]
    public class VacationLevelExecutionResult : BaseExecutionResult<VacationLevel>
    {
        public VacationLevelExecutionResult() { }
        public VacationLevelExecutionResult(VacationLevel e) : base(e) { }
        public VacationLevelExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "VacationLevelResults")]
    public class VacationLevelExecutionResults : BaseExecutionResults<VacationLevel>
    {
        public VacationLevelExecutionResults() { }
        public VacationLevelExecutionResults(VacationLevel[] e) : base(e) { }
        public VacationLevelExecutionResults(Exception ex) : base(ex) { }
    }
}
