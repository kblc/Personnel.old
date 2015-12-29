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
    public class VacationBalance
    {
        /// <summary>
        /// VacationBalance identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Employee identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long EmployeeId { get; set; }

        /// <summary>
        /// Day count
        /// </summary>
        [DataMember(IsRequired = false)]
        public long DayCountMain { get; set; }

        /// <summary>
        /// Day count
        /// </summary>
        [DataMember(IsRequired = false)]
        public long DayCountAdditional { get; set; }

        /// <summary>
        /// Record update date
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime Updated { get; set; }
    }

    [DataContract(Name = "VacationBalanceResult")]
    public class VacationBalanceExecutionResult : BaseExecutionResult<VacationBalance>
    {
        public VacationBalanceExecutionResult() { }
        public VacationBalanceExecutionResult(VacationBalance e) : base(e) { }
        public VacationBalanceExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "VacationBalanceResults")]
    public class VacationBalanceExecutionResults : BaseExecutionResults<VacationBalance>
    {
        public VacationBalanceExecutionResults() { }
        public VacationBalanceExecutionResults(VacationBalance[] e) : base(e) { }
        public VacationBalanceExecutionResults(Exception ex) : base(ex) { }
    }
}
