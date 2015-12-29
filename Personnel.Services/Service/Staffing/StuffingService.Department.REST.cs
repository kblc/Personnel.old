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

    // REST implementation
    public partial class StaffingService : IStaffingServiceREST
    {
        /// <summary>
        /// Get information about all department
        /// </summary>
        /// <returns>Result info</returns>
        public Model.DepartmentExecutionResults RESTDepartmentsGet() => DepartmentsGet();

        /// <summary>
        /// Get information about specified department identifiers
        /// </summary>
        /// <returns>Result info</returns>
        public Model.DepartmentExecutionResults RESTDepartmentGetRange(IEnumerable<string> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(i => LongFromString(i));
                    return DepartmentGetRange(ids.ToArray());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => $"'{i}'", ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new DepartmentExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get information about department
        /// </summary>
        /// <param name="departmentId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.DepartmentExecutionResult RESTDepartmentGet(string departmentId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(departmentId);
                    return DepartmentGet(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(departmentId), departmentId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new DepartmentExecutionResult(ex);
                }
        }

        /// <summary>
        /// Delete single department
        /// </summary>
        /// <param name="departmentId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult RESTDepartmentRemove(string departmentId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(departmentId);
                    return DepartmentRemove(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(departmentId), departmentId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new BaseExecutionResult(ex);
                }
        }

        /// <summary>
        /// Delete departments
        /// </summary>
        /// <param name="departmentIds">Identifiers</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult RESTDepartmentRemoveRange(IEnumerable<string> departmentIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = departmentIds.Select(i => LongFromString(i));
                    return DepartmentRemoveRange(ids.ToArray());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(departmentIds), departmentIds.Concat(i => $"'{i}'", ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new DepartmentExecutionResults(ex);
                }
        }

        /// <summary>
        /// Update single department
        /// </summary>
        /// <param name="department">Department</param>
        /// <returns>Result info</returns>
        public Model.DepartmentExecutionResult RESTDepartmentUpdate(Model.Department department) => DepartmentUpdate(department);


        /// <summary>
        /// Insert single department
        /// </summary>
        /// <param name="department">Department</param>
        /// <returns>Result info</returns>
        public Model.DepartmentExecutionResult RESTDepartmentInsert(Model.Department department) => DepartmentInsert(department);
    }
}
