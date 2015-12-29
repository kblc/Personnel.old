using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Personnel.Services.Service.Vacation
{
    public partial interface IVacationService
    {
        /// <summary>
        /// Get information about vacations
        /// </summary>
        /// <param name="from">Date from</param>
        /// <param name="from">Date to</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationExecutionResults VacationsGet(DateTime? from, DateTime? to);

        /// <summary>
        /// Get information about vacation
        /// </summary>
        /// <param name="vacationId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationExecutionResult VacationGet(long vacationId);

        /// <summary>
        /// Delete single vacation
        /// </summary>
        /// <param name="vacationId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult VacationRemove(long vacationId);

        /// <summary>
        /// Delete vacations
        /// </summary>
        /// <param name="vacationIds">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult VacationRemoveRange(IEnumerable<long> vacationIds);

        /// <summary>
        /// Update single vacation
        /// </summary>
        /// <param name="vacation">Vacation</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationExecutionResult VacationUpdate(Model.Vacation vacation);

        /// <summary>
        /// Insert single vacation
        /// </summary>
        /// <param name="vacation">Vacation</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationExecutionResult VacationInsert(Model.Vacation vacation);
    }

    public partial interface IVacationServiceREST
    {
        /// <summary>
        /// Get information about vacations
        /// </summary>
        /// <param name="from">Date from</param>
        /// <param name="from">Date to</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/vacations?from={from}&to={to}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationExecutionResults RESTVacationsGet(string from, string to);

        /// <summary>
        /// Get information about vacation
        /// </summary>
        /// <param name="vacationId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/vacation/{vacationId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationExecutionResult RESTVacationGet(string vacationId);

        /// <summary>
        /// Delete single vacation
        /// </summary>
        /// <param name="vacationId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", UriTemplate = "/vacation/{vacationId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTVacationRemove(string vacationId);

        /// <summary>
        /// Delete vacations
        /// </summary>
        /// <param name="vacationIds">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/vacation/remove",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTVacationRemoveRange(IEnumerable<string> vacationIds);

        /// <summary>
        /// Update single vacation
        /// </summary>
        /// <param name="staffing">Vacation</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/vacation",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationExecutionResult RESTVacationUpdate(Model.Vacation vacation);

        /// <summary>
        /// Insert single vacation
        /// </summary>
        /// <param name="vacation">Vacation</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", UriTemplate = "/vacation",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationExecutionResult RESTVacationInsert(Model.Vacation vacation);
    }
}
