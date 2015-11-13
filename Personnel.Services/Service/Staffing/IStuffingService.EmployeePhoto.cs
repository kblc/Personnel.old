using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Staffing
{
    public partial interface IStaffingService
    {
        /// <summary>
        /// Get photos for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        [OperationContract]
        Model.EmployeePhotoExecutionResults EmployeePhotosGet(long employeeId);

        /// <summary>
        /// Add photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="name">Employee photo</param>
        [OperationContract]
        Model.EmployeePhotoExecutionResults EmployeePhotosAdd(long employeeId, Model.EmployeePhoto photo);

        /// <summary>
        /// Remove photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifier">Employee photo identifier</param>
        [OperationContract]
        Model.EmployeePhotoExecutionResults EmployeePhotosRemove(long employeeId, Guid photoIdentifier);
    }

    public partial interface IStaffingServiceREST
    {
        /// <summary>
        /// Get photos for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/photos")]
        Model.EmployeePhotoExecutionResults RESTEmployeePhotosGet(string employeeId);

        /// <summary>
        /// Add photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="name">Employee photo</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/photos")]
        Model.EmployeePhotoExecutionResults RESTEmployeePhotosAdd(string employeeId, Model.EmployeePhoto photo);

        /// <summary>
        /// Remove photo from employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifier">Employee photo identifier</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/photos")]
        Model.EmployeePhotoExecutionResults RESTEmployeePhotosRemove(string employeeId, string photoIdentifier);
    }

}
