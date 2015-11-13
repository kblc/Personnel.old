using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using System.Runtime.Serialization;

namespace Personnel.Services.Model
{
    [DataContract(Name = "Result")]
    public class BaseExecutionResult
    {
#if DEBUG
        protected const bool IncludeStackTrace = true;
        protected const bool ClearText = false;
#else
        protected const bool IncludeStackTrace = false;
        protected const bool ClearText = true;
#endif

        public BaseExecutionResult() { }

        public BaseExecutionResult(Exception ex)
        {
            Exception = ex;
            Error = ex.GetExceptionText(includeStackTrace: false, clearText: true);
        }

        [DataMember(IsRequired = false)]
        public string Error { get; set; }

        [IgnoreDataMember]
        private Exception exception = null;

        [IgnoreDataMember]
        public Exception Exception { get { return exception; } set { exception = value; Error = exception?.GetExceptionText(includeStackTrace: IncludeStackTrace, clearText: ClearText); } }
    }

    [DataContract]
    public abstract class BaseExecutionResult<T> : BaseExecutionResult
    {
        public BaseExecutionResult() { }
        public BaseExecutionResult(T value) { Value = value; }
        public BaseExecutionResult(Exception ex) : base(ex) { }

        [DataMember(IsRequired = false)]
        public T Value { get; set; }
    }

    [DataContract]
    public abstract class BaseExecutionResults<T> : BaseExecutionResult
    {
        public BaseExecutionResults() { }
        public BaseExecutionResults(T[] values) { Values = values; }
        public BaseExecutionResults(Exception ex) : base(ex) { }

        [DataMember(IsRequired = false)]
        public T[] Values { get; set; }
    }
}
