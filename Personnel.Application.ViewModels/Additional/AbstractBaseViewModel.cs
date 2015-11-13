using Personnel.Application.ViewModels.AdditionalModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.Additional
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpr = propertyExpression.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException("propertyExpression should represent access to a member");
            string memberName = memberExpr.Member.Name;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public abstract class AbstractBaseViewModel : NotifyPropertyChangedBase, IDisposable
    {
        public AbstractBaseViewModel()
        {
            Init();
        }

        public System.Runtime.Remoting.Contexts.Context Context { get; } = Thread.CurrentContext;

        protected abstract void Init();

        private bool isLoaded = false;
        public bool IsLoaded { get { return isLoaded; } protected set { if (isLoaded == value) return; isLoaded = value; RaisePropertyChanged(() => IsLoaded); IsLoadedChanged?.Invoke(this, value); } }

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
