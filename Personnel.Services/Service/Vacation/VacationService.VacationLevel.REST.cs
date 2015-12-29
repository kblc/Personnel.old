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
        public Model.VacationLevelExecutionResults RESTGetLevels()
            => GetLevels();
    }
}
