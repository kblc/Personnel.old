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
        /// Get information about appoint
        /// </summary>
        /// <param name="appointId">Identifier</param>
        /// <returns>Result info</returns>
        public AppointExecutionResults AppointsGet()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var res = rep.Get<Repository.Model.Appoint>(asNoTracking: true)
                            .ToArray()
                            .Select(i => AutoMapper.Mapper.Map<Model.Appoint>(i))
                            .ToArray();
                        return new AppointExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AppointExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get information about appoint
        /// </summary>
        /// <param name="appointId">Identifier</param>
        /// <returns>Result info</returns>
        public AppointExecutionResults RESTAppointsGet() => AppointsGet();

        /// <summary>
        /// Get information about specified appoint identifiers
        /// </summary>
        /// <returns>Result info</returns>
        public AppointExecutionResults AppointGetRange(IEnumerable<long> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var res = rep.Get<Repository.Model.Appoint>(e => identifiers.Contains(e.AppointId), asNoTracking: true)
                            .ToArray()
                            .Select(i => AutoMapper.Mapper.Map<Model.Appoint>(i))
                            .ToArray();
                        return new AppointExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString(),","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AppointExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get information about specified appoint identifiers
        /// </summary>
        /// <returns>Result info</returns>
        public AppointExecutionResults RESTAppointGetRange(IEnumerable<string> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(i => LongFromString(i));
                    return AppointGetRange(ids.ToArray());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => "'" + i + "'", ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AppointExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get information about appoint
        /// </summary>
        /// <param name="appointId">Identifier</param>
        /// <returns>Result info</returns>
        public AppointExecutionResult AppointGet(long appointId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var app = AppointGetRange(new long[] { appointId });
                    if (app.Exception != null)
                        throw app.Exception;

                    var appFounded = app.Values.SingleOrDefault();
                    if (appFounded == null)
                        throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_AppointNotFound, appointId));

                    return new AppointExecutionResult(appFounded);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(appointId), appointId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AppointExecutionResult(ex);
                }
        }

        /// <summary>
        /// Get information about appoint
        /// </summary>
        /// <param name="appointId">Identifier</param>
        /// <returns>Result info</returns>
        public AppointExecutionResult RESTAppointGet(string appointId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(appointId);
                    return AppointGet(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(appointId), appointId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AppointExecutionResult(ex);
                }
        }

        /// <summary>
        /// Delete appoints
        /// </summary>
        /// <param name="identifiers">Identifiers</param>
        /// <returns>Result info</returns>
        public BaseExecutionResult AppointRemoveRange(IEnumerable<long> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageStaffing);

                        var res = rep.Get<Repository.Model.Appoint>(e => identifiers.Contains(e.AppointId));
                        rep.RemoveRange(res);

                        return new BaseExecutionResult();
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString(),","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new BaseExecutionResult(ex);
                }
        }

        /// <summary>
        /// Delete appoints
        /// </summary>
        /// <param name="identifiers">Identifiers</param>
        /// <returns>Result info</returns>
        public BaseExecutionResult RESTAppointRemoveRange(IEnumerable<string> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(i => LongFromString(i));
                    return AppointRemoveRange(ids.ToArray());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => "'" + i + "'", ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AppointExecutionResults(ex);
                }
        }

        /// <summary>
        /// Delete single appoint
        /// </summary>
        /// <param name="appointId">Identifier</param>
        /// <returns>Result info</returns>
        public BaseExecutionResult AppointRemove(long appointId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    return AppointRemoveRange(new long[] { appointId });
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(appointId), appointId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResult(ex);
                }
        }

        /// <summary>
        /// Delete single appoint
        /// </summary>
        /// <param name="appointId">Identifier</param>
        /// <returns>Result info</returns>
        public BaseExecutionResult RESTAppointRemove(string appointId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(appointId);
                    return AppointRemove(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(appointId), appointId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AppointExecutionResult(ex);
                }
        }

        /// <summary>
        /// Update single appoint
        /// </summary>
        /// <param name="appoint">Appoint</param>
        /// <returns>Result info</returns>
        public AppointExecutionResult AppointUpdate(Appoint appoint)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (appoint == null)
                        throw new ArgumentNullException(nameof(appoint));

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageStaffing);

                        var res = rep.Get<Repository.Model.Appoint>(e => appoint.Id == e.AppointId, asNoTracking: true)
                            .SingleOrDefault();
                        if (res == null)
                            throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_AppointNotFound, appoint.Id));

                        var updating = AutoMapper.Mapper.Map<Repository.Model.Appoint>(appoint);
                        rep.AddOrUpdate(updating);
                    }
                    return AppointGet(appoint.Id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(appoint), appoint);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AppointExecutionResult(ex);
                }
        }

        /// <summary>
        /// Update single appoint
        /// </summary>
        /// <param name="appoint">Appoint</param>
        /// <returns>Result info</returns>
        public AppointExecutionResult RESTAppointUpdate(Appoint appoint) => AppointUpdate(appoint);

        /// <summary>
        /// Insert single appoint
        /// </summary>
        /// <param name="appoint">Appoint</param>
        /// <returns>Result info</returns>
        public AppointExecutionResult AppointInsert(Appoint appoint)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (appoint == null)
                        throw new ArgumentNullException(nameof(appoint));

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageStaffing);
                        var insertingAppoint = AutoMapper.Mapper.Map<Repository.Model.Appoint>(appoint);
                        var res = rep.New<Repository.Model.Appoint>((a) => 
                        {
                            insertingAppoint.CopyObject(a, new string[] { nameof(insertingAppoint.AppointId) });
                        });
                        rep.Add(res);
                        rep.SaveChanges();
                        return new AppointExecutionResult(AutoMapper.Mapper.Map<Model.Appoint>(res));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AppointExecutionResult(ex);
                }
        }

        /// <summary>
        /// Insert single appoint
        /// </summary>
        /// <param name="appoint">Appoint</param>
        /// <returns>Result info</returns>
        public AppointExecutionResult RESTAppointInsert(Appoint appoint) => AppointInsert(appoint);
    }
}
