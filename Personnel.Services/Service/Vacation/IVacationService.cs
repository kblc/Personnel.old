using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Vacation
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public partial interface IVacationService : Service.Base.IBaseService
    {

    }

    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public partial interface IVacationServiceREST : Service.Base.IBaseService
    {

    }
}
