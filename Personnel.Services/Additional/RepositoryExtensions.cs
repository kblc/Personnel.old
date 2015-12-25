using FileStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Additional
{
    public static class RepositoryExtensions
    {
        public static Repository.Model.File FilePut(this Repository.Logic.Repository repository, IFileStorage storage, Stream streamToUpload, string fileName, Encoding encoding = null)
        {
            var repFile = repository.New<Repository.Model.File>(f =>
            {
                f.Date = DateTime.UtcNow;
                f.FileId = Guid.NewGuid();
                f.FileName = fileName;
                f.MimeType = MimeStorage.GetMimeTypeByExtension(fileName);
            });

            var fileInfo = storage.FilePut(repFile.FileId, streamToUpload, fileName);
            repFile.UniqueFileName = fileInfo.Name;
            repFile.FileSize = fileInfo.Length;
            repFile.Encoding = encoding;
            return repFile;
        }

        public static Repository.Model.File FilePut(this Repository.Logic.Repository repository, IFileStorage storage, IEnumerable<string> lines, Encoding encoding, string fileName)
        {
            using (var stream = new System.IO.MemoryStream())
            using (var writer = new System.IO.StreamWriter(stream, encoding))
            {
                lines.SelectMany(str => str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                    .ToList()
                    .ForEach(s => writer.WriteLine(s));
                writer.Flush();
                var file = FilePut(repository, storage, stream, fileName);
                file.Encoding = encoding;
                return file;
            }
        }

        public static Repository.Model.File FilePut(this Repository.Logic.Repository repository, IFileStorage storage, string localFilePath, Encoding encoding)
        {
            using (var fileStream = System.IO.File.OpenRead(localFilePath))
            {
                var fileName = System.IO.Path.GetFileName(localFilePath);
                return repository.FilePut(storage, fileStream, fileName, encoding);
            }
        }

        public static IEnumerable<string> FileGetLines(this Repository.Logic.Repository repository, IFileStorage storage, Repository.Model.File repositoryFile)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (repositoryFile == null)
                throw new ArgumentNullException(nameof(repositoryFile));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            var res = new List<string>();
            using (var fileStream = storage.FileGet(repositoryFile.FileId))
            using (var sr = new System.IO.StreamReader(fileStream, repositoryFile.Encoding))
            {
                var line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                    res.Add(line);
            }
            return res;
        }
    }
}
