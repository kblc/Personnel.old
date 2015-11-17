using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Employee
    /// </summary>
    [DataContract]
    [Serializable]
    public class Employee : BaseModel
    {
        /// <summary>
        /// Employee identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Surname
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Surname { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        /// <summary>
        /// Patronymic
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Patronymic { get; set; }

        /// <summary>
        /// Birthday
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Email { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Phone { get; set; }

        /// <summary>
        /// Stuffing
        /// </summary>
        [DataMember(IsRequired = false)]
        public Staffing Stuffing { get; set; }

        /// <summary>
        /// Logins
        /// </summary>
        [DataMember]
        public EmployeeLogin[] Logins { get; set; }

        /// <summary>
        /// Photoes
        /// </summary>
        [DataMember]
        public EmployeePhoto[] Photos { get; set; }

        /// <summary>
        /// Rights
        /// </summary>
        [DataMember]
        public EmployeeRight[] Rights { get; set; }
    }


    [DataContract(Name = "EmployeeResult")]
    public class EmployeeExecutionResult : BaseExecutionResult<Employee>
    {
        public EmployeeExecutionResult() { }
        public EmployeeExecutionResult(Employee e) : base (e) { }
        public EmployeeExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "EmployeeResults")]
    public class EmployeeExecutionResults : BaseExecutionResults<Employee>
    {
        public EmployeeExecutionResults() { }
        public EmployeeExecutionResults(Employee[] e) : base(e) { }
        public EmployeeExecutionResults(Exception ex) : base(ex) { }
    }
}
