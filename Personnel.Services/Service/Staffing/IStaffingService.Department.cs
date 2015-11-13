using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Personnel.Services.Service.Staffing
{
    public partial interface IStaffingService
    {
        /// <summary>
        /// Get information about all appoint
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.DepartmentExecutionResults DepartmentsGet();

        /// <summary>
        /// Get information about specified appoint identifiers
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.DepartmentExecutionResults DepartmentGetRange(IEnumerable<long> identifiers);

        /// <summary>
        /// Get information about department
        /// </summary>
        /// <param name="departmentId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.DepartmentExecutionResult DepartmentGet(long departmentId);

        /// <summary>
        /// Delete single department
        /// </summary>
        /// <param name="departmentId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult DepartmentRemove(long departmentId);

        /// <summary>
        /// Delete department
        /// </summary>
        /// <param name="identifiers">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult DepartmentRemoveRange(IEnumerable<long> departmentIds);

        /// <summary>
        /// Update single department
        /// </summary>
        /// <param name="department">Department</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.DepartmentExecutionResult DepartmentUpdate(Model.Department department);

        /// <summary>
        /// Insert single department
        /// </summary>
        /// <param name="department">Department</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.DepartmentExecutionResult DepartmentInsert(Model.Department department);
    }

    public partial interface IStaffingServiceREST
    {
        /// <summary>
        /// Get information about all appoint
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/department",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.DepartmentExecutionResults RESTDepartmentsGet();

        /// <summary>
        /// Get information about specified appoint identifiers
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/departments",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.DepartmentExecutionResults RESTDepartmentGetRange(IEnumerable<string> identifiers);

        /// <summary>
        /// Get information about department
        /// </summary>
        /// <param name="departmentId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/department/{departmentId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.DepartmentExecutionResult RESTDepartmentGet(string departmentId);

        /// <summary>
        /// Delete single department
        /// </summary>
        /// <param name="departmentId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", UriTemplate = "/department/{departmentId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTDepartmentRemove(string departmentId);

        /// <summary>
        /// Delete department
        /// </summary>
        /// <param name="identifiers">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/department/remove",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTDepartmentRemoveRange(IEnumerable<string> departmentIds);

        /// <summary>
        /// Update single department
        /// </summary>
        /// <param name="department">Department</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/department",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.DepartmentExecutionResult RESTDepartmentUpdate(Model.Department department);

        /// <summary>
        /// Insert single department
        /// </summary>
        /// <param name="department">Department</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", UriTemplate = "/department",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.DepartmentExecutionResult RESTDepartmentInsert(Model.Department department);
    }

}
