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
            var currentRightIds = employee.Rights.Select(r => r.RightId).ToArray();
            var existedRightTypes = rep.Get<Repository.Model.Right>(asNoTracking: true)
                .ToArray()
                .Join(employee.Rights, r => r.RightId, r => r.RightId, (r1, r2) => r1.Type)
                .ToArray();
            SRVCCheckCredentials(logSession, rep, existedRightTypes, rightTypes);
            return employee;
        }

        protected void SRVCCheckCredentials(Helpers.Log.SessionInfo logSession, Repository.Logic.Repository rep, Repository.Model.RightType[] existedRights, Repository.Model.RightType[] rightTypes)
        {
            var missingRights = rightTypes.Except(existedRights);
            var missingRightName = rep.Get<Repository.Model.Right>(asNoTracking: true)
                .ToArray()
                .Where(r => missingRights.Contains(r.Type))
                .Select(r => r.Name);

            if (missingRightName.Any())
                throw new System.Security.SecurityException(string.Format(Properties.Resources.STUFFINGSERVICE_UserHaveNoFollowingRights, missingRightName.Concat(i => i.ToString(), ", ")));
        }

        protected Model.Employee SRVCGetEmployeeCredentials(Helpers.Log.SessionInfo logSession, Repository.Logic.Repository rep, out Repository.Model.RightType[] rightTypes)
        {
            var employee = SRVCGetCurrentEmployee(logSession, rep);
            var currentRightIds = employee.Rights.Select(r => r.RightId).ToArray();
            var existingRight = rep
                .Get<Repository.Model.Right>(asNoTracking: true)
                .ToArray()
                .Join(employee.Rights, r => r.RightId, r=>r.RightId, (r1,r2) => r1.Type)
                .ToArray();
            rightTypes = existingRight;
            return employee;
        }

        protected Model.Employee SRVCGetCurrentEmployee(Helpers.Log.SessionInfo logSession, Repository.Logic.Repository rep)
        {
            var currentIdentityName =
                ServiceSecurityContext.Current?.WindowsIdentity?.Name
                ?? ServiceSecurityContext.Current?.PrimaryIdentity?.Name
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
