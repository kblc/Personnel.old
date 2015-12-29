using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Vacation
{
    public partial interface IVacationService
    {
        /// <summary>
        /// Get alailable levels for vacation
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationLevelExecutionResults GetLevels();
    }

    public partial interface IVacationServiceREST
    {
        /// <summary>
        /// Get alailable levels for vacation
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/vacation/levels",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationLevelExecutionResults RESTGetLevels();
    }
}
