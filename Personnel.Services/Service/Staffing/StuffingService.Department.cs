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
        /// Get information about all department
        /// </summary>
        /// <returns>Result info</returns>
        public Model.DepartmentExecutionResults DepartmentsGet()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var res = rep.Get<Repository.Model.Department>(asNoTracking: true)
                            .ToArray()
                            .Select(i => AutoMapper.Mapper.Map<Model.Department>(i))
                            .ToArray();
                        return new DepartmentExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new DepartmentExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get information about all department
        /// </summary>
        /// <returns>Result info</returns>
        public Model.DepartmentExecutionResults RESTDepartmentsGet() => DepartmentsGet();

        /// <summary>
        /// Get information about specified department identifiers
        /// </summary>
        /// <returns>Result info</returns>
        public Model.DepartmentExecutionResults DepartmentGetRange(IEnumerable<long> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var res = rep.Get<Repository.Model.Department>(e => identifiers.Contains(e.DepartmentId), asNoTracking: true)
                            .ToArray()
                            .Select(i => AutoMapper.Mapper.Map<Model.Department>(i))
                            .ToArray();
                        return new DepartmentExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString(), ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new DepartmentExecutionResults(ex);
                }
        }

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
                catch(Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => "'" + i + "'",","));
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
        public Model.DepartmentExecutionResult DepartmentGet(long departmentId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var rng = DepartmentGetRange(new long[] { departmentId });
                    if (rng.Exception != null)
                        throw rng.Exception;

                    var rngFounded = rng.Values.SingleOrDefault();
                    if (rngFounded == null)
                        throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_DepartmentNotFound, departmentId));

                    return new DepartmentExecutionResult(rngFounded);
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
        public Model.BaseExecutionResult DepartmentRemove(long departmentId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    return DepartmentRemoveRange(new long[] { departmentId });
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
        public Model.BaseExecutionResult DepartmentRemoveRange(IEnumerable<long> departmentIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageDepartments);

                        var res = rep.Get<Repository.Model.Department>(e => departmentIds.Contains(e.DepartmentId));
                        rep.RemoveRange(res);

                        return new BaseExecutionResult();
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(departmentIds), departmentIds.Concat(i => i.ToString(), ","));
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
                    ex.Data.Add(nameof(departmentIds), departmentIds.Concat(i => "'" + i + "'", ","));
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
        public Model.DepartmentExecutionResult DepartmentUpdate(Model.Department department)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (department == null)
                        throw new ArgumentNullException(nameof(department));

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageDepartments);

                        var res = rep.Get<Repository.Model.Department>(e => department.Id == e.DepartmentId).SingleOrDefault();
                        if (res == null)
                            throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_DepartmentNotFound, department.Id));

                        var updating = AutoMapper.Mapper.Map<Repository.Model.Department>(department);
                        rep.AddOrUpdate(updating, takeChilds: false, original: res);
                    }

                    return DepartmentGet(department.Id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(department), department);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new DepartmentExecutionResult(ex);
                }
        }

        // <summary>
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
        public Model.DepartmentExecutionResult DepartmentInsert(Model.Department department)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (department == null)
                        throw new ArgumentNullException(nameof(department));

                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageDepartments);

                        var inserting = AutoMapper.Mapper.Map<Repository.Model.Department>(department);
                        var res = rep.New<Repository.Model.Department>((a) =>
                        {
                            var copyErrors = inserting.CopyObjectTo(a, new string[] { nameof(inserting.DepartmentId) });
                        });
                        rep.Add(res);
                        return new DepartmentExecutionResult(AutoMapper.Mapper.Map<Model.Department>(res));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new DepartmentExecutionResult(ex);
                }
        }

        /// <summary>
        /// Insert single department
        /// </summary>
        /// <param name="department">Department</param>
        /// <returns>Result info</returns>
        public Model.DepartmentExecutionResult RESTDepartmentInsert(Model.Department department) => DepartmentInsert(department);
    }
}
