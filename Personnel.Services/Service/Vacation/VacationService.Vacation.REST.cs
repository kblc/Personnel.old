using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using Helpers.Linq;

namespace Personnel.Services.Service.Vacation
{
    public partial class VacationService : IVacationServiceREST
    {
        /// <summary>
        /// Get information about vacations
        /// </summary>
        /// <param name="from">Date from</param>
        /// <param name="from">Date to</param>
        /// <returns>Result info</returns>
        public Model.VacationExecutionResults RESTVacationsGet(string from, string to)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var fromD = TryDateTimeFromString(from);
                    var toD = TryDateTimeFromString(to);
                    return VacationsGet(fromD, toD);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(from), from);
                    ex.Data.Add(nameof(to), to);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get information about vacation
        /// </summary>
        /// <param name="vacationId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.VacationExecutionResult RESTVacationGet(string vacationId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    return VacationGet(LongFromString(vacationId));
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(vacationId), vacationId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationExecutionResult(ex);
                }
        }

        /// <summary>
        /// Delete single vacation
        /// </summary>
        /// <param name="vacationId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult RESTVacationRemove(string vacationId) => RESTVacationRemoveRange(new string[] { vacationId });

        /// <summary>
        /// Delete vacations
        /// </summary>
        /// <param name="vacationIds">Identifiers</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult RESTVacationRemoveRange(IEnumerable<string> vacationIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    return VacationRemoveRange(vacationIds.Select(i => LongFromString(i)));
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(vacationIds), vacationIds.Concat(i => $"'{i}'",", "));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationExecutionResult(ex);
                }
        }

        /// <summary>
        /// Update single vacation
        /// </summary>
        /// <param name="staffing">Vacation</param>
        /// <returns>Result info</returns>
        public Model.VacationExecutionResult RESTVacationUpdate(Model.Vacation vacation) => VacationUpdate(vacation);

        /// <summary>
        /// Insert single vacation
        /// </summary>
        /// <param name="vacation">Vacation</param>
        /// <returns>Result info</returns>
        public Model.VacationExecutionResult RESTVacationInsert(Model.Vacation vacation) => VacationInsert(vacation);
    }
}
