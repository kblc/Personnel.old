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
        /// Get information about vacations
        /// </summary>
        /// <param name="from">Date from</param>
        /// <param name="from">Date to</param>
        /// <returns>Result info</returns>
        public Model.VacationExecutionResults VacationsGet(DateTime? from, DateTime? to)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ViewVacation);

                        var dbRes = rep.Get<Repository.Model.Vacation>(asNoTracking: true);

                        if (from != null)
                            dbRes = dbRes.Where(i => i.Begin >= from);

                        if (to != null)
                            dbRes = dbRes.Where(i => i.Begin <= to);

                        var res = dbRes.ToArray().Select(i => AutoMapper.Mapper.Map<Model.Vacation>(i)).ToArray();
                        return new Model.VacationExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
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
        public Model.VacationExecutionResult VacationGet(long vacationId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ViewVacation);

                        var dbRes = rep.Get<Repository.Model.Vacation>(i => i.VacationId == vacationId, asNoTracking: true);
                        var res = AutoMapper.Mapper.Map<Model.Vacation>(dbRes.SingleOrDefault());

                        return new Model.VacationExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
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
        public Model.BaseExecutionResult VacationRemove(long vacationId) => VacationRemoveRange(new long[] { vacationId });

        /// <summary>
        /// Delete vacations
        /// </summary>
        /// <param name="vacationIds">Identifiers</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult VacationRemoveRange(IEnumerable<long> vacationIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var current = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var res = rep.Get<Repository.Model.Vacation>(e => vacationIds.Contains(e.VacationId)).ToArray();

                        if (!res.All(r => r.EmployeeId == current.Id))
                            SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.ManageVacation);

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
        /// Update single vacation
        /// </summary>
        /// <param name="vacation">Vacation</param>
        /// <returns>Result info</returns>
        public Model.VacationExecutionResult VacationUpdate(Model.Vacation vacation)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var current = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var res = rep.Get<Repository.Model.Vacation>(e => e.VacationId == vacation.Id).SingleOrDefault();
                        if (res == null)
                            throw new KeyNotFoundException();

                        if (res.EmployeeId != current.Id)
                            SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.ManageVacation);

                        var newDbValue = AutoMapper.Mapper.Map<Repository.Model.Vacation>(vacation);
                        res.CopyObjectFrom(newDbValue);

                        rep.SaveChanges();
                        
                        return new Model.VacationExecutionResult(AutoMapper.Mapper.Map<Model.Vacation>(res));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationExecutionResult(ex);
                }
        }

        /// <summary>
        /// Insert single vacation
        /// </summary>
        /// <param name="vacation">Vacation</param>
        /// <returns>Result info</returns>
        public Model.VacationExecutionResult VacationInsert(Model.Vacation vacation)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var current = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);
                        if (vacation.EmployeeId != current.Id)
                            SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.ManageVacation);

                        var newDbValue = AutoMapper.Mapper.Map<Repository.Model.Vacation>(vacation);
                        newDbValue.VacationId = 0;
                        rep.Add(newDbValue);
                        return new Model.VacationExecutionResult(AutoMapper.Mapper.Map<Model.Vacation>(newDbValue));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationExecutionResult(ex);
                }
        }
    }
}
