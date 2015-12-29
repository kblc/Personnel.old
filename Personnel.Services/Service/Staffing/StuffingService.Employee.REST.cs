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
        public EmployeeExecutionResult RESTEmployeeGetCurrent() => EmployeeGetCurrent();

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
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i, ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeeExecutionResults(ex);
                }
        }

        public EmployeeExecutionResults RESTEmployeesGet() => EmployeesGet();

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
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i, ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new BaseExecutionResult(ex);
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

        public EmployeeExecutionResult RESTEmployeeUpdate(Employee employee) => EmployeeUpdate(employee);

        public EmployeeExecutionResult RESTEmployeeInsert(Employee employee) => EmployeeInsert(employee);
    }
}
