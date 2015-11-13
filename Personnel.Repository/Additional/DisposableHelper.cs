using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Repository.Additional
{
    internal class DisposableHelper : IDisposable
    {
        public DisposableHelper(Action onDisposeAction = null)
        {
            if (onDisposeAction != null)
                this.OnDispose += (s, e) => { onDisposeAction(); };
        }

        public event EventHandler OnDispose;

        public void Dispose()
        {
            var e = OnDispose;
            if (e != null)
                e(this, new EventArgs());
        }
    }

}
