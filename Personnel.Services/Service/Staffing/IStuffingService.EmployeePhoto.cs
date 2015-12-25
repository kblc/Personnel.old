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
        /// <param name="photo">Employee photo</param>
        [OperationContract]
        Model.EmployeePhotoExecutionResult EmployeePhotoAdd(long employeeId, Model.EmployeePhoto photo);

        /// <summary>
        /// Add photos to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photos">Employee photos</param>
        [OperationContract]
        Model.EmployeePhotoExecutionResults EmployeePhotosAdd(long employeeId, Model.EmployeePhoto[] photos);

        /// <summary>
        /// Remove photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifier">Employee photo identifier</param>
        [OperationContract]
        Model.EmployeePhotoExecutionResult EmployeePhotoRemove(long employeeId, Guid photoIdentifier);

        /// <summary>
        /// Remove photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifiers">Employee photo identifier</param>
        [OperationContract]
        Model.EmployeePhotoExecutionResults EmployeePhotosRemove(long employeeId, Guid[] photoIdentifiers);
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
        /// <param name="photo">Employee photo</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/photo")]
        Model.EmployeePhotoExecutionResult RESTEmployeePhotoAdd(string employeeId, Model.EmployeePhoto photo);

        /// <summary>
        /// Add photos to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photos">Employee photos</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/photos")]
        Model.EmployeePhotoExecutionResults RESTEmployeePhotosAdd(string employeeId, Model.EmployeePhoto[] photos);

        /// <summary>
        /// Remove photo from employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifier">Employee photo identifier</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/photo")]
        Model.EmployeePhotoExecutionResult RESTEmployeePhotoRemove(string employeeId, string photoIdentifier);

        /// <summary>
        /// Remove photo from employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifiers">Employee photo identifiers</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/employee/{employeeId}/photos")]
        Model.EmployeePhotoExecutionResults RESTEmployeePhotosRemove(string employeeId, string[] photoIdentifiers);
    }

}
