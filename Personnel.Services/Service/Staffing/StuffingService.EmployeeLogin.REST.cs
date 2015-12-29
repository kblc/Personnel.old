using Helpers.Linq;
using Personnel.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Staffing
{
    public partial class StaffingService : IStaffingServiceREST
    {
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
        public Model.LoginValueExecutionResults RESTEmployeeLoginsAddOne(string employeeId, string login)
            => RESTEmployeeLoginsAdd(employeeId, new[] { login });

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
        /// <param name="login">Login to remove</param>
        public Model.LoginValueExecutionResults RESTEmployeeLoginsRemoveOne(string employeeId, string login)
            => RESTEmployeeLoginsRemove(employeeId, new[] { login });

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
