using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.History
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IHistoryService : Base.IBaseService
    {
        /// <summary>
        /// Get history long polling request
        /// </summary>
        [OperationContract]
        Model.HistoryExecutionResult Get();
        /// <summary>
        /// Get history long polling request
        /// </summary>
        /// <param name="eventId">Start event from</param>
        [OperationContract]
        Model.HistoryExecutionResult GetFrom(long eventId);
    }

    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IHistoryServiceREST : Base.IBaseService
    {
        /// <summary>
        /// Get history long polling request
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "/get", BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        Model.HistoryExecutionResult RESTGet();
        /// <summary>
        /// Get history long polling request
        /// </summary>
        /// <param name="eventId">Start event from</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/get?from={eventId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        Model.HistoryExecutionResult RESTGetFrom(long eventId);
    }
}
