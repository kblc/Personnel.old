using Helpers.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Base
{
    public abstract class BaseService
    {
        static BaseService()
        {
            Model.ModelMapper.ModelMapperInit();
        }

        protected virtual Repository.Logic.Repository GetNewRepository(Helpers.Log.SessionInfo logSession)
        {
            var rep = new Repository.Logic.Repository();
            rep.SqlLog += (s, e) => RaiseSqlLog(e);
            rep.Log += (s, e) => logSession.Add(e, "[REPOSITORY]");
            return rep;
        }

        public bool VerboseLog { get; set; } = true;
        public CultureInfo Culture { get; set; } = Thread.CurrentThread.CurrentUICulture;

        protected void UpdateSessionCulture()
        {
            var cultureForSession = Culture;
            Thread.CurrentThread.CurrentCulture = cultureForSession;
            Thread.CurrentThread.CurrentUICulture = cultureForSession;
        }

        protected long LongFromString(string id)
        {
            var res = TryLongFromString(id);
            if (res.HasValue)
                return res.Value;
            throw new ArgumentException(string.Format(Properties.Resources.BASESERVICE_BadIdentifierFormat, id ?? "NULL"));
        }
        protected long? TryLongFromString(string id)
        {
            long res;
            if (!string.IsNullOrWhiteSpace(id) && long.TryParse(id, out res))
                return res;
            return null;
        }

        protected Guid GuidFromString(string id)
        {
            var res = TryGuidFromString(id);
            if (res.HasValue)
                return res.Value;
            throw new ArgumentException(string.Format(Properties.Resources.BASESERVICE_BadIdentifierFormat, id ?? "NULL"));
        }
        protected Guid? TryGuidFromString(string id)
        {
            Guid res;
            if (!string.IsNullOrWhiteSpace(id) && (Guid.TryParse(id, out res) || Guid.TryParseExact(id, "N", out res)))
                return res;
            return null;
        }

        protected Model.Employee SRVCCheckCredentials(Helpers.Log.SessionInfo logSession, Repository.Logic.Repository rep, params Repository.Model.RightType[] rightTypes)
        {
            var employee = SRVCGetCurrentEmployee(logSession, rep);
            var currentRightIds = employee.Rights.Select(r => r.RightId);
            var mustExists = rep.Get<Repository.Model.Right>(asNoTracking: true)
                .ToArray()
                .Where(r => rightTypes.Contains(r.Type) && !currentRightIds.Contains(r.RightId))
                .Select(r => r.Name);

            if (mustExists.Any())
                throw new System.Security.SecurityException(string.Format(Properties.Resources.STUFFINGSERVICE_UserHaveNoFollowingRights, mustExists.Concat(i => i.ToString().ToUpper(), ", ")));

            return employee;
        }

        protected Model.Employee SRVCGetCurrentEmployee(Helpers.Log.SessionInfo logSession, Repository.Logic.Repository rep)
        {
            var currentIdentityName =
                ServiceSecurityContext.Current.WindowsIdentity?.Name
                ?? ServiceSecurityContext.Current.PrimaryIdentity?.Name
                ?? nameof(ServiceSecurityContext.Current.IsAnonymous);

            if (!string.IsNullOrWhiteSpace(currentIdentityName))
            {
                var headers = OperationContext.Current.RequestContext.RequestMessage.Headers;

                var employee = rep.Get<Repository.Model.Employee>(asNoTracking: true)
                        .Join(rep.Get<Repository.Model.EmployeeLogin>(asNoTracking: true), e => e.EmployeeId, el => el.EmployeeId, (e, el) => new { Employee = e, el.DomainLogin })
                        .Where(i => string.Compare(i.DomainLogin, currentIdentityName, true) == 0)
                        .Select(i => i.Employee)
                        .FirstOrDefault();

                if (employee != null)
                    //return AutoMapper.Mapper.Map(employee,typeof(Repository.Model.Employee), typeof(Model.Employee)) as Model.Employee;
                    return AutoMapper.Mapper.Map<Repository.Model.Employee, Model.Employee>(employee);
            }

            var ex = new System.Security.SecurityException(string.Format(Properties.Resources.STUFFINGSERVICE_UserNotIdentified, currentIdentityName ?? "NULL"));
            ex.Data.Add(nameof(currentIdentityName), currentIdentityName);
            throw ex;
        }

        /// <summary>
        /// Set session lang
        /// </summary>
        /// <param name="codename">New session codename</param>
        public void ChangeLanguage(string codename)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    Culture = CultureInfo.GetCultureInfo(codename);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(codename), codename);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                }
        }

        #region Log events

        public event EventHandler<string> SqlLog;
        public event EventHandler<string> Log;

        protected void RaiseSqlLog(string logMessage)
        {
            SqlLog?.Invoke(this, logMessage);
            StaticSqlLog?.Invoke(this, logMessage);
        }
        protected void RaiseLog(string logMessage)
        {
            Log?.Invoke(this, logMessage);
            StaticLog?.Invoke(this, logMessage);
        }
        protected void RaiseLog(IEnumerable<string> logMessages)
        {
            foreach (var s in logMessages)
                RaiseLog(s);
        }

        public static event EventHandler<string> StaticSqlLog;
        public event EventHandler<string> StaticLog;

        #endregion
    }
}
