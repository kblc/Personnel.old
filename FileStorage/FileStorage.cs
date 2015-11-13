using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage
{
    /// <summary>
    /// Default implementation for file storage
    /// </summary>
    public class FileStorage : IFileStorage
    {
        /// <summary>
        /// Create new instance
        /// </summary>
        public FileStorage()
        {
            try
            {
                var cfg = new Configuration();
                cfg.CopyObject(this);
            }
            catch { }
        }

        /// <summary>
        /// File storage location
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Enable verbose log events
        /// </summary>
        public bool VerboseLog { get; set; } = false;

        /// <summary>
        /// Delete file from file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        public void FileDelete(Guid fileId)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(FileDelete)}()", VerboseLog, RaiseLogEvent))
                try
                {
                    var searchPattern = fileId.ToString("N") + "*";

                    logSession.Add($"Delete file with '{searchPattern}' names in next location: '{Location}'");
                    var filesToDelete = System.IO.Directory.GetFiles(Location, searchPattern, SearchOption.TopDirectoryOnly);
                    logSession.Add($"Files founded: {filesToDelete.Length}");

                    foreach (var fileName in filesToDelete)
                        try
                        {
                            logSession.Add($"Try delete file '{fileName}'");
                            System.IO.File.Delete(fileName);
                        }
                        catch (Exception ex)
                        {
                            ex.Data.Add(nameof(fileName), fileName);
                            throw ex;
                        }
                    logSession.Add("All files deleted");
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileId), fileId);
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    RaiseExceptionEvent(ex);
                    throw ex;
                }
        }

        /// <summary>
        /// Get file stream from file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <returns>File stream</returns>
        public Stream FileGet(Guid fileId)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(FileGet)}()", VerboseLog, RaiseLogEvent))
                try
                {
                    var searchPattern = fileId.ToString("N") + "*";

                    logSession.Add($"Try get file with '{searchPattern}' names in next location: '{Location}'");
                    var filesExisted = System.IO.Directory.GetFiles(Location, searchPattern, SearchOption.TopDirectoryOnly);
                    logSession.Add($"Files founded: {filesExisted.Length}");
                    if (filesExisted.Length == 1)
                    {
                        var fileName = filesExisted.First();
                        logSession.Add($"Open following file: {fileName}");
                        return System.IO.File.OpenRead(fileName);
                    }
                    else
                        throw new Exception($"File count must be 1 instead of {filesExisted.Length}");
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileId), fileId);
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    RaiseExceptionEvent(ex);
                    throw ex;
                }
        }

        /// <summary>
        /// Get file stream from file storage
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>File stream</returns>
        public Stream FileGet(string filePath)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(FileGet)}()", VerboseLog, RaiseLogEvent))
                try
                {
                    filePath = filePath.Replace(@"/", @"\");
                    if (filePath.StartsWith(@"\"))
                        filePath = filePath.Remove(0);
                    var fullFilePath = System.IO.Path.Combine(Location, filePath.Replace(@"/", @"\"));
                    logSession.Add($"Try get file '{filePath}' in next location: '{fullFilePath}'");

                    if (filePath.StartsWith(Location))
                    {
                        if (System.IO.File.Exists(fullFilePath))
                        {
                            return System.IO.File.OpenRead(fullFilePath);
                        }
                        else
                            throw new System.Exception("File not found");
                    }
                    else
                        throw new System.Security.SecurityException($"Wrong file path");
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(filePath), filePath);
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    RaiseExceptionEvent(ex);
                    throw ex;
                }
        }

        /// <summary>
        /// Rename file from storage with file identifier and newFileName
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <param name="newFileName">New file name</param>
        /// <returns>Rename file info</returns>
        public FileInfo FileRename(Guid fileId, string newFileName)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLogEvent))
                try
                {
                    var nwFileName = Path.GetFileName(newFileName);
                    foreach (var c in Path.GetInvalidFileNameChars())
                        nwFileName = nwFileName.Replace(c.ToString(), "");

                    var newFullName = fileId.ToString("N") + (Path.GetExtension(nwFileName) != nwFileName ? "_" : string.Empty) + nwFileName;
                    var newFilePath = System.IO.Path.Combine(Location, newFullName);


                    var searchPattern = fileId.ToString("N") + "*";
                    logSession.Add($"Try get file with '{searchPattern}' names in next location: '{Location}'");
                    var filesExisted = System.IO.Directory.GetFiles(Location, searchPattern, SearchOption.TopDirectoryOnly);
                    logSession.Add($"Files founded: {filesExisted.Length}");
                    if (filesExisted.Length == 1)
                    {
                        var originalFilePath = filesExisted.First();

                        logSession.Add($"Try put file to '{newFilePath}' from '{originalFilePath}'");
                        try
                        {
                            System.IO.File.Move(originalFilePath, newFilePath);
                            return new FileInfo(newFilePath);
                        }
                        catch (Exception ex)
                        {
                            ex.Data.Add(nameof(newFilePath), newFilePath);
                            throw ex;
                        }
                    }
                    else
                        throw new Exception($"File count must be 1 instead of {filesExisted.Length}");
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileId), fileId);
                    ex.Data.Add(nameof(newFileName), newFileName);
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    RaiseExceptionEvent(ex);
                    throw ex;
                }
        }

        /// <summary>
        /// Put file to file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <param name="stream">Input file stream</param>
        /// <param name="fileName">File name</param>
        public FileInfo FilePut(Guid fileId, Stream stream, string fileName)
        {
            return FilePutWithExtension(fileId, stream, fileName);
        }

        /// <summary>
        /// Put file to file storage with file identifier and extension
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <param name="stream">Stream to save</param>
        /// <param name="originalFileName">File extension</param>
        /// <returns>Save file info</returns>
        private FileInfo FilePutWithExtension(Guid fileId, Stream stream, string originalFileName)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(FilePutWithExtension)}()", VerboseLog, RaiseLogEvent))
                try
                {
                    var origFileName = Path.GetFileName(originalFileName);
                    foreach (var c in Path.GetInvalidFileNameChars())
                        origFileName = origFileName.Replace(c.ToString(), "");

                    var fileName = fileId.ToString("N") + (Path.GetExtension(origFileName) != origFileName ? "_" : string.Empty) + origFileName;
                    var filePath = System.IO.Path.Combine(Location, fileName);
                    logSession.Add($"Try put file with '{fileName}' name in next location: '{Location}'");
                    try
                    {
                        using (var fileStream = File.OpenWrite(filePath))
                        {
                            if (stream.CanSeek)
                                stream.Seek(0, SeekOrigin.Begin);

                            stream.CopyTo(fileStream);
                            fileStream.Flush();
                        }
                        logSession.Add($"File saved in '{filePath}'");
                        return new FileInfo(filePath);
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add(nameof(filePath), filePath);
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileId), fileId);
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    RaiseExceptionEvent(ex);
                    throw ex;
                }
        }

        #region Events

        public event EventHandler<string> Log;
        public event EventHandler<Exception> Exception;

        private void RaiseLogEvent(IEnumerable<string> logTexts)
        {
            logTexts?.ToList().ForEach(s => RaiseLogEvent(s));
        }
        private void RaiseLogEvent(string logText)
        {
            Log?.Invoke(this, logText);
        }
        private void RaiseExceptionEvent(Exception ex)
        {
            Exception?.Invoke(this, ex);
        }

        #endregion
    }
}
