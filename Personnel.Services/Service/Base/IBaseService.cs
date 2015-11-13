using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Base
{
    [ServiceContract]
    public interface IBaseService
    {
        /// <summary>
        /// Set session lang
        /// </summary>
        /// <param name="codename">New session codename</param>
        [OperationContract(IsOneWay = true)]
        [WebInvoke(UriTemplate = "/ln/{codename}", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        void ChangeLanguage(string codename);
    }
}
