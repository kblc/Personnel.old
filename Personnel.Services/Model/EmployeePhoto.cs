using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Employee photo
    /// </summary>
    [DataContract]
    public class EmployeePhoto
    {
        /// <summary>
        /// Employee identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long EmployeeId { get; set; }

        /// <summary>
        /// Employee photo identifier
        /// </summary>
        [DataMember]
        public Guid FileId { get; set; }

        /// <summary>
        /// Employee photo identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public Picture Picture { get; set; }
    }

    [DataContract(Name = "EmployeePhotoResult")]
    public class EmployeePhotoExecutionResult : BaseExecutionResult<EmployeePhoto>
    {
        public EmployeePhotoExecutionResult() { }
        public EmployeePhotoExecutionResult(EmployeePhoto e) : base(e) { }
        public EmployeePhotoExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "EmployeePhotoResults")]
    public class EmployeePhotoExecutionResults : BaseExecutionResults<EmployeePhoto>
    {
        public EmployeePhotoExecutionResults() { }
        public EmployeePhotoExecutionResults(EmployeePhoto[] e) : base(e) { }
        public EmployeePhotoExecutionResults(Exception ex) : base(ex) { }
    }
}
