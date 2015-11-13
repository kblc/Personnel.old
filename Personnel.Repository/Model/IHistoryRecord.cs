using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Repository.Model
{
    /// <summary>
    /// History record interface
    /// </summary>
    public interface IHistoryRecord
    {
        /// <summary>
        /// Record identifier
        /// </summary>
        string SourceId { get; }

        /// <summary>
        /// History record source type
        /// </summary>
        string SourceName { get; }
    }
}
