using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Helpers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Helpers.WPF;
using System.Windows.Input;
using Personnel.Application.ViewModels.AdditionalModels;

namespace Personnel.Application.ViewModels.Notifications
{
    public class NotificationsViewModel : DependencyObject
    {
        private readonly ServiceWorkers.NotificationsWorker worker = new ServiceWorkers.NotificationsWorker();

        private NotifyCollection<NotificationViewModel> notifications = new NotifyCollection<NotificationViewModel>();
        public IReadOnlyNotifyCollection<NotificationViewModel> Notifications => notifications;

        #region SelectedNotification

        public static readonly DependencyProperty SelectedNotificationProperty = DependencyProperty.Register(nameof(SelectedNotification), typeof(NotificationViewModel),
            typeof(NotificationsViewModel), new PropertyMetadata(null, (s, e) => { }));

        public NotificationViewModel SelectedNotification
        {
            get { return (NotificationViewModel)GetValue(SelectedNotificationProperty); }
            private set { SetValue(SelectedNotificationProperty, value); }
        }

        #endregion
        #region IsLoaded

        private static readonly DependencyPropertyKey ReadOnlyIsLoadedPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsLoaded), typeof(bool), typeof(NotificationsViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s,e) => { })));
        public static readonly DependencyProperty ReadOnlyIsLoadedProperty = ReadOnlyIsLoadedPropertyKey.DependencyProperty;

        public bool IsLoaded
        {
            get { return (bool)GetValue(ReadOnlyIsLoadedProperty); }
            private set { SetValue(ReadOnlyIsLoadedPropertyKey, value); }
        }

        #endregion
        #region Error

        private static readonly DependencyPropertyKey ReadOnlyErrorPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Error), typeof(string), typeof(NotificationsViewModel),
                new FrameworkPropertyMetadata(string.Empty,
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
            = DependencyProperty.RegisterReadOnly(nameof(State), typeof(ServiceWorkers.WorkerState), typeof(NotificationsViewModel),
                new FrameworkPropertyMetadata(ServiceWorkers.WorkerState.None,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyStateProperty = ReadOnlyStatePropertyKey.DependencyProperty;

        public ServiceWorkers.WorkerState State
        {
            get { return (ServiceWorkers.WorkerState)GetValue(ReadOnlyStateProperty); }
            private set { SetValue(ReadOnlyStatePropertyKey, value); }
        }
        #endregion
        #region IsActive

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool),
            typeof(NotificationsViewModel), new PropertyMetadata(false, (s, e) =>
            {
                var model = s as NotificationsViewModel;
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
        #region ConnectionTimeInterval

        public static readonly DependencyProperty ConnectionTimeIntervalProperty = DependencyProperty.Register(nameof(ConnectionTimeInterval), typeof(TimeSpan),
            typeof(NotificationsViewModel), new PropertyMetadata(TimeSpan.FromSeconds(ServiceWorkers.NotificationsWorker.NotificationLifeTimeInSeconds), (s, e) =>
            {
                var model = s as NotificationsViewModel;
                if (model != null)
                    model.worker.ConnectionTimeInterval = (TimeSpan)e.NewValue;
            }));

        public TimeSpan ConnectionTimeInterval
        {
            get { return (TimeSpan)GetValue(ConnectionTimeIntervalProperty); }
            set { SetValue(ConnectionTimeIntervalProperty, value); }
        }
        #endregion
        #region MaxNotificationsCount

        public static readonly DependencyProperty MaxNotificationsCountProperty = DependencyProperty.Register(nameof(MaxNotificationsCount), typeof(int),
            typeof(NotificationsViewModel), new PropertyMetadata(ServiceWorkers.NotificationsWorker.MaxNotificationCount, (s, e) =>
            {
                var model = s as NotificationsViewModel;
                if (model != null)
                    model.worker.MaxNotifications = (int)e.NewValue;
            }));

        public int MaxNotificationsCount
        {
            get { return (int)GetValue(MaxNotificationsCountProperty); }
            set { SetValue(MaxNotificationsCountProperty, value); }
        }
        #endregion
        #region Clear selection command

        private DelegateCommand clearSelectionCommand = null;
        public ICommand ClearSelectionCommand { get { return clearSelectionCommand ?? (clearSelectionCommand = new DelegateCommand((o) => SelectedNotification = null)); } }

        #endregion

        public NotificationsViewModel()
        {
            worker.CopyObjectTo(this);
            InitCollection();
            worker.OnErrorChanged += (s, e) => RunUnderDispatcher(new Action(() => Error = e));
            worker.OnLoadedChanged += (s, e) => RunUnderDispatcher(new Action(() => IsLoaded = e));
            worker.OnStateChanged += (s, e) => RunUnderDispatcher(new Action(() => State = e));
            worker.OnMaxNotificationsChanged += (s, e) => RunUnderDispatcher(new Action(() => MaxNotificationsCount = e));
            worker.OnChanged += OnNotificationsChanged;
        }

        private void RunUnderDispatcher(Delegate a)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, a);
        }

        private NotificationViewModel FromItem(ServiceWorkers.NotificationItem item)
        {
            var n = new NotificationViewModel(item);
            n.OnCloseClick += (s, e) => Remove(((NotificationViewModel)s).Notification);
            n.OnOpenClick += (s, e) => SelectedNotification = (NotificationViewModel)s;

            return n;
        }

        private void InitCollection()
        {
            var initItems = worker
                .Get()
                .Select(i => FromItem(i))
                .ToArray();
            RunUnderDispatcher(new Action(() =>
            {
                foreach (var item in initItems)
                    notifications.Add(item);
            }));
        }
        private void OnNotificationsChanged(object sender, ServiceWorkers.NotificationItemEventArgs e)
        {
            if (e.Action == ServiceWorkers.NotificationListAction.Add)
            {
                RunUnderDispatcher(new Action(() => 
                    e.Items.Select(i => FromItem(i))
                        .ToList()
                        .ForEach(i => notifications.Add(i)))
                );
            }
            else if (e.Action == ServiceWorkers.NotificationListAction.Remove)
            {
                RunUnderDispatcher(new Action(() =>
                {
                    if (SelectedNotification != null && e.Items.Contains(SelectedNotification.Notification, ServiceWorkers.NotificationsWorker.GenericComparer))
                        SelectedNotification = null;

                    e.Items.Join(notifications, i => i, n => n.Notification, (i, n) => n, ServiceWorkers.NotificationsWorker.GenericComparer)
                         .ToList()
                         .ForEach(i => notifications.Remove(i));
                }));
            }
        }

        public void Add(IEnumerable<ServiceWorkers.NotificationItem> items) => worker.Add(items);
        public void Add(ServiceWorkers.NotificationItem item) => worker.Add(item);
        public void Remove(IEnumerable<ServiceWorkers.NotificationItem> items) => worker.Remove(items);
        public void Remove(ServiceWorkers.NotificationItem item) => worker.Remove(item);
    }
}
