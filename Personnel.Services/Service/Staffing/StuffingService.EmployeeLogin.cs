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
        private IEnumerable<EmployeeLogin> SRVCUpdateLogins(Helpers.Log.SessionInfo logSession, Repository.Logic.Repository rep,
            long employeeId, IEnumerable<string> addLogins, IEnumerable<string> removeLogins)
        {
#pragma warning disable 618
            logSession.Add($"Try to get employee with id = {employeeId}");
            var emp = rep.Get<Repository.Model.Employee>(e => e.EmployeeId == employeeId, false, new string[] { "Logins" }).FirstOrDefault();
            if (emp == null)
                throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_EmployeeNotFound, employeeId));

            var existedLogins = emp.Logins.Select(r => r.DomainLogin);

            #region Add logins

            if (addLogins != null && addLogins.Any())
            {
                logSession.Add($"Add logins...");
                var addLoginsUpper = addLogins
                    .Except(existedLogins)
                    .ToArray()
                    .Select(r => rep.New<Repository.Model.EmployeeLogin>((er) =>
                    {
                        er.EmployeeLoginId = emp.EmployeeId;
                        er.DomainLogin = r;
                    }))
                    .ToArray();

                logSession.Add($"Add this logins {addLoginsUpper.Concat(r => r.DomainLogin, ",")} for employee id = {employeeId}");
                foreach (var r in addLoginsUpper)
                    emp.Logins.Add(r);

                rep.AddRange(addLoginsUpper, saveAfterInsert: false);
            }

            #endregion
            #region Remove rights

            if (removeLogins != null && removeLogins.Any())
            {
                logSession.Add($"Remove logins...");
                var removeLoginsUpper = removeLogins
                    .Intersect(existedLogins)
                    .ToArray()
                    .Join(emp.Logins, r => r, er => er.DomainLogin.ToUpper(), (r, er) => er)
                    .ToArray();

                logSession.Add($"Remove this logins {removeLoginsUpper.Concat(r => r.DomainLogin, ",")} for employee id = {employeeId}");
                foreach (var r in removeLoginsUpper)
                    emp.Logins.Remove(r);

                rep.RemoveRange(removeLoginsUpper, saveAfterRemove: false);
            }

            #endregion

            rep.SaveChanges();

            return emp.Logins.Select(er => AutoMapper.Mapper.Map<EmployeeLogin>(er));
#pragma warning restore 618
        }


        /// <summary>
        /// Add login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="logins">Logins to add</param>
        public Model.LoginValueExecutionResults EmployeeLoginsAdd(long employeeId, string[] logins)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (logins == null)
                        throw new ArgumentNullException(nameof(logins));

                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageEmployeesLogins);
                        var res = SRVCUpdateLogins(logSession, rep, emp.Value.Id, logins, null).ToArray();
                        return new LoginValueExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(logins), logins?.Concat(e => e.ToString(), ",") ?? "NULL");
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LoginValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Add login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="logins">Logins to add</param>
        public Model.LoginValueExecutionResults RESTEmployeeLoginsAdd(string employeeId, string[] logins)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeLoginsAdd(id, logins);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(logins), logins?.Concat(e => e.ToString(), ",") ?? "NULL");
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LoginValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Add login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="login">Login to add</param>
        public Model.LoginValueExecutionResults EmployeeLoginsAddOne(long employeeId, string login) 
            => EmployeeLoginsAdd(employeeId, new string[] { login });

        /// <summary>
        /// Add login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="login">Login to add</param>
        public Model.LoginValueExecutionResults RESTEmployeeLoginsAddOne(string employeeId, string login)
            => RESTEmployeeLoginsAdd(employeeId, new[] { login });

        /// <summary>
        /// Remove login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="identifiers">Logins to delete</param>
        public Model.LoginValueExecutionResults EmployeeLoginsRemove(long employeeId, string[] logins)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (logins == null)
                        throw new ArgumentNullException(nameof(logins));

                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageEmployeesLogins);
                        var loginsToDeleteUpper = logins.Select(l => l.ToUpper()).ToArray();
                        var loginsToDelete = rep.Get<Repository.Model.EmployeeLogin>(r => r.EmployeeLoginId == employeeId && loginsToDeleteUpper.Contains(r.DomainLogin.ToUpper()), asNoTracking: true)
                            .Select(r => r.DomainLogin);
                        var res = SRVCUpdateLogins(logSession, rep, emp.Value.Id, null, loginsToDelete).ToArray();
                        return new LoginValueExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(logins), logins?.Concat(e => e.ToString(), ",") ?? "NULL");
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LoginValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Remove login to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="logins">Logins to delete</param>
        public Model.LoginValueExecutionResults RESTEmployeeLoginsRemove(string employeeId, string[] logins)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeLoginsRemove(id, logins);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(logins), logins?.Concat(e => e.ToString(), ",") ?? "NULL");
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LoginValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Remove login from employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="login">Login to delete</param>
        public Model.LoginValueExecutionResults EmployeeLoginsRemoveOne(long employeeId, string login)
            => EmployeeLoginsRemove(employeeId, new[] { login });

        /// <summary>
        /// Remove login from employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="login">Login to remove</param>
        public Model.LoginValueExecutionResults RESTEmployeeLoginsRemoveOne(string employeeId, string login)
            => RESTEmployeeLoginsRemove(employeeId, new[] { login });

        /// <summary>
        /// Get Logins for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        public Model.LoginValueExecutionResults EmployeeLoginsGet(long employeeId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;
                    return new LoginValueExecutionResults(emp.Value.Logins.ToArray());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LoginValueExecutionResults(ex);
                }
        }
        
        /// <summary>
        /// Get Logins for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        public Model.LoginValueExecutionResults RESTEmployeeLoginsGet(string employeeId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeLoginsGet(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LoginValueExecutionResults(ex);
                }
        }

        /// <summary>
        /// Update employee Logins
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="logins">System login names</param>
        public Model.LoginValueExecutionResults EmployeeLoginsUpdate(long employeeId, string[] logins)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (logins == null)
                        throw new ArgumentNullException(nameof(logins));

                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageEmployeesLogins);

                        var delLogins = emp.Value.Logins.Select(r => r.Login).Except(logins);
                        var addLogins = logins.Except(emp.Value.Logins.Select(r => r.Login));

                        var res = SRVCUpdateLogins(logSession, rep, emp.Value.Id, addLogins, delLogins).ToArray();

                        return new LoginValueExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(logins), logins?.Concat(e => e.ToString(), ",") ?? "NULL");
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LoginValueExecutionResults(ex);
                }
        }
        
        /// <summary>
        /// Update employee Logins
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="logins">System login names</param>
        public Model.LoginValueExecutionResults RESTEmployeeLoginsUpdate(string employeeId, string[] logins)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeLoginsUpdate(id, logins);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(logins), logins?.Concat(e => e.ToString(), ",") ?? "NULL");
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LoginValueExecutionResults(ex);
                }
        }
    }
}
