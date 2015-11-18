using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Helpers;
using System.Collections.ObjectModel;
using Personnel.Application.ViewModels.AdditionalModels;
using Personnel.Application.ViewModels.ServiceWorkers;
using Personnel.Application.ViewModels.StaffingService;

namespace Personnel.Application.ViewModels.Staffing
{
    public class StaffingViewModel : DependencyObject
    {
        private const string MANAGEDEPARTMENTS = "MANAGEDEPARTMENTS";

        private readonly ServiceWorkers.StaffingWorker worker = new ServiceWorkers.StaffingWorker();

        private ObservableCollection<StaffingService.Right> rights = new ObservableCollection<StaffingService.Right>();
        public IReadOnlyNotifyCollection<StaffingService.Right> Rights => (IReadOnlyNotifyCollection<StaffingService.Right>)rights;

        #region Notifications

        public static readonly DependencyProperty NotificationsProperty = DependencyProperty.Register(nameof(Notifications), typeof(Notifications.NotificationsViewModel),
            typeof(StaffingViewModel), new PropertyMetadata(null, (s, e) => { }));

        public Notifications.NotificationsViewModel Notifications
        {
            get { return (Notifications.NotificationsViewModel)GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        #endregion
        #region History

        public static readonly DependencyProperty HistoryProperty = DependencyProperty.Register(nameof(History), typeof(History.HistoryViewModel),
            typeof(StaffingViewModel), new PropertyMetadata(null, (s, e) => 
            {
                var model = s as StaffingViewModel;
                var historyNewvalue = e.NewValue as History.HistoryViewModel;
                var historyOldvalue = e.OldValue as History.HistoryViewModel;
                if (model != null)
                {
                    if (historyOldvalue != null)
                        historyOldvalue.AsyncChanged -= model.OnHistoryChanged;
                    if (historyNewvalue != null)
                        historyNewvalue.AsyncChanged += model.OnHistoryChanged;
                }
            }));

        public History.HistoryViewModel History
        {
            get { return (History.HistoryViewModel)GetValue(HistoryProperty); }
            set { SetValue(HistoryProperty, value); }
        }

        #endregion
        #region IsLoaded

        private static readonly DependencyPropertyKey ReadOnlyIsLoadedPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsLoaded), typeof(bool), typeof(StaffingViewModel),
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
            = DependencyProperty.RegisterReadOnly(nameof(Error), typeof(string), typeof(StaffingViewModel),
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
            = DependencyProperty.RegisterReadOnly(nameof(State), typeof(ServiceWorkers.WorkerState), typeof(StaffingViewModel),
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
        #region ConnectionTimeInterval

        public static readonly DependencyProperty ConnectionTimeIntervalProperty = DependencyProperty.Register(nameof(ConnectionTimeInterval), typeof(TimeSpan),
            typeof(StaffingViewModel), new PropertyMetadata(TimeSpan.FromSeconds(ServiceWorkers.AbstractBaseWorker.DefaultConnectionTimeIntervalIsSeconds), (s, e) =>
            {
                var model = s as StaffingViewModel;
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
            typeof(StaffingViewModel), new PropertyMetadata(System.Threading.Thread.CurrentThread.CurrentUICulture, (s, e) =>
            {
                var model = s as StaffingViewModel;
                if (model != null)
                    model.worker.ServiceCultureInfo = (CultureInfo)e.NewValue;
            }));

        public CultureInfo ServiceCultureInfo
        {
            get { return (CultureInfo)GetValue(ServiceCultureInfoProperty); }
            set { SetValue(ServiceCultureInfoProperty, value); }
        }
        #endregion
        #region IsActive

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool),
            typeof(StaffingViewModel), new PropertyMetadata(false, (s, e) =>
            {
                var model = s as StaffingViewModel;
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
        #region IsDebugView

        public static readonly DependencyProperty IsDebugViewProperty = DependencyProperty.Register(nameof(IsDebugView), typeof(bool),
            typeof(StaffingViewModel), new PropertyMetadata(true, (s, e) => { }));

        public bool IsDebugView
        {
            get { return (bool)GetValue(IsDebugViewProperty); }
            set { SetValue(IsDebugViewProperty, value); }
        }
        #endregion
        #region CanManage

        private static readonly DependencyPropertyKey ReadOnlyCanManagePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(CanManage), typeof(bool), typeof(StaffingViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyCanManageProperty = ReadOnlyIsLoadedPropertyKey.DependencyProperty;

        public bool CanManage
        {
            get { return (bool)GetValue(ReadOnlyCanManageProperty); }
            private set { SetValue(ReadOnlyCanManagePropertyKey, value); }
        }

        private bool GetCanManageProperty()
        {
            var res = false;
            if (Rights != null && Current != null)
            {
                var canManageRight = Rights.FirstOrDefault(r => r.SystemName == MANAGEDEPARTMENTS);
                if (canManageRight != null)
                    res = Current.Rights.Any(r => r.RightId == canManageRight.Id);
            }
            return res;
        }

        #endregion
        #region Current

        private static readonly DependencyPropertyKey ReadOnlyCurrentPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Current), typeof(StaffingService.Employee), typeof(StaffingViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyCurrentProperty = ReadOnlyCurrentPropertyKey.DependencyProperty;

        public StaffingService.Employee Current
        {
            get { return (StaffingService.Employee)GetValue(ReadOnlyCurrentProperty); }
            private set { SetValue(ReadOnlyCurrentPropertyKey, value); }
        }

        #endregion

        private void OnHistoryChanged(object sender, HistoryService.History e) => worker.ApplyHistoryChanges(e);

        public StaffingViewModel()
        {
            worker.CopyObjectTo(this);
            worker.OnErrorChanged += (s, e) => Error = e;
            worker.OnLoadedChanged += (s, e) => IsLoaded = e;
            worker.OnStateChanged += (s, e) => State = e;
            worker.OnNotification += (s, e) => Notifications?.Add(e);
            worker.OnCurrentChanged += (s, e) =>
            {
                Current = e;
                CanManage = GetCanManageProperty();
            };
            worker.OnRightsChanged += (s, e) =>
            {
                OnRightsChanged(s, e);
                CanManage = GetCanManageProperty();
            };

            //worker.OnCurrentChanged += (s,e) => 
        }

        private void RunUnderDispatcher(Delegate a)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, a);
        }

        private void OnRightsChanged(object s, StaffingListItemsEventArgs<Right> e)
        {
            if (e.Action == StaffingListsAction.Add)
                RunUnderDispatcher(new Action(() => e.Items.ToList().ForEach(i => rights.Add(i))));
            else if (e.Action == StaffingListsAction.Change)
                RunUnderDispatcher(new Action(() => e.Items.Join(rights, i => i.Id, n => n.Id, (i, n) => new { New = i, Old = n }).ToList().ForEach(i => i.New.CopyObjectTo(i.Old))));
            else if (e.Action == StaffingListsAction.Remove)
                RunUnderDispatcher(new Action(() => e.Items.Join(rights, i => i.Id, n => n.Id, (i, n) => n).ToList().ForEach(i => rights.Remove(i))));
        }
    }
}
