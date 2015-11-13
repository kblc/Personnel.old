using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    [DataContract]
    public enum PictureType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Avatar32,
        [EnumMember]
        Avatar64,
        [EnumMember]
        Avatar128,
        [EnumMember]
        Avatar256,
    }

    /// <summary>
    /// Picture
    /// </summary>
    [DataContract]
    public class Picture
    {
        [DataMember]
        public Guid FileId { get; set; }

        /// <summary>
        /// Picture type name
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Picture type
        /// </summary>
        [DataMember]
        public PictureType PictureType { get; set; }

        /// <summary>
        /// Picture width
        /// </summary>
        [DataMember]
        public int Width { get; set; }

        /// <summary>
        /// Picture height
        /// </summary>
        [DataMember]
        public int Height { get; set; }

        [DataMember(IsRequired = false)]
        public File File { get; set; }
    }

    [DataContract(Name = "PictureResult")]
    public class PictureExecutionResult : BaseExecutionResult<Picture>
    {
        public PictureExecutionResult() { }
        public PictureExecutionResult(Picture e) : base(e) { }
        public PictureExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "PictureResults")]
    public class PictureExecutionResults : BaseExecutionResults<Picture>
    {
        public PictureExecutionResults() { }
        public PictureExecutionResults(Picture[] e) : base(e) { }
        public PictureExecutionResults(Exception ex) : base(ex) { }
    }

}
