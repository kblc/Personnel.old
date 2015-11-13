using Personnel.Repository.Additional;
using Personnel.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace Personnel.Repository.Logic
{
    public partial class Repository
    {
        /// <summary>
        /// Get new File without any link to database
        /// </summary>
        /// <returns>File instance</returns>
        public File NewFile(string fileName = null, Encoding encoding = null, long? fileSize = null, string mimeType = null)
        {
            return New<File>(f =>
            {
                f.FileId = Guid.NewGuid();
                f.Date = DateTime.UtcNow;
                f.Encoding = encoding;
                f.FileName = fileName;
                f.FileSize = fileSize == null ? default(long) : fileSize.Value;
                f.MimeType = mimeType;
            });
        }

        /// <summary>
        /// Get file with identifier from database
        /// </summary>
        /// <param name="asNoTracking">Without tracking</param>
        /// <param name="identifier">File identifier</param>
        /// <returns>File instance</returns>
        public File GetFile(Guid identifier, bool asNoTracking = false)
        {
            return Get<File>(f => f.FileId == identifier, asNoTracking: asNoTracking)
                .SingleOrDefault();
        }

        /// <summary>
        /// Get file with identifier from database
        /// </summary>
        /// <param name="asNoTracking">Without tracking</param>
        /// <param name="fileName">File name</param>
        /// <returns>File instance</returns>
        public File GetFile(string fileName, bool asNoTracking = false)
        {
            return Get<File>(f => f.UniqueFileName.EndsWith(fileName), asNoTracking: asNoTracking)
                .SingleOrDefault();
        }
    }
}
