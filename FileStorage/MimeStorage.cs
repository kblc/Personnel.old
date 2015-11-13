using FileStorage.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage
{
    public class MimePreview
    {
        public string Small { get; private set; }
        public string Big { get; private set; }
        public MimePreview(string small, string big)
        {
            Small = small;
            Big = big;
        }
    }

    public static class MimeStorage
    {
        private static Configuration config = new Configuration();

        public static IQueryable<MimeType> MimeTypes
        {
            get
            {
                return config.MimeTypes;
            }
        }

        public static string GetMimeTypeByExtension(string fileNameOrExtension)
        {
            var extension = System.IO.Path.GetExtension(fileNameOrExtension);
            if (string.IsNullOrWhiteSpace(extension))
                extension = config.UnknownExtension;

            var mimeType = config.MimeTypes.FirstOrDefault(mt => string.Compare(mt.Extension, extension, true) == 0);
            return (mimeType == null) ? config.UnknownMimeType : mimeType.Name;
        }

        public static string GetExtensionByMimeType(string mimeType)
        {
            var mimeTp = 
                config.MimeTypes.FirstOrDefault(mt => string.Compare(mt.Name, mimeType, true) == 0)
                ?? config.MimeTypes.FirstOrDefault(mt => string.Compare(mt.Name, config.UnknownMimeType, true) == 0);
            return (mimeTp == null) ? config.UnknownExtension : mimeTp.Extension;
        }

        public static MimePreview GetPreviewImagesForMimeType(string mimeType)
        {
            var mimeTp =
                config.MimeTypes.FirstOrDefault(mt => string.Compare(mt.Name, mimeType, true) == 0)
                ?? config.MimeTypes.FirstOrDefault(mt => string.Compare(mt.Name, config.UnknownMimeType, true) == 0);
            return (mimeTp == null) ? null : new MimePreview(mimeTp.Small, mimeTp.Resource);
        }
    }
}
