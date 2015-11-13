using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Config
{
    public class StorageConfigSection : ConfigurationSection
    {
        public const string SectionName = "storageConfig";
        public const bool DefaultVerboseLog = false;
        public const string DefaultLocation = "";

        [ConfigurationProperty("location", IsRequired = true, DefaultValue = DefaultLocation)]
        public string Location
        {
            get
            {
                return this["location"] as string;
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
