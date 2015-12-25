using Helpers.Linq;
using Personnel.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Staffing
{
    public partial class StaffingService
    {
        private void CheckBadRights(Helpers.Log.SessionInfo logSession, Repository.Logic.Repository rep, IEnumerable<long> rights)
        {
            logSession.Add($"Check rights data...");
#pragma warning disable 618
            var availableRights = rep.Get<Repository.Model.Right>(asNoTracking: true).ToArray();
            var badRights = rights
                .Distinct()
                .LeftOuterJoin(availableRights, r => r, r => r.RightId, (rName, r) => new { rName, r })
                .Where(r => r.r == null)
                .Concat(r => r.r.SystemName, ", ");
#pragma warning restore 618

            if (!string.IsNullOrWhiteSpace(badRights))
                throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_RightNotFound, badRights));
        }

        private IEnumerable<EmployeeRight> SRVCUpdateRights(Helpers.Log.SessionInfo logSession, Repository.Logic.Repository rep,
            long employeeId, IEnumerable<long> addRights, IEnumerable<long> removeRights, bool checkRights = true)
        {
            if (checkRights)
                CheckBadRights(logSession, rep, Enumerable.Empty<long>()
                    .Union(addRights ?? Enumerable.Empty<long>())
                    .Union(removeRights ?? Enumerable.Empty<long>())
                    );

            logSession.Add($"Get available rights from database");
            var availableRights = rep.Get<Repository.Model.Right>(asNoTracking: true).ToArray();

#pragma warning disable 618
            logSession.Add($"Try to get employee with id = {employeeId}");
            var emp = rep.Get<Repository.Model.Employee>(e => e.EmployeeId == employeeId, false, new string[] { "Rights" }).FirstOrDefault();
            if (emp == null)
                throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_EmployeeNotFound, employeeId));

            var existedRights = emp.Rights.Select(r => r.RightId);

            #region Add rights

            if (addRights != null && addRights.Any())
            {
                logSession.Add($"Add rights...");
                var addRightsUpper = addRights
                    .Except(existedRights)
                    .Join(availableRights, r => r, ar => ar.RightId, (r, ar) => ar)
                    .ToArray()
                    .Select(r => rep.New<Repository.Model.EmployeeRight>((er) =>
                    {
                        er.EmployeeId = emp.EmployeeId;
                        er.RightId = r.RightId;
                        //er.Employee = emp;
                        //er.Right = r;
                    })).ToArray();

                logSession.Add($"Add this rights {addRightsUpper.Concat(r => r.RightId.ToString(), ",")} for employee id = {employeeId}");
                foreach (var r in addRightsUpper)
                    emp.Rights.Add(r);

                rep.AddRange(addRightsUpper, saveAfterInsert: false);
            }

            #endregion
            #region Remove rights

            if (removeRights != null && removeRights.Any())
            {
                logSession.Add($"Remove rights...");
                var removeRightsUpper = removeRights
                    .Intersect(existedRights)
                    .Join(availableRights, r => r, ar => ar.RightId, (r, ar) => ar)
                    .ToArray()
                    .Join(emp.Rights, r => r.RightId, er => er.RightId, (r, er) => er)
                    .ToArray();

                logSession.Add($"Remove this rights {removeRightsUpper.Concat(r => r.RightId.ToString(), ",")} for employee id = {employeeId}");
                foreach (var r in removeRightsUpper)
                    emp.Rights.Remove(r);

                rep.RemoveRange(removeRightsUpper, saveAfterRemove: false);
            }

            #endregion

            rep.SaveChanges();

            return emp.Rights.Select(r => AutoMapper.Mapper.Map<EmployeeRight>(r));
