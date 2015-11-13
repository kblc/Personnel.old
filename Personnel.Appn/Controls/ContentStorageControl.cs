using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.WPF;

namespace Personnel.Appn.Controls
{
    public class ContentStorageControl : Helpers.WPF.PropertyChangedBase
    {
        private string headerName = null;
        public string HeaderName { get { return headerName; } set { if (headerName == value) return; headerName = value; RaisePropertyChange(nameof(HeaderName)); } }

        private object header = null;
        public object Header { get { return header; } set { if (header == value) return; header = value; RaisePropertyChange(nameof(Header)); } }

        private object content = null;
        public object Content { get { return content; } set { if (content == value) return; content = value; RaisePropertyChange(nameof(Content)); } }
    }
}
