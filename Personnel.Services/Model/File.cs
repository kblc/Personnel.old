using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// File
    /// </summary>
    [DataContract]
    public class File
    {
        /// <summary>
        /// File identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public Guid Id { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        /// <summary>
        /// File size
        /// </summary>
        [DataMember(IsRequired = false)]
        public long? Size { get; set; }

        /// <summary>
        /// File mime type
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Mime { get; set; }

        /// <summary>
        /// File mime type
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Link { get; set; }

        /// <summary>
        /// File mime type
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Preview { get; set; }

        /// <summary>
        /// File mime type
        /// </summary>
        [DataMember(IsRequired = false)]
        public string PreviewSmall { get; set; }

        /// <summary>
        /// File date created
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime? Date { get; set; }

        /// <summary>
        /// File encoding
        /// </summary>
        [DataMember(Name = "Encoding", IsRequired = false)]
        public string EncodingName { get; set; }

        [IgnoreDataMember]
        public Encoding Encoding
        {
            get { try { return Encoding.GetEncoding(EncodingName); } catch { return Encoding.Default; } }
            set { EncodingName = value == null ? null : value.WebName; }
        }
    }
}
