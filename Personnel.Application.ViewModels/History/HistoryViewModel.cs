using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Helpers;

namespace Personnel.Application.ViewModels.History
{
    public class HistoryViewModel : DependencyObject
    {
        private readonly ServiceWorkers.HistoryWorker worker = new ServiceWorkers.HistoryWorker();

        #region Notifications

        public static readonly DependencyProperty NotificationsProperty = DependencyProperty.Register(nameof(Notifications), typeof(Notifications.NotificationsViewModel),
            typeof(HistoryViewModel), new PropertyMetadata(null, (s, e) => { }));

        public Notifications.NotificationsViewModel Notifications
        {
            get { return (Notifications.NotificationsViewModel)GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        #endregion
        #region IsLoaded

        private static readonly DependencyPropertyKey ReadOnlyIsLoadedPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsLoaded), typeof(bool), typeof(HistoryViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyIsLoadedProperty = ReadOnlyIsLoadedPropertyKey.DependencyProperty;

        public bool IsLoaded
        {
            get { return (bool)GetValue(ReadOnlyIsLoadedProperty); }
            private set { SetValue(ReadOnlyIsLoadedPropertyKey, value); }
        }

        #endregion
        #region Error

        private static readonly DependencyPropertyKey ReadOnlyErrorPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Error), typeof(string), typeof(HistoryViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyErrorProperty = ReadOnlyErrorPropertyKey.DependencyProperty;

        public string Error
        {
            get { return (string)GetValue(ReadOnlyErrorProperty); }
            private set { SetValue(ReadOnlyErrorPropertyKey, value); }
        }

        #endregion
        #region State

        private static readonly DependencyPropertyKey ReadOnlyStatePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(State), typeof(ServiceWorkers.WorkerState), typeof(HistoryViewModel),
                new FrameworkPropertyMetadata(ServiceWorkers.WorkerState.None,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyStateProperty = ReadOnlyErrorPropertyKey.DependencyProperty;

        public ServiceWorkers.WorkerState State
        {
            get { return (ServiceWorkers.WorkerState)GetValue(ReadOnlyStateProperty); }
            private set { SetValue(ReadOnlyStatePropertyKey, value); }
        }
        #endregion
        #region  ConnectionTimeInterval

        public static readonly DependencyProperty ConnectionTimeIntervalProperty = DependencyProperty.Register(nameof(ConnectionTimeInterval), typeof(TimeSpan),
            typeof(HistoryViewModel), new PropertyMetadata(TimeSpan.FromSeconds(ServiceWorkers.AbstractBaseWorker.DefaultConnectionTimeIntervalIsSeconds), (s, e) => 
            {
                var model = s as HistoryViewModel;
                if (model != null)
                    model.worker.ConnectionTimeInterval = (TimeSpan)e.NewValue;
            }));

        public TimeSpan ConnectionTimeInterval
        {
            get { return (TimeSpan)GetValue(ConnectionTimeIntervalProperty); }
            set { SetValue(ConnectionTimeIntervalProperty, value); }
        }
        #endregion
        #region ServiceCultureInfo

        public static readonly DependencyProperty ServiceCultureInfoProperty = DependencyProperty.Register(nameof(ServiceCultureInfo), typeof(CultureInfo),
            typeof(HistoryViewModel), new PropertyMetadata(System.Threading.Thread.CurrentThread.CurrentUICulture, (s, e) =>
            {
                var model = s as HistoryViewModel;
                if (model != null)
                    model.worker.ServiceCultureInfo = (CultureInfo)e.NewValue;
            }));

        public CultureInfo ServiceCultureInfo
        {
            get { return (CultureInfo)GetValue(ServiceCultureInfoProperty); }
            set { SetValue(ServiceCultureInfoProperty, value); }
        }
        #endregion
        #region IsConnecting

        private static readonly DependencyPropertyKey ReadOnlyIsConnectingPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsConnecting), typeof(bool), typeof(HistoryViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyIsConnectingProperty = ReadOnlyIsLoadedPropertyKey.DependencyProperty;

        public bool IsConnecting
        {
            get { return (bool)GetValue(ReadOnlyIsConnectingProperty); }
            private set { SetValue(ReadOnlyIsConnectingPropertyKey, value); }
        }

        #endregion
        #region IsWaiting

        private static readonly DependencyPropertyKey ReadOnlyIsWaitingPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsWaiting), typeof(bool), typeof(HistoryViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyIsWaitingProperty = ReadOnlyIsLoadedPropertyKey.DependencyProperty;

        public bool IsWaiting
        {
            get { return (bool)GetValue(ReadOnlyIsWaitingProperty); }
            private set { SetValue(ReadOnlyIsWaitingPropertyKey, value); }
        }

        #endregion
        #region IsActive

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool),
            typeof(HistoryViewModel), new PropertyMetadata(false, (s, e) => 
            {
                var model = s as HistoryViewModel;
                if (model != null && (bool)e.NewValue != (bool)e.OldValue)
                {
                    if ((bool)e.NewValue)
                        model.worker.Start();
                    else
                        model.worker.Stop();
                }
            }));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        #endregion

        public HistoryViewModel()
        {
            worker.CopyObjectTo(this);
            worker.OnErrorChanged += (s, e) => Error = e;
            worker.OnLoadedChanged += (s, e) => IsLoaded = e;
            worker.OnConnectingChanged += (s, e) => IsConnecting = e;
            worker.OnWaitingChanged += (s, e) => IsWaiting = e;
            worker.OnStateChanged += (s, e) => State = e;
            worker.OnNotification += (s, e) => Notifications?.Add(e);
            worker.Changed += OnHistoryChanged;
        }

        private void OnHistoryChanged(object sender, HistoryService.History e)
        {
            AsyncChanged?.Invoke(this, e);
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => Changed?.Invoke(this, e)));
        }

        public event EventHandler<HistoryService.History> AsyncChanged;
        public event EventHandler<HistoryService.History> Changed;
    }
}
