using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using Personnel.Application.ViewModels.Additional;
using System.Runtime.CompilerServices;

namespace Personnel.Application.ViewModels.ServiceWorkers
{
    public class NotificationItem : NotifyPropertyChangedBase
    {
        private DateTime created;
        public DateTime Created { get { return created; } protected set { if (created == value) return; created = value; RaisePropertyChanged(); } }

        private string header;
        public string Header { get { return header; } protected set { if (header == value) return; header = value; RaisePropertyChanged(); } }

        private string message;
        public string Message { get { return message; } protected set { if (message == value) return; message = value; RaisePropertyChanged(); } }

        private bool isError;
        public bool IsError { get { return isError; } protected set { if (isError == value) return; isError = value; RaisePropertyChanged(); } }

        private string iconUrl;
        public string IconUrl { get { return iconUrl; } protected set { if (iconUrl == value) return; iconUrl = value; RaisePropertyChanged(); } }

        public NotificationItem(string header, string message, bool isError, string iconUrl, DateTime created)
        {
            this.Header = header;
            this.Message = message;
            this.IsError = isError;
            this.IconUrl = iconUrl;
            this.Created = created;
        }
        public NotificationItem(string header, string message, bool isError, string iconUrl)
            : this(header, message, isError, iconUrl, DateTime.UtcNow)
        { }
    }

    public enum WorkerState
    {
        None,
        Error,
        Started,
        Stoped,
    }

    public abstract class AbstractBaseWorker : Additional.NotifyPropertyChangedBase, IDisposable
    {
        static AbstractBaseWorker()
        {
            Automapper.Init();
        }

        public const int DefaultConnectionTimeIntervalIsSeconds = 5;

        public System.Runtime.Remoting.Contexts.Context Context { get; } = Thread.CurrentContext;

        protected override void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            Context.DoCallBack(() => base.RaisePropertyChanged(propertyName));
        }
        protected override void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            Context.DoCallBack(() => base.RaisePropertyChanged<T>(propertyExpression));
        }

        protected abstract bool DoStart();
        protected abstract bool DoStop();

        public void Start()
        {
            if (State == WorkerState.Started)
                return;

            State = (DoStart()) ? WorkerState.Started : WorkerState.Error;
        }
        public void Stop()
        {
            if (State == WorkerState.Stoped)
                return;

            State = (DoStop()) ? WorkerState.Stoped : WorkerState.Error;
        }

        private WorkerState state = WorkerState.None;
        public WorkerState State
        {
            get { return state; }
            protected set
            {
                if (state == value)
                    return;
                state = value;
                RaisePropertyChanged();
                RaiseOnStateChanged();
            }
        }

        private bool isLoaded = false;
        public bool IsLoaded
        {
            get { return isLoaded; }
            protected set
            {
                if (isLoaded == value)
                    return;
                isLoaded = value;
                RaisePropertyChanged();
                RaiseOnLoadedChanged();
            }
        }

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
            private set
            {
                if (error == value)
                    return;

                error = value;
                RaisePropertyChanged();
                RaiseOnErrorChanged();
            }
        }

        internal static string IconUrlByType(Type type, CultureInfo cultureInfo)
            => Properties.Resources.ResourceManager.GetString($"{type.Name.ToUpper()}_NOTIFICATIONICONURL", cultureInfo);

        internal static string GetExceptionText(Exception ex) => ex.GetExceptionText();

        protected void SetError(string ex)
        {
            Error = string.IsNullOrWhiteSpace(ex) ? null : ex;
        }
        protected void SetError(Exception ex)
        {
            SetError(GetExceptionText(ex));
        }

        private CultureInfo serviceCultureInfo = Thread.CurrentThread.CurrentUICulture;
        public CultureInfo ServiceCultureInfo
        {
            get { return serviceCultureInfo; }
            set
            {
                if (serviceCultureInfo == value)
                    return;

                if (value == null)
                    throw new ArgumentNullException(nameof(ServiceCultureInfo));

                serviceCultureInfo = value;
                RaisePropertyChanged();
            }
        }

        private TimeSpan connectionTimeInterval = TimeSpan.FromSeconds(DefaultConnectionTimeIntervalIsSeconds);
        public TimeSpan ConnectionTimeInterval
        {
            get { return connectionTimeInterval; }
            set
            {
                if (connectionTimeInterval == value)
                    return;
                connectionTimeInterval = value;
                RaisePropertyChanged();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void OnDisposing() { }

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (State == WorkerState.Started)
                        DoStop();
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

        private void RaiseOnLoadedChanged()
        {
            RaiseOnNotification(
            new NotificationItem(
                header: Properties.Resources.NOTIFICATION,
                message: IsLoaded ? Properties.Resources.LOADED : Properties.Resources.UNLOADED,
                isError: false,
                iconUrl: IconUrlByType(GetType(), ServiceCultureInfo)
            ));
            Context.DoCallBack(() => OnLoadedChanged?.Invoke(this, IsLoaded));
        }
        private void RaiseOnErrorChanged()
        {
            if (!string.IsNullOrWhiteSpace(Error))
                RaiseOnNotification(
                    new NotificationItem(
                        header: Properties.Resources.ERROR,
                        message: Error,
                        isError: true,
                        iconUrl: IconUrlByType(GetType(), ServiceCultureInfo)
                    ));
            Context.DoCallBack(() => OnErrorChanged?.Invoke(this, Error));
        }
        private void RaiseOnStateChanged()
        {
            if (new[] { WorkerState.Started, WorkerState.Stoped }.Contains(this.State))
                RaiseOnNotification(
                    new NotificationItem(
                        header: Properties.Resources.NOTIFICATION,
                        message: State == WorkerState.Started ? Properties.Resources.STARTED : Properties.Resources.STOPED,
                        isError: false,
                        iconUrl: IconUrlByType(GetType(), ServiceCultureInfo)
                    ));
            Context.DoCallBack(() => OnStateChanged?.Invoke(this, State));
        }
        protected void RaiseOnNotification(NotificationItem notification) => Context.DoCallBack(() => OnNotification?.Invoke(this, notification));

        public event EventHandler<bool> OnLoadedChanged;
        public event EventHandler<string> OnErrorChanged;
        public event EventHandler<WorkerState> OnStateChanged;
        public event EventHandler<NotificationItem> OnNotification;
    }
}
