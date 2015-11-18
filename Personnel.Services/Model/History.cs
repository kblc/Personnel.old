using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal class RepositoryResolvingAttribute : Attribute
    {
        public Type EntityType { get; set; }
        public Type ReturnArrayElementType { get; set; }

        public RepositoryResolvingAttribute() { }
        public RepositoryResolvingAttribute(Type entityType) { EntityType = entityType; ReturnArrayElementType = entityType; }
        public RepositoryResolvingAttribute(Type entityType, Type returnArrayElementType) { EntityType = entityType; ReturnArrayElementType = returnArrayElementType; }
    }

    [DataContract]
    public class HistoryUpdateInfo
    {
        /// <summary>
        /// Departments
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.Department))]
        public Department[] Department { get; set; }

        /// <summary>
        /// Employees
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.Employee))]
        public Employee[] Employee { get; set; }

        /// <summary>
        /// Employee rights
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.EmployeeRight))]
        public EmployeeRight[] EmployeeRight { get; set; }

        /// <summary>
        /// Employee logins
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.EmployeeLogin))]
        public EmployeeLogin[] EmployeeLogin { get; set; }

        /// <summary>
        /// Employee photos
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.EmployeePhoto))]
        public EmployeePhoto[] EmployeePhoto { get; set; }

        /// <summary>
        /// Stuffing
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.Staffing))]
        public Staffing[] Stuffing { get; set; }
        
        /// <summary>
        /// Files
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.File))]
        public File[] File { get; set; }
    }

    [DataContract]
    public class HistoryRemoveInfo
    {
        /// <summary>
        /// Departments
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.Department), typeof(long))]
        public long[] Departments { get; set; }

        /// <summary>
        /// Employees
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.Employee), typeof(long))]
        public long[] Employees { get; set; }

        /// <summary>
        /// Employee rights
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.EmployeeRight), typeof(long))]
        public long[] EmployeeRights { get; set; }

        /// <summary>
        /// Employee logins
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.EmployeeLogin), typeof(long))]
        public long[] EmployeeLogins { get; set; }

        /// <summary>
        /// Employee photos
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.EmployeePhoto), typeof(Guid))]
        public Guid[] EmployeePhotos { get; set; }

        /// <summary>
        /// Stuffing
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.Staffing), typeof(long))]
        public long[] Stuffing { get; set; }

        /// <summary>
        /// Files
        /// </summary>
        [DataMember(IsRequired = false)]
        [RepositoryResolving(typeof(Repository.Model.File), typeof(long))]
        public long[] File { get; set; }
    }

    /// <summary>
    /// History record
    /// </summary>
    [DataContract]
    public class History
    {
        /// <summary>
        /// History last identifier
        /// </summary>
        [DataMember(IsRequired = true)]
        public long EventId { get; set; }

        /// <summary>
        /// Added info
        /// </summary>
        [DataMember(IsRequired = false)]
        public HistoryUpdateInfo Add { get; set; }

        /// <summary>
        /// Changed info
        /// </summary>
        [DataMember(IsRequired = false)]
        public HistoryUpdateInfo Change { get; set; }

        /// <summary>
        /// Removed info
        /// </summary>
        [DataMember(IsRequired = false)]
        public HistoryRemoveInfo Remove { get; set; }
    }


    [DataContract(Name = "HistoryResult")]
    public class HistoryExecutionResult : BaseExecutionResult<History>
    {
        public HistoryExecutionResult() { }
        public HistoryExecutionResult(History e) : base(e) { }
        public HistoryExecutionResult(Exception ex) : base(ex) { }
    }
}
