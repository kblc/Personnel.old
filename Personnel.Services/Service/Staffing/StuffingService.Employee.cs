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
    public partial class StaffingService
    {
        #region Service contract implementation

        public EmployeeExecutionResult EmployeeGetCurrent()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    { 
                        var res = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);
                        return new Model.EmployeeExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    return new Model.EmployeeExecutionResult(ex);
                }
        }

        public EmployeeExecutionResult RESTEmployeeGetCurrent() => EmployeeGetCurrent();

        public EmployeeExecutionResults EmployeeGetRange(IEnumerable<long> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var me = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ViewEmployes);
                        var rightIds = me.Rights.Select(r => r.RightId).ToArray();
                        var rightTypes = rep.Get<Repository.Model.Right>(asNoTracking: true)
                            .Where(r => rightIds.Contains(r.RightId))
                            .ToArray()
                            .Select(r => r.Type);

                        var res = rep.Get<Repository.Model.Employee>(e => identifiers.Contains(e.EmployeeId), asNoTracking: true)
                            .ToArray()
                            .Select(i => AutoMapper.Mapper.Map<Model.Employee>(i, opts => opts.AfterMap(
                                (src, dst) =>
                                {
                                    var dstE = (Model.Employee)dst;
                                    if (!rightTypes.Contains(Repository.Model.RightType.ManageEmployeesRights) && dstE.Id != me.Id)
                                    {
                                        dstE.Rights = null;
                                        dstE.Logins = null;
                                    }
                                })
                            ))
                            .ToArray();
                        return new EmployeeExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString(),","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResults(ex);
                }
        }

        public EmployeeExecutionResults RESTEmployeeGetRange(IEnumerable<string> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(id => LongFromString(id));
                    return EmployeeGetRange(ids);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i=>i,","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResults(ex);
                }
        }

        public EmployeeExecutionResults EmployeesGet()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ViewEmployes);

                        var res = rep.Get<Repository.Model.Employee>(asNoTracking: true, eagerLoad: new string[] { "Logins","Rights","Stuffing" })
                            .ToArray()
                            .Select(i => AutoMapper.Mapper.Map<Model.Employee>(i))
                            .ToArray();
                        return new EmployeeExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResults(ex);
                }
        }

        public EmployeeExecutionResults RESTEmployeesGet() => EmployeesGet();

        public EmployeeExecutionResult EmployeeGet(long employeeId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var emp = EmployeeGetRange(new long[] { employeeId });
                    if (emp.Exception != null)
                        throw emp.Exception;

                    var empFounded = emp.Values.SingleOrDefault();
                    if (empFounded == null)
                        throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_EmployeeNotFound, employeeId));

                    return new EmployeeExecutionResult(empFounded);
                }
                catch(Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResult(ex);
                }
        }

        public EmployeeExecutionResult RESTEmployeeGet(string employeeId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeGet(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResult(ex);
                }
        }

        public BaseExecutionResult EmployeeRemoveRange(IEnumerable<long> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageEmployes);

                        var res = rep.Get<Repository.Model.Employee>(e => identifiers.Contains(e.EmployeeId));
                        rep.RemoveRange(res);

                        return new BaseExecutionResult();
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new BaseExecutionResult(ex);
                }
        }

        public BaseExecutionResult RESTEmployeeRemoveRange(IEnumerable<string> identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(id => LongFromString(id));
                    return EmployeeRemoveRange(ids);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i,","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new BaseExecutionResult(ex);
                }
        }

        public BaseExecutionResult EmployeeRemove(long employeeId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    return EmployeeRemoveRange(new long[] { employeeId });
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResult(ex);
                }
        }

        public BaseExecutionResult RESTEmployeeRemove(string employeeId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeeRemove(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResult(ex);
                }
        }

        public EmployeeExecutionResult EmployeeUpdate(Employee employee)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (employee == null)
                        throw new ArgumentNullException(nameof(employee));

                    using (var rep = GetNewRepository(logSession))
                    {
                        Repository.Model.RightType[] existedRightTypes;
                        var currentEmployee = SRVCGetEmployeeCredentials(logSession, rep, out existedRightTypes);
                        SRVCCheckCredentials(logSession, rep, existedRightTypes, new Repository.Model.RightType[] { Repository.Model.RightType.Login });

                        var res = rep.Get<Repository.Model.Employee>(e => employee.Id == e.EmployeeId, asNoTracking: false)
                            .SingleOrDefault();
                        if (res == null)
                            throw new Exception(string.Format(Properties.Resources.STUFFINGSERVICE_EmployeeNotFound, employee.Id));

                        if (currentEmployee.Id != res.EmployeeId) {
                            SRVCCheckCredentials(logSession, rep, existedRightTypes, new Repository.Model.RightType[] { Repository.Model.RightType.ManageEmployes });
                        }

                        var ignoreFields = new List<string>();
                        ignoreFields.Add(nameof(res.Logins));
                        ignoreFields.Add(nameof(res.Rights));
                        ignoreFields.Add(nameof(res.Photos));
                        ignoreFields.Add(nameof(res.Stuffing));
                        ignoreFields.Add(nameof(res.StuffingId));

                        res.CopyObjectFrom(AutoMapper.Mapper.Map<Repository.Model.Employee>(employee), ignoreFields.ToArray());

                        if (existedRightTypes.Contains(Repository.Model.RightType.ManageStaffing))
                        {
                            var newStaffingId = employee.Stuffing?.Id ?? (long)0;
                            if (newStaffingId > 0)
                            {
                                var staff = rep.Get<Repository.Model.Staffing>(s => s.StaffingId == newStaffingId).SingleOrDefault();
                                res.Stuffing = staff;
                                res.StuffingId = newStaffingId;
                            } else
                            {
                                res.Stuffing = null;
                                res.StuffingId = null;
                            }
                        }

                        rep.SaveChanges();

                        return new EmployeeExecutionResult(AutoMapper.Mapper.Map<Employee>(res));
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employee), employee);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResult(ex);
                }
        }

        public EmployeeExecutionResult RESTEmployeeUpdate(Employee employee) => EmployeeUpdate(employee);

        public EmployeeExecutionResult EmployeeInsert(Employee employee)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (employee == null)
                        throw new ArgumentNullException(nameof(employee));

                    using (var rep = GetNewRepository(logSession))
                    {
                        var me = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageEmployes);
                        var rightIds = me.Rights.Select(r => r.RightId).ToArray();
                        var rightTypes = rep.Get<Repository.Model.Right>(asNoTracking: true)
                            .Where(r => rightIds.Contains(r.RightId))
                            .ToArray()
                            .Select(r => r.Type);

                        var insertingEmployee = AutoMapper.Mapper.Map<Repository.Model.Employee>(employee, opts =>
                        {
                            opts.BeforeMap((srcO, dstO) =>
                            {
                                var src = ((Model.Employee)srcO);
                                var dst = ((Repository.Model.Employee)dstO);

                                src.Id = 0;
                                if (!rightTypes.Contains(Repository.Model.RightType.ManageEmployeesRights))
                                {
                                    src.Rights = null;
                                    src.Logins = null;
                                }
                            });
                            opts.AfterMap((srcO, dstO) =>
                            {
                                var src = ((Model.Employee)srcO);
                                var dst = ((Repository.Model.Employee)dstO);
                                //dst.Rights.ToList().ForEach(r => r.Employee = dst);
                                //dst.Logins.ToList().ForEach(l => l.Employee = dst);
                            });
                        });
                        rep.Add(insertingEmployee);
                        return new EmployeeExecutionResult(AutoMapper.Mapper.Map<Model.Employee>(insertingEmployee));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResult(ex);
                }
        }

        public EmployeeExecutionResult RESTEmployeeInsert(Employee employee) => EmployeeInsert(employee);

        #endregion
    }
}
