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
        /// Get information about current employee
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Model.EmployeeExecutionResult EmployeeGetCurrent();

        /// <summary>
        /// Get information about specified employee identifiers
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Model.EmployeeExecutionResults EmployeeGetRange(IEnumerable<long> identifiers);

        /// <summary>
        /// Get information about all employee
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Model.EmployeeExecutionResults EmployeesGet();

        /// <summary>
        /// Get information about single employee
        /// </summary>
        /// <returns>Employee information</returns>
        [OperationContract]
        Model.EmployeeExecutionResult EmployeeGet(long employeeId);

        /// <summary>
        /// Delete single employee
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult EmployeeRemove(long employeeId);

        /// <summary>
        /// Delete single employee
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult EmployeeRemoveRange(IEnumerable<long> employeeIds);

        /// <summary>
        /// Update single employee
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.EmployeeExecutionResult EmployeeUpdate(Model.Employee employee);

        /// <summary>
        /// Add single employee
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.EmployeeExecutionResult EmployeeInsert(Model.Employee employee);
    }

    public partial interface IStaffingServiceREST
    {
        /// <summary>
        /// Get information about current employee
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/me",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.EmployeeExecutionResult RESTEmployeeGetCurrent();

        /// <summary>
        /// Get information about specified employee identifiers
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/employees",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.EmployeeExecutionResults RESTEmployeeGetRange(IEnumerable<string> identifiers);

        /// <summary>
        /// Get information about all employee
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/employee",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.EmployeeExecutionResults RESTEmployeesGet();

        /// <summary>
        /// Get information about single employee
        /// </summary>
        /// <returns>Employee information</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/employee/{employeeId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.EmployeeExecutionResult RESTEmployeeGet(string employeeId);

        /// <summary>
        /// Delete single employee
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", UriTemplate = "/employee/{employeeId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTEmployeeRemove(string employeeId);

        /// <summary>
        /// Delete single employee
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/employee/remove",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTEmployeeRemoveRange(IEnumerable<string> employeeIds);

        /// <summary>
        /// Update single employee
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/employee",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.EmployeeExecutionResult RESTEmployeeUpdate(Model.Employee employee);

        /// <summary>
        /// Add single employee
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", UriTemplate = "/employee",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.EmployeeExecutionResult RESTEmployeeInsert(Model.Employee employee);
    }
}
