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
        /// Delete single vacation agreement
        /// </summary>
        /// <param name="vacationAgreementId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult VacationAgreementRemove(long vacationAgreementId)
            => VacationAgreementRemoveRange(new long[] { vacationAgreementId  });

        /// <summary>
        /// Delete vacation agreement range
        /// </summary>
        /// <param name="vacationAgreementIds">Identifiers</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult VacationAgreementRemoveRange(IEnumerable<long> vacationAgreementIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var current = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);
                        var res = rep.Get<Repository.Model.VacationAgreement>(e => vacationAgreementIds.Contains(e.VacationAgreementId)).ToArray();
                        rep.RemoveRange(res);
                        return new Model.BaseExecutionResult();
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.BaseExecutionResult(ex);
                }
        }

        /// <summary>
        /// Insert single vacation agreement
        /// </summary>
        /// <param name="vacation">Vacation agreement</param>
        /// <returns>Result info</returns>
        public Model.VacationAgreementExecutionResult VacationAgreementInsert(Model.VacationAgreement vacationAgreement)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var current = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);
                        var newDbValue = AutoMapper.Mapper.Map<Repository.Model.VacationAgreement>(vacationAgreement);
                        newDbValue.VacationId = 0;
                        rep.Add(newDbValue);
                        return new Model.VacationAgreementExecutionResult(AutoMapper.Mapper.Map<Model.VacationAgreement>(newDbValue));
                    }
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
        /// <param name="vacationId">Vacation identifier</param>
        /// <returns>Result info</returns>
        public Model.VacationAgreementExecutionResult VacationAgreementInsertByUser(long vacationId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var current = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);
                        return VacationAgreementInsert(new Model.VacationAgreement()
                        {
                            Date = DateTime.UtcNow,
                            EmployeeId = current.Id,
                            VacationId = vacationId
                        });
                    }
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
