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
    public partial class StaffingService : IStaffingServiceREST
    {
        /// <summary>
        /// Get information about all staffing
        /// </summary>
        /// <returns>Result info</returns>
        public Model.StaffingExecutionResults RESTStaffingsGet() => StaffingsGet();

        /// <summary>
        /// Get information about specified staffing identifiers
        /// </summary>
        /// <returns>Result info</returns>
        public Model.StaffingExecutionResults RESTStaffingGetRange(IEnumerable<string> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(i => LongFromString(i));
                    return StaffingGetRange(ids.ToArray());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => "'" + i + "'", ","));
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
        public Model.StaffingExecutionResult RESTStaffingGet(string staffingId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(staffingId);
                    return StaffingGet(id);
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
        public Model.BaseExecutionResult RESTStaffingRemove(string staffingId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(staffingId);
                    return StaffingRemove(id);
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
        public Model.BaseExecutionResult RESTStaffingRemoveRange(IEnumerable<string> staffingIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = staffingIds.Select(i => LongFromString(i));
                    return StaffingRemoveRange(ids.ToArray());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(staffingIds), staffingIds.Concat(i => "'" + i + "'", ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new StaffingExecutionResults(ex);
                }
        }

        /// <summary>
        /// Update single staffing
        /// </summary>
        /// <param name="staffing">Staffing</param>
        /// <returns>Result info</returns>
        public Model.StaffingExecutionResult RESTStaffingUpdate(Model.Staffing staffing) => StaffingUpdate(staffing);

        /// <summary>
        /// Insert single staffing
        /// </summary>
        /// <param name="staffing">Staffing</param>
        /// <returns>Result info</returns>
        public Model.StaffingExecutionResult RESTStaffingInsert(Model.Staffing staffing) => StaffingInsert(staffing);
    }
}
