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
        Model.AppointExecutionResults AppointsGet();

        /// <summary>
        /// Get information about specified appoint identifiers
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.AppointExecutionResults AppointGetRange(IEnumerable<long> identifiers);

        /// <summary>
        /// Get information about appoint
        /// </summary>
        /// <param name="appointId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.AppointExecutionResult AppointGet(long appointId);

        /// <summary>
        /// Delete single appoint
        /// </summary>
        /// <param name="appointId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult AppointRemove(long appointId);

        /// <summary>
        /// Delete appoints
        /// </summary>
        /// <param name="identifiers">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult AppointRemoveRange(IEnumerable<long> identifiers);

        /// <summary>
        /// Update single appoint
        /// </summary>
        /// <param name="appoint">Appoint</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.AppointExecutionResult AppointUpdate(Model.Appoint appoint);

        /// <summary>
        /// Insert single appoint
        /// </summary>
        /// <param name="appoint">Appoint</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.AppointExecutionResult AppointInsert(Model.Appoint appoint);
    }

    public partial interface IStaffingServiceREST
    {
        /// <summary>
        /// Get information about all appoint
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/appoint", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.AppointExecutionResults RESTAppointsGet();

        /// <summary>
        /// Get information about specified appoint identifiers
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/appoints", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.AppointExecutionResults RESTAppointGetRange(IEnumerable<string> identifiers);

        /// <summary>
        /// Get information about appoint
        /// </summary>
        /// <param name="appointId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/appoint/{appointId}", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.AppointExecutionResult RESTAppointGet(string appointId);

        /// <summary>
        /// Delete single appoint
        /// </summary>
        /// <param name="appointId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/appoint/{appointId}", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTAppointRemove(string appointId);

        /// <summary>
        /// Delete appoints
        /// </summary>
        /// <param name="identifiers">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/appoint/remove", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTAppointRemoveRange(IEnumerable<string> identifiers);

        /// <summary>
        /// Update single appoint
        /// </summary>
        /// <param name="appoint">Appoint</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/appoint", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.AppointExecutionResult RESTAppointUpdate(Model.Appoint appoint);

        /// <summary>
        /// Insert single appoint
        /// </summary>
        /// <param name="appoint">Appoint</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/appoint", Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.AppointExecutionResult RESTAppointInsert(Model.Appoint appoint);
    }
}
