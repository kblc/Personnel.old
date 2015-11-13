using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Personnel.Repository.Logic;
using Personnel.Repository.Model;

namespace Personnel.Services.Service.Staffing
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class StaffingService : Service.Base.BaseService, IStaffingService, IStaffingServiceREST
    {
    }
}
