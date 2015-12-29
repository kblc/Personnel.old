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
        /// Get all rights
        /// </summary>
        /// <returns>Rights</returns>
        public Model.RightExecutionResults RESTRightsGet() => RightsGet();

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
                    ex.Data.Add(nameof(rightIds), rightIds.Concat(r => r.ToString(), ","));
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
                    ex.Data.Add(nameof(rightIds), rightIds.Concat(i => i.ToString(), ","));
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
