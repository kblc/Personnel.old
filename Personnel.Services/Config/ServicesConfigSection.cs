using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Config
{
    public class ServicesConfigSection : ConfigurationSection
    {
        public const string SectionName = "servicesConfig";
        public const string DefaultFileServiceUrlPrefix = "";
        public const int DefaultMaxHistoryItemsPerRequest = 100;
        public const bool DefaultVerboseLog = false;
        public const string DefaultDefaultFileName = "unknown.bin";

        [ConfigurationProperty("fileServiceUrlPrefix", IsRequired = true, DefaultValue = DefaultFileServiceUrlPrefix)]
        public Uri FileServiceUrlPrefix
        {
            get
            {
                return this["fileServiceUrlPrefix"] as Uri;
            }
        }

        [ConfigurationProperty("defaultFileName", IsRequired = false, DefaultValue = DefaultDefaultFileName)]
        public string DefaultFileName
        {
            get
            {
                return this["defaultFileName"] as string;
            }
        }

        [ConfigurationProperty("maxHistoryItemsPerRequest", IsRequired = false, DefaultValue = DefaultMaxHistoryItemsPerRequest)]
        public int MaxHistoryItemsPerRequest
        {
            get
            {
                return (int)this["maxHistoryItemsPerRequest"];
            }
        }

        [ConfigurationProperty("verboseLog", IsRequired = false, DefaultValue = DefaultVerboseLog)]
        public bool VerboseLog
        {
            get
            {
                return (bool)this["verboseLog"];
            }
        }
    }
}