#pragma warning restore 618
        }

        /// <summary>
        /// Get all rights
        /// </summary>
        /// <returns>Rights</returns>
        public Model.RightExecutionResults RightsGet()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var res = rep.Get<Repository.Model.Right>(asNoTracking: true)
                            .ToList()
                            .Select(r => AutoMapper.Mapper.Map<Repository.Model.Right, Model.Right>(r))
                            .ToArray();
                        return new RightExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get all rights
        /// </summary>
        /// <returns>Rights</returns>
        public Model.RightExecutionResults RESTRightsGet() => RightsGet();

        /// <summary>
        /// Add right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">System right identifiers</param>
        public Model.RightValueExecutionResults EmployeeRightsAdd(long employeeId, long[] rightIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageEmployeesRights);
                        var rights = rep.Get<Repository.Model.Right>(r => rightIds.Contains(r.RightId), asNoTracking: true)
                            .Select(r => r.RightId);

                        var res = SRVCUpdateRights(logSession, rep, emp.Value.Id, rights, null).ToArray();
                        return new RightValueExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(rightIds), rightIds.Concat(i=>i.ToString(),","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Add right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">System right identifiers</param>
        public Model.RightValueExecutionResults RESTEmployeeRightsAdd(string employeeId, long[] rightIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeRightsAdd(id, rightIds);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(rightIds), rightIds.Concat(r => r.ToString(),","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Add right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightName">System right name</param>
        public Model.RightValueExecutionResults EmployeeRightsAddByName(long employeeId, string rightName)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
#pragma warning disable 618
                        var rights = rep.Get<Repository.Model.Right>(asNoTracking: true)
                            .Where(r => string.Compare(r.SystemName, rightName, true) == 0)
                            .Select(r => r.RightId)
                            .ToArray();
#pragma warning restore 618
                        return EmployeeRightsAdd(employeeId, rights);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(rightName), rightName);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }
        /// <summary>
        /// Add right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightName">System right name</param>
        public Model.RightValueExecutionResults RESTEmployeeRightsAddByName(string employeeId, string rightName)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeRightsAddByName(id, rightName);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(rightName), rightName);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Remove right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">System right name</param>
        public Model.RightValueExecutionResults EmployeeRightsRemove(long employeeId, long[] rightIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageEmployeesRights);
                        var rights = rep.Get<Repository.Model.Right>(r => rightIds.Contains(r.RightId), asNoTracking: true)
                            .Select(r => r.RightId);
                        var res = SRVCUpdateRights(logSession, rep, emp.Value.Id, null, rights).ToArray();
                        return new RightValueExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(rightIds), rightIds);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }
        /// <summary>
        /// Remove right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">System right name</param>
        public Model.RightValueExecutionResults RESTEmployeeRightsRemove(string employeeId, long[] rightIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeRightsRemove(id, rightIds);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(rightIds), rightIds.Concat(i=>i.ToString(),","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Add right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightName">System right name</param>
        public Model.RightValueExecutionResults EmployeeRightsRemoveByName(long employeeId, string rightName)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
#pragma warning disable 618
                        var rights = rep.Get<Repository.Model.Right>(asNoTracking: true)
                            .Where(r => string.Compare(r.SystemName, rightName, true) == 0)
                            .Select(r => r.RightId)
                            .ToArray();
#pragma warning restore 618
                        return EmployeeRightsRemove(employeeId, rights);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(rightName), rightName);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }
        /// <summary>
        /// Add right to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightName">System right name</param>
        public Model.RightValueExecutionResults RESTEmployeeRightsRemoveByName(string employeeId, string rightName)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeRightsRemoveByName(id, rightName);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(rightName), rightName);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get rights for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        public Model.RightValueExecutionResults EmployeeRightsGet(long employeeId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;
                    return new RightValueExecutionResults(emp.Value.Rights.ToArray());
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }
        /// <summary>
        /// Get rights for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        public Model.RightValueExecutionResults RESTEmployeeRightsGet(string employeeId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeRightsGet(id);
                }
                catch (Exception ex)
                {
                    if (!ex.Data.Contains(nameof(employeeId)))
                        ex.Data.Add(nameof(employeeId), employeeId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Update employee rights
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">System right names</param>
        public Model.RightValueExecutionResults EmployeeRightsUpdate(long employeeId, long[] rightIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (rightIds == null)
                        throw new ArgumentNullException(nameof(rightIds));

                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageEmployeesRights);
                        CheckBadRights(logSession, rep, rightIds);

                        var delRights = emp.Value.Rights.Select(r => r.RightId).Except(rightIds);
                        var addRights = rightIds.Except(emp.Value.Rights.Select(r => r.RightId));

                        var res = SRVCUpdateRights(logSession, rep, emp.Value.Id, addRights, delRights, false).ToArray();

                        return new RightValueExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Data.Contains(nameof(employeeId)))
                        ex.Data.Add(nameof(employeeId), employeeId);
                    if (!ex.Data.Contains(nameof(rightIds)))
                        ex.Data.Add(nameof(rightIds), rightIds?.Concat(e => e.ToString(), ",") ?? "NULL");
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }
        /// <summary>
        /// Update employee rights
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="rightIds">System right names</param>
        public Model.RightValueExecutionResults RESTEmployeeRightsUpdate(string employeeId, long[] rightIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeRightsUpdate(id, rightIds);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(rightIds), rightIds?.Concat(e => e.ToString(), ",") ?? "NULL");
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new RightValueExecutionResults(ex);
                }
        }
    }
}
