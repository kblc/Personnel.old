using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Vacation agreement
    /// </summary>
    [DataContract]
    [Serializable]
    public class VacationAgreement
    {
        /// <summary>
        /// VacationAgreement identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Vacation identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long VacationId { get; set; }

        /// <summary>
        /// Employee who agree
        /// </summary>
        [DataMember(IsRequired = false)]
        public long EmployeeId { get; set; }

        /// <summary>
        /// Agree date
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime Date { get; set; }
    }

    [DataContract(Name = "VacationAgreementResult")]
    public class VacationAgreementExecutionResult : BaseExecutionResult<VacationAgreement>
    {
        public VacationAgreementExecutionResult() { }
        public VacationAgreementExecutionResult(VacationAgreement e) : base(e) { }
        public VacationAgreementExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "VacationAgreementResults")]
    public class VacationAgreementExecutionResults : BaseExecutionResults<VacationAgreement>
    {
        public VacationAgreementExecutionResults() { }
        public VacationAgreementExecutionResults(VacationAgreement[] e) : base(e) { }
        public VacationAgreementExecutionResults(Exception ex) : base(ex) { }
    }
}
