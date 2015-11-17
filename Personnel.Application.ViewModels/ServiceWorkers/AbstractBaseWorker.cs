using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.ServiceWorkers
{
    public abstract class AbstractBaseWorker : Additional.NotifyPropertyChangedBase, IDisposable
    {
        public AbstractBaseWorker()
        {
            Init();
        }

        public System.Runtime.Remoting.Contexts.Context Context { get; } = Thread.CurrentContext;

        protected abstract void Init();

        private bool isLoaded = false;
        public bool IsLoaded
        {
            get { return isLoaded; }
            protected set
            {
                if (isLoaded == value)
                    return;
                isLoaded = value;
                Context.DoCallBack(() =>
                {
                    RaisePropertyChanged(() => IsLoaded);
                    IsLoadedChanged?.Invoke(this, value);
                });
            }
        }

        private CultureInfo serviceCultureInfo = Thread.CurrentThread.CurrentUICulture;
        public CultureInfo ServiceCultureInfo { get { return serviceCultureInfo; } protected set { if (serviceCultureInfo == value) return; serviceCultureInfo = value; RaisePropertyChanged(() => ServiceCultureInfo); } }

        private TimeSpan connectionTimeInterval = TimeSpan.FromSeconds(5);
        public TimeSpan ConnectionTimeInterval { get { return connectionTimeInterval; } protected set { if (connectionTimeInterval == value) return; connectionTimeInterval = value; RaisePropertyChanged(() => ConnectionTimeInterval); } }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void OnDisposing() { }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDisposing();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public event EventHandler<bool> IsLoadedChanged;
    }
}
