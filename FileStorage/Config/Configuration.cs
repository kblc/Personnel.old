using FileStorage.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage
{
    public class Configuration
    {
        /// <summary>
        /// Mimetypes from config
        /// </summary>
        public IQueryable<MimeType> MimeTypes
        {
            get
            {
                var configInfo = ConfigurationManager.GetSection(MimeTypesConfigSection.SectionName) as MimeTypesConfigSection;
                return (
                    configInfo == null
                    ? Enumerable.Empty<MimeType>()
                    : configInfo.MimeTypes.OfType<MimeType>()
                    )
                    .AsQueryable();
            }
        }

        /// <summary>
        /// Unknown extension for unknown  mime type
        /// </summary>
        public string UnknownExtension
        {
            get
            {
                var configInfo = ConfigurationManager.GetSection(MimeTypesConfigSection.SectionName) as MimeTypesConfigSection;
                return (configInfo == null)
                    ? MimeTypesConfigSection.DefaultUnknownExtension
                    : configInfo.UnknownExtension;
            }
        }

        /// <summary>
        /// Unknown mime type for unknown extension
        /// </summary>
        public string UnknownMimeType
        {
            get
            {
                var configInfo = ConfigurationManager.GetSection(MimeTypesConfigSection.SectionName) as MimeTypesConfigSection;
                return (configInfo == null)
                    ? MimeTypesConfigSection.DefaultUnknownMimetype
                    : configInfo.UnknownMimetype;
            }
        }

        /// <summary>
        /// Verbose log from config
        /// </summary>
        public bool VerboseLog
        {
            get
            {
                var configInfo = ConfigurationManager.GetSection(StorageConfigSection.SectionName) as StorageConfigSection;
                return (configInfo == null)
                    ? StorageConfigSection.DefaultVerboseLog 
                    : configInfo.VerboseLog;
            }
        }

        /// <summary>
        /// Storage location from config
        /// </summary>
        public string Location
        {
            get
            {
                var configInfo = ConfigurationManager.GetSection(StorageConfigSection.SectionName) as StorageConfigSection;
                return (configInfo == null)
                    ? StorageConfigSection.DefaultLocation
                    : configInfo.Location;
            }
        }
    }
}
