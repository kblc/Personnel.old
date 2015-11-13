using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Department
    /// </summary>
    [DataContract]
    public class Department
    {
        /// <summary>
        /// Department identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Parent department
        /// </summary>
        [DataMember(IsRequired = false)]
        public long? ParentId { get; set; }

        /// <summary>
        /// Department name
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Name { get; set; }
    }

    [DataContract(Name = "DepartmentResult")]
    public class DepartmentExecutionResult : BaseExecutionResult<Department>
    {
        public DepartmentExecutionResult() { }
        public DepartmentExecutionResult(Department e) : base(e) { }
        public DepartmentExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "DepartmentResults")]
    public class DepartmentExecutionResults : BaseExecutionResults<Department>
    {
        public DepartmentExecutionResults() { }
        public DepartmentExecutionResults(Department[] e) : base(e) { }
        public DepartmentExecutionResults(Exception ex) : base(ex) { }
    }
}
