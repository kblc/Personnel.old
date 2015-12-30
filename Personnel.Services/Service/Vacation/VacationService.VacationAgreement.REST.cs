using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace Personnel.Services.Service.Vacation
{
    public partial class VacationService : IVacationServiceREST
    {
        /// <summary>
        /// Delete single vacation
        /// </summary>
        /// <param name="vacationAgreementId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult RESTVacationAgreementRemove(string vacationAgreementId)
            => RESTVacationAgreementRemoveRange(new string[] { vacationAgreementId });

        /// <summary>
        /// Delete vacations
        /// </summary>
        /// <param name="vacationAgreementIds">Identifiers</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult RESTVacationAgreementRemoveRange(IEnumerable<string> vacationAgreementIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = vacationAgreementIds.Select(id => LongFromString(id));
                    return VacationAgreementRemoveRange(ids);
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationAgreementExecutionResult(ex);
                }
        }

        /// <summary>
        /// Insert single vacation agreement
        /// </summary>
        /// <param name="vacationAgreement">Vacation agreement</param>
        /// <returns>Result info</returns>
        public Model.VacationAgreementExecutionResult RESTVacationAgreementInsert(Model.VacationAgreement vacationAgreement)
            => VacationAgreementInsert(vacationAgreement);

        /// <summary>
        /// Insert single vacation agreement
        /// </summary>
        /// <param name="vacationId">Vacation identifier</param>
        /// <returns>Result info</returns>
        public Model.VacationAgreementExecutionResult RESTVacationAgreementInsertByUser(string vacationId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(vacationId);
                    return VacationAgreementInsertByUser(id);
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationAgreementExecutionResult(ex);
                }
        }
    }
}
