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
    public partial class VacationService : IVacationService
    {
        /// <summary>
        /// Get information about employee vacation balances
        /// </summary>
        /// <returns>Result info</returns>
        public Model.VacationBalanceExecutionResults VacationBalanceGet()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ViewVacation);

                        var dbRes = rep.Get<Repository.Model.VacationBalance>(asNoTracking: true)
                            .GroupBy(v => v.EmployeeId)
                            .Select(g => g.OrderByDescending(i => i.Updated).First())
                            .ToArray();

                        var res = dbRes.Select(i => AutoMapper.Mapper.Map<Model.VacationBalance>(i));

                        return new Model.VacationBalanceExecutionResults(res.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationBalanceExecutionResults(ex);
                }
        }

        /// <summary>
        /// Insert single vacation balance
        /// </summary>
        /// <param name="vacationBalance">Vacation balance</param>
        /// <returns>Result info</returns>
        public Model.VacationBalanceExecutionResult VacationBalanceInsert(Model.VacationBalance vacationBalance)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var current = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageVacation);
                        var newDbValue = AutoMapper.Mapper.Map<Repository.Model.VacationBalance>(vacationBalance);
                        newDbValue.VacationBalanceId = 0;
                        newDbValue.Updated = DateTime.UtcNow;
                        rep.Add(newDbValue);

                        return new Model.VacationBalanceExecutionResult(AutoMapper.Mapper.Map<Model.VacationBalance>(newDbValue));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationBalanceExecutionResult(ex);
                }
        }
    }
}
