using FileStorage;
using Helpers;
using Helpers.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.File
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class FileService : Base.BaseService, IFileService, IFileServiceREST
    {
        public string DefaultFileName { get; set; } = Config.ServicesConfigSection.DefaultDefaultFileName;

        public FileService()
        {
            try
            {
                var cfg = new Configuration();
                cfg.CopyObject(this);
            }
            catch { }
        }

        #region File storage

        private IFileStorage mainFileStorage = null;
        protected IFileStorage MainFileStorage
        {
            get
            {
                if (mainFileStorage == null)
                {
                    mainFileStorage = new FileStorage.FileStorage();
                    mainFileStorage.Log += (s, e) => RaiseLog($"[FILESTORAGE] {e}");
                    mainFileStorage.VerboseLog = VerboseLog;
                }
                return mainFileStorage;
            }
        }

        #endregion

        /// <summary>
        /// Set some header for output response stream
        /// </summary>
        /// <param name="mime">File mime type</param>
        /// <param name="encoding">File encoding</param>
        /// <param name="fileName">File name</param>
        private void SetOutputResponseHeaders(string mime, Encoding encoding, string fileName, Helpers.Log.SessionInfo upperLogSession)
        {
            if (upperLogSession != null)
                throw new ArgumentNullException(nameof(upperLogSession));

            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, ss => ss.ToList().ForEach(s => upperLogSession.Add(s))))
                try
                {
                    var webContext = System.ServiceModel.Web.WebOperationContext.Current;
                    if (webContext != null)
                    {
                        webContext.OutgoingResponse.ContentType = new string[] { mime, encoding?.WebName }.Where(s => !string.IsNullOrWhiteSpace(s)).Concat(s => s, "; ");
                        if (!string.IsNullOrWhiteSpace(fileName))
                            webContext.OutgoingResponse.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                    }
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    upperLogSession.Enabled = true;
                }
        }

        /// <summary>
        /// Get parameters from input request stream
        /// </summary>
        /// <param name="mime">File mime type</param>
        /// <param name="encoding">File encoding</param>
        /// <param name="fileName">File name</param>
        private void GetInputRequestHeaders(out string mime, out Encoding encoding, out string fileName, Helpers.Log.SessionInfo upperLogSession)
        {
            if (upperLogSession != null)
                throw new ArgumentNullException(nameof(upperLogSession));

            mime = null;
            encoding = null;
            fileName = null;
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, ss => ss.ToList().ForEach(s => upperLogSession.Add(s))))
                try
                {
                    var webContext = System.ServiceModel.Web.WebOperationContext.Current;
                    if (webContext != null)
                    {
                        var ct = webContext.IncomingRequest.ContentType.Split(new char[] { ';' }, StringSplitOptions.None).Select(i => i.Trim());
                        mime = ct.FirstOrDefault();
                        var encName = ct.Skip(1).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(encName))
                            try { encoding = Encoding.GetEncoding(encName); } catch { }

                        var cd = webContext.IncomingRequest.Headers.Get("Content-Disposition");
                        if (!string.IsNullOrWhiteSpace(cd))
                        {
                            var fName = cd.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).FirstOrDefault(i => i.ToLower().StartsWith("filename="));
                            fileName = fName.Substring(fName.IndexOf("=") + 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    upperLogSession.Enabled = true;
                }
        }

        #region Service contract implementation

        /// <summary>
        /// Delete file by identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        public void Remove(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageFile);

                        logSession.Add($"Try to get file with id = '{identifier}' from database...");
                        var file = rep.GetFile(identifier);
                        if (file != null)
                        {
                            logSession.Add($"File found: '{file}'.");
                            rep.Remove(file);
                            logSession.Add($"File with id = '{identifier}' deleted from database.");
                            MainFileStorage.FileDelete(file.FileId);
                            logSession.Add($"File with id = '{identifier}' deleted from file storage.");
                        }
                        else
                            throw new Exception(string.Format(Properties.Resources.FILESERVICE_FileNotFound, identifier));
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
                }
        }
        /// <summary>
        /// Delete file by identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        public void RESTRemove(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GuidFromString(identifier);
                    Remove(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                }
        }

        /// <summary>
        /// Get file info by file identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>File info</returns>
        public FileExecutionResult Get(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = GetRange(new Guid[] { identifier });
                    if (res.Exception != null)
                        throw res.Exception;

                    if (res.Values.Length != 1)
                        throw new Exception(string.Format(Properties.Resources.FILESERVICE_FileNotFound, identifier));

                    return new FileExecutionResult(res.Values.First());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResult(ex);
                }
        }
        /// <summary>
        /// Get file info by file identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>File info</returns>
        public FileExecutionResult RESTGet(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GuidFromString(identifier);
                    return Get(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResult(ex);
                }
        }

        /// <summary>
        /// Get picture info by file identifier
        /// </summary>
        /// <param name="identifier">Picture file identifier</param>
        /// <returns>Picture info</returns>
        public PictureExecutionResult GetPicture(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbPicture = rep.Get<Repository.Model.Picture>(p => p.FileId == identifier).SingleOrDefault();
                        if (dbPicture == null)
                            throw new Exception(Properties.Resources.FILESERVICE_FileNotFound);
                        var res = AutoMapper.Mapper.Map<Model.Picture>(dbPicture);
                        return new PictureExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new PictureExecutionResult(ex);
                }
        }
        /// <summary>
        /// Get picture info by file identifier
        /// </summary>
        /// <param name="identifier">Picture file identifier</param>
        /// <returns>Picture info</returns>
        public PictureExecutionResult RESTGetPicture(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GuidFromString(identifier);
                    return GetPicture(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new PictureExecutionResult(ex);
                }
        }

        /// <summary>
        /// Get file infos by identifiers
        /// </summary>
        /// <param name="identifiers">File info identifiers</param>
        /// <returns>Files info</returns>
        public FileExecutionResults GetRange(Guid[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var res = rep.Get<Repository.Model.File>(f => identifiers.Contains(f.FileId))
                            .ToArray()
                            .Select(f => AutoMapper.Mapper.Map<Model.File>(f))
                            .ToArray();
                        return new FileExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString("N"), ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResults(ex);
                }
        }
        /// <summary>
        /// Get file infos by identifiers
        /// </summary>
        /// <param name="identifiers">File info identifiers</param>
        /// <returns>Files info</returns>
        public FileExecutionResults RESTGetRange(string[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var valiIdentifiers = identifiers.Select(i => GuidFromString(i)).ToArray();
                    return GetRange(valiIdentifiers);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i ?? "NULL", ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>Source stream</returns>
        public Stream GetSource(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var file = rep.GetFile(identifier);
                        if (file != null)
                            SetOutputResponseHeaders(file.MimeType, file.Encoding, file.FileName, logSession);
                    }
                    logSession.Add($"Try to get file with id = '{identifier}' from data storage...");
                    return MainFileStorage.FileGet(identifier);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
                }
        }
        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Source stream</returns>
        public Stream GetSourceByName(string fileName)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login);

                        var file = rep.GetFile(fileName);
                        if (file != null)
                            SetOutputResponseHeaders(file.MimeType, file.Encoding, file.FileName, logSession);
                    }
                    logSession.Add($"Try to get file with name = '{fileName}' from data storage...");
                    return MainFileStorage.FileGet(fileName);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileName), fileName);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
                }
        }
        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="fileIdOrName">File identifier or name</param>
        /// <returns>Source stream</returns>
        public Stream RESTGetSource(string fileIdOrName)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    logSession.Add($"Try to get file with id or name = '{fileIdOrName}' from database...");
                    var fileId = TryGuidFromString(fileIdOrName);
                    if (fileId.HasValue)
                        return GetSource(fileId.Value);
                    return GetSourceByName(fileIdOrName);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileIdOrName), fileIdOrName);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
                }
        }

        /// <summary>
        /// Put file with source and parameters
        /// </summary>
        /// <param name="content">Source stream</param>
        /// <returns>File identifier</returns>
        public FileExecutionResult Put(System.IO.Stream content)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.UploadFile);

                        string mimeType;
                        Encoding encoding;
                        string fileName;

                        GetInputRequestHeaders(out mimeType, out encoding, out fileName, logSession);

                        var dbFile = rep.New<Repository.Model.File>((f) =>
                        {
                            f.FileName = string.IsNullOrWhiteSpace(fileName) ? DefaultFileName : fileName;
                            f.Encoding = encoding;
                            f.MimeType = string.IsNullOrWhiteSpace(mimeType) ? FileStorage.MimeStorage.GetMimeTypeByExtension(f.FileName) : mimeType;
                        });

                        logSession.Add($"Try to save file to file storage...");
                        var fi = MainFileStorage.FilePut(dbFile.FileId, content, dbFile.FileName);

                        dbFile.FileSize = fi.Length;
                        dbFile.UniqueFileName = fi.Name;

                        logSession.Add($"Try to save file to database...");
                        rep.Add(dbFile);

                        return new FileExecutionResult(AutoMapper.Mapper.Map<Model.File>(dbFile));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResult(ex);
                }
        }
        /// <summary>
        /// Put file with source and parameters
        /// </summary>
        /// <param name="content">Source stream</param>
        /// <returns>File identifier</returns>
        public FileExecutionResult RESTPut(System.IO.Stream content) => Put(content);

        /// <summary>
        /// Update file in database
        /// </summary>
        /// <param name="file">File to update</param>
        /// <returns>File info</returns>
        public FileExecutionResult Update(Model.File item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.UploadFile);

                        logSession.Add($"Try to get file with id = '{item.Id}' from database...");

                        var dbFile = rep.GetFile(item.Id);
                        if (dbFile == null)
                            throw new Exception(string.Format(Properties.Resources.FILESERVICE_FileNotFound, item.Id));

                        if (item.EncodingName != null)
                            dbFile.Encoding = item.Encoding;

                        dbFile.MimeType = (string.IsNullOrEmpty(item.Mime))
                            ? FileStorage.MimeStorage.GetMimeTypeByExtension(dbFile.UniqueFileName)
                            : item.Mime;

                        if (!string.IsNullOrEmpty(item.Name))
                        {
                            dbFile.FileName = System.IO.Path.GetFileName(item.Name);

                            logSession.Add($"Try to rename file in file storage...");
                            var fi = MainFileStorage.FileRename(item.Id, dbFile.FileName);
                            dbFile.UniqueFileName = fi.Name;
                        }

                        logSession.Add($"Try to update file in database...");
                        rep.SaveChanges();

                        return new FileExecutionResult(AutoMapper.Mapper.Map<Model.File>(dbFile));
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item.ToString());
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResult(ex);
                }
        }
        /// <summary>
        /// Update file in database
        /// </summary>
        /// <param name="file">File to update</param>
        /// <returns>File info</returns>
        public FileExecutionResult RESTUpdate(Model.File item) => Update(item);

        /// <summary>
        /// Update picture in database
        /// </summary>
        /// <param name="item">Picture to update</param>
        /// <returns>Picture info</returns>
        public PictureExecutionResult UpdatePicture(Model.Picture item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var itm = rep
                            .Get<Repository.Model.File>(p => p.FileId == item.FileId, asNoTracking: true)
                            .LeftOuterJoin(rep.Get<Repository.Model.Picture>(asNoTracking: true), f=>f.FileId, p=>p.FileId, (File,Picture) => new { File, Picture })
                            .FirstOrDefault();

                        if (itm == null || itm.File == null)
                            throw new Exception(string.Format(Properties.Resources.FILESERVICE_FileNotFound, item.FileId));

                        var dbPicture = AutoMapper.Mapper.Map<Repository.Model.Picture>(item);
                        rep.AddOrUpdate(dbPicture, state: (itm.Picture != null ? System.Data.Entity.EntityState.Modified : System.Data.Entity.EntityState.Added) );
                    }
                    return GetPicture(item.FileId);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item.ToString());
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new PictureExecutionResult(ex);
                }
        }

        /// <summary>
        /// Update picture in database
        /// </summary>
        /// <param name="item">Picture to update</param>
        /// <returns>Picture info</returns>
        public PictureExecutionResult RESTUpdatePicture(Model.Picture item) => UpdatePicture(item);

        /// <summary>
        /// Update file in database
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <param name="fileName">New file name</param>
        /// <param name="encoding">New file encoding</param>
        /// <param name="mime">New mime type</param>
        /// <returns>File info</returns>
        public FileExecutionResult RESTUpdate(string identifier, string fileName, string encoding, string mime)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    return Update(new Model.File() { Id = GuidFromString(identifier), Name = fileName, EncodingName = encoding, Mime = mime });
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    ex.Data.Add(nameof(fileName), fileName);
                    ex.Data.Add(nameof(encoding), encoding);
                    ex.Data.Add(nameof(mime), mime);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResult(ex);
                }
        }

        #endregion
    }
}
