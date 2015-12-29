using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Personnel.Services.Model;
using Helpers;
using Helpers.Linq;

namespace Personnel.Services.Service.Staffing
{
    public partial class StaffingService : IStaffingService
    {
        /// <summary>
        /// Get information about all staffing
        /// </summary>
        /// <returns>Result info</returns>
        public Model.StaffingExecutionResults StaffingsGet()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var res = rep.Get<Repository.Model.Staffing>(asNoTracking: true)
                            .ToArray()
                            .Select(i => AutoMapper.Mapper.Map<Model.Staffing>(i))
                            .ToArray();
                        return new StaffingExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new StaffingExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get information about specified staffing identifiers
        /// </summary>
        /// <returns>Result info</returns>
        public Model.StaffingExecutionResults StaffingGetRange(IEnumerable<long> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var res = rep.Get<Repository.Model.Staffing>(e => identifiers.Contains(e.StaffingId), asNoTracking: true)
                            .ToArray()
                            .Select(i => AutoMapper.Mapper.Map<Model.Staffing>(i))
                            .ToArray();
                        return new StaffingExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString(), ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new StaffingExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get information about staffing
        /// </summary>
        /// <param name="staffingId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.StaffingExecutionResult StaffingGet(long staffingId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var rng = StaffingGetRange(new long[] { staffingId });
                    if (rng.Exception != null)
                        throw rng.Exception;

                    var rngFounded = rng.Values.SingleOrDefault();
                    if (rngFounded == null)
                        throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_StaffingNotFound, staffingId));

                    return new StaffingExecutionResult(rngFounded);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(staffingId), staffingId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new StaffingExecutionResult(ex);
                }
        }

        /// <summary>
        /// Delete single staffing
        /// </summary>
        /// <param name="staffingId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult StaffingRemove(long staffingId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    return StaffingRemoveRange(new long[] { staffingId });
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(staffingId), staffingId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new BaseExecutionResult(ex);
                }
        }

        /// <summary>
        /// Delete staffings
        /// </summary>
        /// <param name="staffingIds">Identifiers</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult StaffingRemoveRange(IEnumerable<long> staffingIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageStaffing);

                        var res = rep.Get<Repository.Model.Staffing>((System.Linq.Expressions.Expression<Func<Repository.Model.Staffing, bool>>)(e => staffingIds.Contains(e.StaffingId)));
                        rep.RemoveRange(res);

                        return new BaseExecutionResult();
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(staffingIds), staffingIds.Concat(i => i.ToString(), ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new BaseExecutionResult(ex);
                }
        }

        /// <summary>
        /// Update single staffing
        /// </summary>
        /// <param name="staffing">Staffing</param>
        /// <returns>Result info</returns>
        public Model.StaffingExecutionResult StaffingUpdate(Model.Staffing staffing)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (staffing == null)
                        throw new ArgumentNullException(nameof(staffing));

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageStaffing);

                        var res = rep.Get<Repository.Model.Staffing>(e => staffing.Id == e.StaffingId).SingleOrDefault();
                        if (res == null)
                            throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_StaffingNotFound, staffing.Id));

                        var updating = AutoMapper.Mapper.Map<Repository.Model.Staffing>(staffing);
                        rep.AddOrUpdate(updating, takeChilds: false, original: res);
                    }

                    return StaffingGet(staffing.Id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(staffing), staffing);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new StaffingExecutionResult(ex);
                }
        }

        /// <summary>
        /// Insert single staffing
        /// </summary>
        /// <param name="staffing">Staffing</param>
        /// <returns>Result info</returns>
        public Model.StaffingExecutionResult StaffingInsert(Model.Staffing staffing)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (staffing == null)
                        throw new ArgumentNullException(nameof(staffing));

                    var inserting = AutoMapper.Mapper.Map<Repository.Model.Staffing>(staffing);

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageStaffing);
                        var res = rep.New<Repository.Model.Staffing>((Action<Repository.Model.Staffing>)((a) =>
                        {
                            inserting.CopyObject(a, new string[] { nameof(inserting.StaffingId) });
                        }));
                        rep.Add(res);
                        rep.SaveChanges();
                        return new StaffingExecutionResult(AutoMapper.Mapper.Map<Model.Staffing>(res));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new StaffingExecutionResult(ex);
                }
        }
    }
}
