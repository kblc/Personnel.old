using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Vacation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class VacationService : Service.Base.BaseService, IVacationService, IVacationServiceREST
    {
    }
}
