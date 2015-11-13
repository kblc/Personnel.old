using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage
{
    /// <summary>
    /// File storage contract
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Get file stream from file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <returns>File stream</returns>
        Stream FileGet(Guid fileId);

        /// <summary>
        /// Get file stream from file storage
        /// </summary>
        /// <param name="fileName">File path</param>
        /// <returns>File stream</returns>
        Stream FileGet(string filePath);

        /// <summary>
        /// Put file to file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <param name="stream">Input file stream</param>
        /// <param name="fileName">File name or file extension</param>
        FileInfo FilePut(Guid fileId, Stream stream, string fileName);

        /// <summary>
        /// Rename file with specified identifier
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <param name="newFileName">New file name</param>
        /// <returns>File info</returns>
        FileInfo FileRename(Guid fileId, string newFileName);

        /// <summary>
        /// Delete file from file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        void FileDelete(Guid fileId);

        /// <summary>
        /// Log event
        /// </summary>
        event EventHandler<string> Log;

        /// <summary>
        /// Exception event
        /// </summary>
        event EventHandler<Exception> Exception;

        /// <summary>
        /// Verbose log
        /// </summary>
        bool VerboseLog { get; set; }
    }
}
