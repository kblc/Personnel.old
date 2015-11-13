using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.AdditionalModels
{
    public class TempData<T> : Additional.NotifyPropertyChangedBase, IDisposable
        where T : class
    {
        private Timer timer = null;

        public TempData(TimeSpan timeToLive, T data)
        {
            TimeToLive = timeToLive;
            Data = data;
            timer = new Timer(TimerCallBack, new { Action = new Action(RaiseOnEnd), Context = Thread.CurrentContext }, TimeSpan.FromTicks(0), TimeToLive);
        }
        public TempData(T data) : this(TimeSpan.FromSeconds(5), data) { }
        public TempData() : this(null) { }

        private void TimerCallBack(object state)
        {
            timer?.Dispose();
            timer = null;

            var prm = (dynamic)state;

            var context = prm.Context as System.Runtime.Remoting.Contexts.Context;
            var action = prm.Action as Action;

            if (context != null && action != null)
                context.DoCallBack(() => action.Invoke());
        }

        private T data = default(T);
        public T Data
        {
            get { return data; }
            set
            {
                if (data == value)
                    return;
                data = value;
                RaisePropertyChanged(() => Data);
            }
        }
        internal TimeSpan TimeToLive { get; private set; }

        private void RaiseOnEnd()
        {
            OnEnd?.Invoke(this, new EventArgs());
        }
        public event EventHandler OnEnd;

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (timer != null)
                        timer.Dispose();
                    timer = null;
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
