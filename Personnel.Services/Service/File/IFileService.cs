using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.File
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IFileService : Base.IBaseService
    {
        /// <summary>
        /// Get file info by file identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>File info</returns>
        [OperationContract]
        FileExecutionResult Get(Guid identifier);

        /// <summary>
        /// Get picture info by file identifier
        /// </summary>
        /// <param name="identifier">Picture file identifier</param>
        /// <returns>Picture info</returns>
        [OperationContract]
        PictureExecutionResult GetPicture(Guid identifier);

        /// <summary>
        /// Delete file by identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        [OperationContract(IsOneWay = true)]
        void Remove(Guid identifier);

        /// <summary>
        /// Get file infos by identifiers
        /// </summary>
        /// <param name="identifiers">File info identifiers</param>
        /// <returns>Files info</returns>
        [OperationContract]
        FileExecutionResults GetRange(Guid[] identifiers);

        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Source stream</returns>
        [OperationContract]
        System.IO.Stream GetSourceByName(string fileName);

        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>Source stream</returns>
        [OperationContract]
        System.IO.Stream GetSource(Guid identifier);

        /// <summary>
        /// Put file with source and parameters
        /// </summary>
        /// <param name="content">Source stream</param>
        /// <returns>File identifier</returns>
        [OperationContract]
        FileExecutionResult Put(System.IO.Stream content);

        /// <summary>
        /// Update file in database
        /// </summary>
        /// <param name="file">File to update</param>
        /// <returns>File info</returns>
        [OperationContract]
        FileExecutionResult Update(Model.File item);

        /// <summary>
        /// Update picture in database
        /// </summary>
        /// <param name="file">Picture to update</param>
        /// <returns>Picture info</returns>
        [OperationContract]
        PictureExecutionResult UpdatePicture(Model.Picture item);
    }

    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IFileServiceREST : Base.IBaseService
    {
        /// <summary>
        /// Get file info by file identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>File info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/file/{identifier}")]
        FileExecutionResult RESTGet(string identifier);

        /// <summary>
        /// Get picture info by file identifier
        /// </summary>
        /// <param name="identifier">Picture file identifier</param>
        /// <returns>Picture info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/picture/{identifier}")]
        PictureExecutionResult RESTGetPicture(string identifier);

        /// <summary>
        /// Delete file by identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        [OperationContract(IsOneWay = true)]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/file/{identifier}")]
        void RESTRemove(string identifier);

        /// <summary>
        /// Get file infos by identifiers
        /// </summary>
        /// <param name="identifiers">File info identifiers</param>
        /// <returns>Files info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/file/range")]
        FileExecutionResults RESTGetRange(string[] identifiers);

        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="fileIdOrName">File identifier or name</param>
        /// <returns>Source stream</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/file?source={fileIdOrName}")]
        System.IO.Stream RESTGetSource(string fileIdOrName);

        /// <summary>
        /// Put file with source and parameters
        /// </summary>
        /// <param name="content">Source stream</param>
        /// <returns>File identifier</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/file")]
        FileExecutionResult RESTPut(System.IO.Stream content);

        /// <summary>
        /// Update file in database
        /// </summary>
        /// <param name="file">File to update</param>
        /// <returns>File info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/file")]
        FileExecutionResult RESTUpdate(Model.File item);
        
        /// <summary>
        /// Update picture in database
        /// </summary>
        /// <param name="file">Picture to update</param>
        /// <returns>Picture info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/picture")]
        PictureExecutionResult RESTUpdatePicture(Model.Picture item);
    }
}
