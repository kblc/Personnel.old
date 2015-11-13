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
        /// Get information about all stuffing
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.StaffingExecutionResults StaffingsGet();

        /// <summary>
        /// Get information about specified stuffing identifiers
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.StaffingExecutionResults StaffingGetRange(IEnumerable<long> identifiers);

        /// <summary>
        /// Get information about staffing
        /// </summary>
        /// <param name="staffingId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.StaffingExecutionResult StaffingGet(long staffingId);

        /// <summary>
        /// Delete single staffing
        /// </summary>
        /// <param name="staffingId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult StaffingRemove(long staffingId);

        /// <summary>
        /// Delete staffings
        /// </summary>
        /// <param name="identifiers">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult StaffingRemoveRange(IEnumerable<long> staffingIds);

        /// <summary>
        /// Update single staffing
        /// </summary>
        /// <param name="staffing">Staffing</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.StaffingExecutionResult StaffingUpdate(Model.Staffing stuffing);

        /// <summary>
        /// Insert single staffing
        /// </summary>
        /// <param name="staffing">Staffing</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.StaffingExecutionResult StaffingInsert(Model.Staffing stuffing);
    }

    public partial interface IStaffingServiceREST
    {
        /// <summary>
        /// Get information about all stuffing
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/staffing",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.StaffingExecutionResults RESTStaffingsGet();

        /// <summary>
        /// Get information about specified stagging identifiers
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/staffing/range",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.StaffingExecutionResults RESTStaffingGetRange(IEnumerable<string> identifiers);

        /// <summary>
        /// Get information about staffing
        /// </summary>
        /// <param name="staffingId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/staffing/{staffingId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.StaffingExecutionResult RESTStaffingGet(string staffingId);

        /// <summary>
        /// Delete single staffing
        /// </summary>
        /// <param name="staffingId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", UriTemplate = "/staffing/{staffingId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTStaffingRemove(string staffingId);

        /// <summary>
        /// Delete staffings
        /// </summary>
        /// <param name="identifiers">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/staffing/remove",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTStaffingRemoveRange(IEnumerable<string> staffingIds);

        /// <summary>
        /// Update single staffing
        /// </summary>
        /// <param name="staffing">Staffing</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/staffing",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.StaffingExecutionResult RESTStaffingUpdate(Model.Staffing stuffing);

        /// <summary>
        /// Insert single staffing
        /// </summary>
        /// <param name="staffing">Staffing</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", UriTemplate = "/staffing",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.StaffingExecutionResult RESTStaffingInsert(Model.Staffing stuffing);
    }
}
