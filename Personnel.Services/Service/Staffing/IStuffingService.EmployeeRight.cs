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
        /// Get all rights
        /// </summary>
        /// <returns>Rights</returns>
        [OperationContract]
        Model.RightExecutionResults RightsGet();

        /// <summary>
        /// Get rights for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        [OperationContract]
        Model.RightValueExecutionResults EmployeeRightsGet(long employeeId);

        /// <summary>
        /// Add right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">System right name</param>
        [OperationContract]
        Model.RightValueExecutionResults EmployeeRightsAdd(long employeeId, long[] rightIds);

        /// <summary>
        /// Add right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightName">System right name</param>
        [OperationContract]
        Model.RightValueExecutionResults EmployeeRightsAddByName(long employeeId, string rightName);

        /// <summary>
        /// Remove right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">System right identifiers</param>
        [OperationContract]
        Model.RightValueExecutionResults EmployeeRightsRemove(long employeeId, long[] rightIds);

        /// <summary>
        /// Remove right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightName">System right name</param>
        [OperationContract]
        Model.RightValueExecutionResults EmployeeRightsRemoveByName(long employeeId, string rightName);

        /// <summary>
        /// Update employee rights
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">Rights</param>
        [OperationContract]
        Model.RightValueExecutionResults EmployeeRightsUpdate(long employeeId, long[] rightIds);
    }

    public partial interface IStaffingServiceREST
    {
        /// <summary>
        /// Get all rights
        /// </summary>
        /// <returns>Rights</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/rights")]
        Model.RightExecutionResults RESTRightsGet();

        /// <summary>
        /// Get rights for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/rights")]
        Model.RightValueExecutionResults RESTEmployeeRightsGet(string employeeId);

        /// <summary>
        /// Add right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">System right name</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/rights/add")]
        Model.RightValueExecutionResults RESTEmployeeRightsAdd(string employeeId, long[] rightIds);

        /// <summary>
        /// Add right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightName">System right name</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/rights")]
        Model.RightValueExecutionResults RESTEmployeeRightsAddByName(string employeeId, string rightName);

        /// <summary>
        /// Remove right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">System right identifiers</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/rights/remove")]
        Model.RightValueExecutionResults RESTEmployeeRightsRemove(string employeeId, long[] rightIds);

        /// <summary>
        /// Remove right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightName">System right name</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/rights")]
        Model.RightValueExecutionResults RESTEmployeeRightsRemoveByName(string employeeId, string rightName);

        /// <summary>
        /// Update employee rights
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">Rights</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/rights")]
        Model.RightValueExecutionResults RESTEmployeeRightsUpdate(string employeeId, long[] rightIds);
    }

}
