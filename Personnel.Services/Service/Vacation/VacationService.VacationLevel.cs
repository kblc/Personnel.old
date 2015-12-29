using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace Personnel.Services.Service.Vacation
{
    public partial class VacationService : IVacationService
    {
        /// <summary>
        /// Get alailable levels for vacation
        /// </summary>
        /// <returns>Result info</returns>
        public Model.VacationLevelExecutionResults GetLevels()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var dbRes = rep.Get<Repository.Model.VacationLevel>(asNoTracking: true);
                        var res = dbRes.ToArray().Select(i => AutoMapper.Mapper.Map<Model.VacationLevel>(i)).ToArray();

                        return new Model.VacationLevelExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationLevelExecutionResults(ex);
                }
        }
    }
}
