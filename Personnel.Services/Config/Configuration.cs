using Personnel.Services.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services
{
    public class Configuration
    {
        /// <summary>
        /// Max history items per request
        /// </summary>
        public int MaxHistoryItemsPerRequest
        {
            get
            {
                var configInfo = ConfigurationManager.GetSection(ServicesConfigSection.SectionName) as ServicesConfigSection;
                return (configInfo == null)
                    ? ServicesConfigSection.DefaultMaxHistoryItemsPerRequest
                    : configInfo.MaxHistoryItemsPerRequest;
            }
        }

        /// <summary>
        /// Default file name for file service
        /// </summary>
        public string DefaultFileName
        {
            get
            {
                var configInfo = ConfigurationManager.GetSection(ServicesConfigSection.SectionName) as ServicesConfigSection;
                return (configInfo == null)
                    ? ServicesConfigSection.DefaultDefaultFileName
                    : configInfo.DefaultFileName;
            }
        }

        /// <summary>
        /// Verbose log from config
        /// </summary>
        public bool VerboseLog
        {
            get
            {
                var configInfo = ConfigurationManager.GetSection(ServicesConfigSection.SectionName) as ServicesConfigSection;
                return (configInfo == null)
                    ? ServicesConfigSection.DefaultVerboseLog 
                    : configInfo.VerboseLog;
            }
        }

        /// <summary>
        /// File service preffix
        /// </summary>
        public Uri FileServiceUrlPrefix
        {
            get
            {
                var configInfo = ConfigurationManager.GetSection(ServicesConfigSection.SectionName) as ServicesConfigSection;
                return (configInfo == null)
                    ? new Uri(ServicesConfigSection.DefaultFileServiceUrlPrefix)
                    : configInfo.FileServiceUrlPrefix;
            }
        }
    }
}
