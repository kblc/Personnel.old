using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.File
{
    [DataContract(Name = "FileResult")]
    public class FileExecutionResult : Model.BaseExecutionResult<Model.File>
    {
        public FileExecutionResult() { }
        public FileExecutionResult(Model.File value) : base(value) { }
        public FileExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "FileResults")]
    public class FileExecutionResults : Model.BaseExecutionResults<Model.File>
    {
        public FileExecutionResults() { }
        public FileExecutionResults(Model.File[] values) : base(values) { }
        public FileExecutionResults(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "PictureResult")]
    public class PictureExecutionResult : Model.BaseExecutionResult<Model.Picture>
    {
        public PictureExecutionResult() { }
        public PictureExecutionResult(Model.Picture value) : base(value) { }
        public PictureExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "PictureResults")]
    public class PictureExecutionResults : Model.BaseExecutionResults<Model.Picture>
    {
        public PictureExecutionResults() { }
        public PictureExecutionResults(Model.Picture[] values) : base(values) { }
        public PictureExecutionResults(Exception ex) : base(ex) { }
    }
}
