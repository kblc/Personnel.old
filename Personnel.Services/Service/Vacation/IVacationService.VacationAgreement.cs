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
        /// Delete single vacation agreement
        /// </summary>
        /// <param name="vacationAgreementId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult VacationAgreementRemove(long vacationAgreementId);

        /// <summary>
        /// Delete vacation agreement range
        /// </summary>
        /// <param name="vacationAgreementIds">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult VacationAgreementRemoveRange(IEnumerable<long> vacationAgreementIds);

        /// <summary>
        /// Insert single vacation agreement
        /// </summary>
        /// <param name="vacation">Vacation agreement</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationAgreementExecutionResult VacationAgreementInsert(Model.VacationAgreement vacationAgreement);

        /// <summary>
        /// Insert single vacation agreement
        /// </summary>
        /// <param name="vacationId">Vacation identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationAgreementExecutionResult VacationAgreementInsertByUser(long vacationId);
    }

    public partial interface IVacationServiceREST
    {
        /// <summary>
        /// Delete single vacation
        /// </summary>
        /// <param name="vacationAgreementId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", UriTemplate = "/agreement/{vacationAgreementId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTVacationAgreementRemove(string vacationAgreementId);

        /// <summary>
        /// Delete vacations
        /// </summary>
        /// <param name="vacationAgreementIds">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/agreement/remove",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTVacationAgreementRemoveRange(IEnumerable<string> vacationAgreementIds);

        /// <summary>
        /// Insert single vacation agreement
        /// </summary>
        /// <param name="vacation">Vacation agreement</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", UriTemplate = "/agreement",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationAgreementExecutionResult RESTVacationAgreementInsert(Model.VacationAgreement vacationAgreement);

        /// <summary>
        /// Insert single vacation agreement
        /// </summary>
        /// <param name="vacationAgreement">Vacation agreement</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", UriTemplate = "/agreement/sign/{vacationId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationAgreementExecutionResult RESTVacationAgreementInsertByUser(string vacationId);
    }
}
