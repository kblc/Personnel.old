using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Staffing
{
    public partial interface IStaffingService
    {
        /// <summary>
        /// Get logins for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        [OperationContract]
        Model.LoginValueExecutionResults EmployeeLoginsGet(long employeeId);

        /// <summary>
        /// Add logins to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="logins">System login name</param>
        [OperationContract]
        Model.LoginValueExecutionResults EmployeeLoginsAdd(long employeeId, string[] logins);

        /// <summary>
        /// Add login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="name">System login name</param>
        [OperationContract]
        Model.LoginValueExecutionResults EmployeeLoginsAddOne(long employeeId, string name);

        /// <summary>
        /// Remove login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="logins">System login identifiers</param>
        [OperationContract]
        Model.LoginValueExecutionResults EmployeeLoginsRemove(long employeeId, string[] logins);

        /// <summary>
        /// Remove login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="login">System login name</param>
        [OperationContract]
        Model.LoginValueExecutionResults EmployeeLoginsRemoveOne(long employeeId, string login);

        /// <summary>
        /// Update employee Logins
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="identifiers">Logins</param>
        [OperationContract]
        Model.LoginValueExecutionResults EmployeeLoginsUpdate(long employeeId, string[] identifiers);
    }

    public partial interface IStaffingServiceREST
    {
        /// <summary>
        /// Get Logins for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/logins")]
        Model.LoginValueExecutionResults RESTEmployeeLoginsGet(string employeeId);

        /// <summary>
        /// Add login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="logins">Logins to add</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/logins/add")]
        Model.LoginValueExecutionResults RESTEmployeeLoginsAdd(string employeeId, string[] logins);

        /// <summary>
        /// Add login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="login">Login to add</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/logins")]
        Model.LoginValueExecutionResults RESTEmployeeLoginsAddOne(string employeeId, string login);

        /// <summary>
        /// Remove login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="logins">Login to remove</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/logins/remove")]
        Model.LoginValueExecutionResults RESTEmployeeLoginsRemove(string employeeId, string[] logins);

        /// <summary>
        /// Remove login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="login">Login to remove</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/logins")]
        Model.LoginValueExecutionResults RESTEmployeeLoginsRemoveOne(string employeeId, string login);

        /// <summary>
        /// Update employee Logins
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="logins">Logins</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/logins")]
        Model.LoginValueExecutionResults RESTEmployeeLoginsUpdate(string employeeId, string[] logins);
    }

}
