using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Right
    /// </summary>
    [DataContract]
    public class Right
    {
        /// <summary>
        /// Right system name
        /// </summary>
        [DataMember(IsRequired = true)]
        public long Id { get; set; }
        /// <summary>
        /// Right system name
        /// </summary>
        [DataMember(IsRequired = true)]
        public string SystemName { get; set; }

        /// <summary>
        /// Right description
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Name { get; set; }
    }

    [DataContract(Name = "RightValueResults")]
    public class RightValueExecutionResults : BaseExecutionResults<long>
    {
        public RightValueExecutionResults() { }
        public RightValueExecutionResults(long[] e) : base(e) { }
        public RightValueExecutionResults(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "RightResults")]
    public class RightExecutionResults : BaseExecutionResults<Right>
    {
        public RightExecutionResults() { }
        public RightExecutionResults(Right[] e) : base(e) { }
        public RightExecutionResults(Exception ex) : base(ex) { }
    }
}
