﻿using Helpers;
using Helpers.WPF;
using Personnel.Application.ViewModels.AdditionalModels;
using Personnel.Application.ViewModels.ServiceWorkers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Personnel.Application.ViewModels.Vacation
{
    public class VacationViewModel : DependencyObject, INotifyPropertyChanged
    {
        private const string MANAGEVACATION = "MANAGEVACATION";

        private readonly ServiceWorkers.VacationWorker worker = new ServiceWorkers.VacationWorker();

        private NotifyCollection<VacationService.VacationLevel> levels = new NotifyCollection<VacationService.VacationLevel>();
        public IReadOnlyNotifyCollection<VacationService.VacationLevel> Levels => levels;

        private NotifyCollection<VacationService.Vacation> vacations = new NotifyCollection<VacationService.Vacation>();
        public IReadOnlyNotifyCollection<VacationService.Vacation> Vacations => vacations;

        private NotifyCollection<VacationService.VacationBalance> vacationBalances = new NotifyCollection<VacationService.VacationBalance>();
        public IReadOnlyNotifyCollection<VacationService.VacationBalance> VacationBalances => vacationBalances;

        #region Notifications

        public static readonly DependencyProperty NotificationsProperty = DependencyProperty.Register(nameof(Notifications), typeof(Notifications.NotificationsViewModel),
            typeof(VacationViewModel), new PropertyMetadata(null, (s, e) => { }));

        public Notifications.NotificationsViewModel Notifications
        {
            get { return (Notifications.NotificationsViewModel)GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        #endregion
        #region History

        public static readonly DependencyProperty HistoryProperty = DependencyProperty.Register(nameof(History), typeof(History.HistoryViewModel),
            typeof(VacationViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as VacationViewModel;
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
        #region Staffing

        public static readonly DependencyProperty StaffingProperty = DependencyProperty.Register(nameof(Staffing), typeof(Staffing.StaffingViewModel),
            typeof(VacationViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as VacationViewModel;
                var staffingNewvalue = e.NewValue as Staffing.StaffingViewModel;
                var staffingOldvalue = e.OldValue as Staffing.StaffingViewModel;
                if (model != null)
                {
                    if (staffingOldvalue != null) {
                        staffingOldvalue.OnRightsChanged -= model.OnRightsChanged;
                        staffingOldvalue.OnCurrentChanged -= model.OnCurrentChanged;
                    }
                    if (staffingNewvalue != null)
                    {
                        staffingNewvalue.OnRightsChanged += model.OnRightsChanged;
                        staffingNewvalue.OnCurrentChanged += model.OnCurrentChanged;
                    }
                }
            }));

        public Staffing.StaffingViewModel Staffing
        {
            get { return (Staffing.StaffingViewModel)GetValue(StaffingProperty); }
            set { SetValue(StaffingProperty, value); }
        }

        #endregion
        #region From

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(nameof(From), typeof(DateTime?),
            typeof(VacationViewModel), new PropertyMetadata(null, (s, e) => {
                var model = (VacationViewModel)s;
                var dateNewValue = (DateTime?)e.NewValue;
                model.worker.From = dateNewValue;
            }));

        public DateTime? From
        {
            get { return (DateTime?)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        #endregion
        #region To

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(nameof(To), typeof(DateTime?),
            typeof(VacationViewModel), new PropertyMetadata(null, (s, e) => {
                var model = (VacationViewModel)s;
                var dateNewValue = (DateTime?)e.NewValue;
                model.worker.To = dateNewValue;
            }));

        public DateTime? To
        {
            get { return (DateTime?)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        #endregion
        #region IsLoaded

        private static readonly DependencyPropertyKey ReadOnlyIsLoadedPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsLoaded), typeof(bool), typeof(VacationViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) =>
                    {
                        var model = s as VacationViewModel;
                        if (model != null)
                            model.RaiseOnIsLoadedChanged((bool)e.NewValue);
                    })));
        public static readonly DependencyProperty ReadOnlyIsLoadedProperty = ReadOnlyIsLoadedPropertyKey.DependencyProperty;

        public bool IsLoaded
        {
            get { return (bool)GetValue(ReadOnlyIsLoadedProperty); }
            private set { SetValue(ReadOnlyIsLoadedPropertyKey, value); RaiseOnIsLoadedChanged(value); }
        }

        #endregion
        #region Error

        private static readonly DependencyPropertyKey ReadOnlyErrorPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Error), typeof(string), typeof(VacationViewModel),
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
            = DependencyProperty.RegisterReadOnly(nameof(State), typeof(ServiceWorkers.WorkerState), typeof(VacationViewModel),
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
            typeof(VacationViewModel), new PropertyMetadata(TimeSpan.FromSeconds(ServiceWorkers.AbstractBaseWorker.DefaultConnectionTimeIntervalIsSeconds), (s, e) =>
            {
                var model = s as VacationViewModel;
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
            typeof(VacationViewModel), new PropertyMetadata(System.Threading.Thread.CurrentThread.CurrentUICulture, (s, e) =>
            {
                var model = s as VacationViewModel;
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
            typeof(VacationViewModel), new PropertyMetadata(false, (s, e) =>
            {
                var model = s as VacationViewModel;
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
            typeof(VacationViewModel), new PropertyMetadata(true, (s, e) =>
            {
                var model = s as VacationViewModel;
                if (model != null)
                    model.RaisePropertyChanged(e.Property.Name);
            }));

        public bool IsDebugView
        {
            get { return (bool)GetValue(IsDebugViewProperty); }
            set { SetValue(IsDebugViewProperty, value); RaisePropertyChanged(); }
        }
        #endregion
        #region CanManageVacations

        private static readonly DependencyPropertyKey ReadOnlyCanManageVacationsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(CanManageVacations), typeof(bool), typeof(VacationViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) =>
                    {
                        var model = s as VacationViewModel;
                        if (model != null)
                        {
                            model.RaisePropertyChanged(e.Property.Name);
                            model.UpdateCommands();
                        }
                    })));
        public static readonly DependencyProperty ReadOnlyCanManageVacationsProperty = ReadOnlyCanManageVacationsPropertyKey.DependencyProperty;

        public bool CanManageVacations
        {
            get { return (bool)GetValue(ReadOnlyCanManageVacationsProperty); }
            protected set { SetValue(ReadOnlyCanManageVacationsPropertyKey, value); RaisePropertyChanged(); UpdateCommands(); }
        }

        private bool GetCanManageVacationsProperty()
        {
            var res = false;
            if (Staffing != null && Staffing.Current != null)
            {
                var canManageRight = Staffing.Rights.FirstOrDefault(r => string.Compare(r.SystemName, MANAGEVACATION, true) == 0);
                if (canManageRight != null)
                    res = Staffing.Current.Rights.Any(r => r.RightId == canManageRight.Id);
            }
            return res;
        }

        #endregion
        #region Commands

        private void UpdateCommands()
        {
            insertVacationCommand?.RaiseCanExecuteChanged();
        }

        private DelegateCommand insertVacationCommand = null;
        public ICommand InsertVacationCommand { get { return insertVacationCommand ?? (insertVacationCommand = new DelegateCommand(o => InsertVacation())); } }

        private void InsertVacation()
        {
            //var newDep = new DepartmentEditViewModel(true)
            //{
            //    Data = new DepartmentAndStaffingData(this)
            //    {
            //        Department = new Department()
            //        {
            //            ParentId = null,
            //            Name = Properties.Resources.DEPARTMENTEDIT_NewDepartmentName,
            //        }
            //    },
            //    Parent = this,
            //    Owner = this,
            //    IsSelected = true,
            //};
            //departments.Add(newDep);
        }

        #endregion

        private void RunUnderDispatcher(Delegate a)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, a);
        }

        public VacationViewModel()
        {
            worker.CopyObjectTo(this);
            worker.OnErrorChanged += (s, e) => RunUnderDispatcher(new Action(() => Error = e));
            worker.OnLoadedChanged += (s, e) => RunUnderDispatcher(new Action(() => IsLoaded = e));
            worker.OnStateChanged += (s, e) => RunUnderDispatcher(new Action(() => State = e));
            worker.OnNotification += (s, e) => RunUnderDispatcher(new Action(() => Notifications?.Add(e)));
            worker.OnVacationLevelChanged += (s,e) => RunUnderDispatcher(new Action(() => OnWorkerVacationLevelChanged(s, e)));
            worker.OnVacationBalanceChanged += (s, e) => RunUnderDispatcher(new Action(() => OnWorkerVacationBalanceChanged(s, e)));
            worker.OnVacationChanged += (s, e) => RunUnderDispatcher(new Action(() => OnWorkerVacationChanged(s, e)));
            CanManageVacations = GetCanManageVacationsProperty();
        }

        private void OnHistoryChanged(object sender, HistoryService.History e) => worker.ApplyHistoryChanges(e);

        private void OnRightsChanged(object semder, ListItemsEventArgs<StaffingService.Right> e)
        {
            CanManageVacations = GetCanManageVacationsProperty();
        }
        private void OnCurrentChanged(object semder, StaffingService.Employee e)
        {
            CanManageVacations = GetCanManageVacationsProperty();
        }

        private void OnWorkerVacationLevelChanged(object sender, ListItemsEventArgs<VacationService.VacationLevel> e)
        {
            if (new[] { ChangeAction.Add, ChangeAction.Change }.Contains(e.Action))
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedLevel = levels.FirstOrDefault(l => l.Id == d.Id);
                        if (existedLevel != null)
                        {
                            existedLevel.CopyObjectFrom(d);
                        }
                        else
                        {
                            levels.Add(d);
                        }
                    }
                }
            } else
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedLevel = levels.FirstOrDefault(l => l.Id == d.Id);
                        if (existedLevel != null)
                            levels.Remove(existedLevel);
                    }
                }
            }

            OnVacationLevelChanged?.Invoke(this, e);
        }
        private void OnWorkerVacationBalanceChanged(object sender, ListItemsEventArgs<VacationService.VacationBalance> e)
        {
            if (new[] { ChangeAction.Add, ChangeAction.Change }.Contains(e.Action))
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedLevel = vacationBalances.FirstOrDefault(l => l.Id == d.Id);
                        if (existedLevel != null)
                        {
                            existedLevel.CopyObjectFrom(d);
                        }
                        else
                        {
                            vacationBalances.Add(d);
                        }
                    }
                }
            }
            else
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedLevel = vacationBalances.FirstOrDefault(l => l.Id == d.Id);
                        if (existedLevel != null)
                            vacationBalances.Remove(existedLevel);
                    }
                }
            }

            OnVacationBalanceChanged?.Invoke(this, e);
        }
        private void OnWorkerVacationChanged(object sender, ListItemsEventArgs<VacationService.Vacation> e)
        {
            if (new[] { ChangeAction.Add, ChangeAction.Change }.Contains(e.Action))
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedVacation = vacations.FirstOrDefault(l => l.Id == d.Id);
                        if (existedVacation != null)
                        {
                            existedVacation.CopyObjectFrom(d);
                        }
                        else
                        {
                            vacations.Add(d);
                        }
                    }
                }
            }
            else
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedVacation = vacations.FirstOrDefault(l => l.Id == d.Id);
                        if (existedVacation != null)
                            vacations.Remove(existedVacation);
                    }
                }
            }

            OnVacationChanged?.Invoke(this, e);
        }

        private void RaiseOnIsLoadedChanged(bool value)
        {
            RunUnderDispatcher(new Action(() => OnIsLoadedChanged?.Invoke(this, value)));
        }

        public event EventHandler<bool> OnIsLoadedChanged;
        public event EventHandler<ListItemsEventArgs<VacationService.VacationLevel>> OnVacationLevelChanged;
        public event EventHandler<ListItemsEventArgs<VacationService.VacationBalance>> OnVacationBalanceChanged;
        public event EventHandler<ListItemsEventArgs<VacationService.Vacation>> OnVacationChanged;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([ParenthesizePropertyName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}