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
        /// Get information about employee vacation balances
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationBalanceExecutionResults VacationBalanceGet();

        /// <summary>
        /// Insert single vacation balance
        /// </summary>
        /// <param name="vacation">Vacation balance</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationBalanceExecutionResult VacationBalanceInsert(Model.VacationBalance vacationBalance);
    }

    public partial interface IVacationServiceREST
    {
        /// <summary>
        /// Get information about employee vacation balances
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/vacation/balances",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationBalanceExecutionResults RESTVacationBalanceGet();

        /// <summary>
        /// Insert single vacation balance
        /// </summary>
        /// <param name="vacationBalance">Vacation balance</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", UriTemplate = "/vacation/balances",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationBalanceExecutionResult RESTVacationBalanceInsert(Model.Vacation vacationBalance);
    }
}
