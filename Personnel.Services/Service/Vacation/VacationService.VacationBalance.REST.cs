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
        /// Get information about employee vacation balances
        /// </summary>
        /// <returns>Result info</returns>
        public Model.VacationBalanceExecutionResults RESTVacationBalanceGet()
            => VacationBalanceGet();

        /// <summary>
        /// Insert single vacation balance
        /// </summary>
        /// <param name="vacationBalance">Vacation balance</param>
        /// <returns>Result info</returns>
        public Model.VacationBalanceExecutionResult RESTVacationBalanceInsert(Model.Vacation vacationBalance)
            => RESTVacationBalanceInsert(vacationBalance);
    }
}
