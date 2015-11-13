using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Repository.Additional
{
    [AttributeUsage(AttributeTargets.All)]
    public class ResourceDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Resource key
        /// </summary>
        public string ResourceKey { get; }

        /// <summary>
        /// Create attribute with resource key
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        public ResourceDescriptionAttribute(string resourceKey)
        {
            this.ResourceKey = resourceKey;
        }

        /// <summary>
        /// Get description from resource file by resource key
        /// </summary>
        public string Description => Properties.Resources.ResourceManager.GetObject(ResourceKey)?.ToString() ?? ResourceKey;
    }
}
