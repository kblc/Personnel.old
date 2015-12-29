using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Vacation
    /// </summary>
    [DataContract]
    [Serializable]
    public class Vacation
    {
        /// <summary>
        /// Stuffing identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Employee identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long EmployeeId { get; set; }

        /// <summary>
        /// Vacation level
        /// </summary>
        [DataMember(IsRequired = false)]
        public long VacationLevelId { get; set; }

        /// <summary>
        /// Begin date
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime Begin { get; set; }

        /// <summary>
        /// Day count
        /// </summary>
        [DataMember(IsRequired = false)]
        public long DayCount { get; set; }

        /// <summary>
        /// Is this vacation is not used
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool NotUsed { get; set; }
    }

    [DataContract(Name = "VacationResult")]
    public class VacationExecutionResult : BaseExecutionResult<Vacation>
    {
        public VacationExecutionResult() { }
        public VacationExecutionResult(Vacation e) : base(e) { }
        public VacationExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "VacationResults")]
    public class VacationExecutionResults : BaseExecutionResults<Vacation>
    {
        public VacationExecutionResults() { }
        public VacationExecutionResults(Vacation[] e) : base(e) { }
        public VacationExecutionResults(Exception ex) : base(ex) { }
    }
}
