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
using System.ComponentModel;
using System.Windows.Input;
using Helpers.WPF;
using Helpers.Linq;

namespace Personnel.Application.ViewModels.Staffing
{
    public class StaffingViewModel : DependencyObject, ITreeDepartmentItem, ITreeOwner<DepartmentEditViewModel>
    {
        private const string MANAGEDEPARTMENTS = "MANAGEDEPARTMENTS";

        private readonly ServiceWorkers.StaffingWorker worker = new ServiceWorkers.StaffingWorker();

        private NotifyCollection<Right> rights = new NotifyCollection<Right>();
        public IReadOnlyNotifyCollection<Right> Rights => rights;

        private NotifyCollection<ITreeItem<DepartmentEditViewModel, Department>> departments = new NotifyCollection<ITreeItem<DepartmentEditViewModel, Department>>();
        public IReadOnlyNotifyCollection<ITreeItem<DepartmentEditViewModel, Department>> Departments => departments;

        private NotifyCollection<Employee> employees = new NotifyCollection<Employee>();
        public IReadOnlyNotifyCollection<Employee> Employees => employees;

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
            typeof(StaffingViewModel), new PropertyMetadata(true, (s, e) => 
            {
                var model = s as StaffingViewModel;
                if (model != null)
                    model.RaisePropertyChanged(e.Property.Name);
            }));

        public bool IsDebugView
        {
            get { return (bool)GetValue(IsDebugViewProperty); }
            set { SetValue(IsDebugViewProperty, value); RaisePropertyChanged(); }
        }
        #endregion
        #region CanManageDepartments

        private static readonly DependencyPropertyKey ReadOnlyCanManageDepartmentsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(CanManageDepartments), typeof(bool), typeof(StaffingViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => 
                    {
                        var model = s as StaffingViewModel;
                        if (model != null)
                        { 
                            model.RaisePropertyChanged(e.Property.Name);
                            model.UpdateCommands();
                        }
                    })));
        public static readonly DependencyProperty ReadOnlyCanManageDepartmentsProperty = ReadOnlyCanManageDepartmentsPropertyKey.DependencyProperty;

        public bool CanManageDepartments
        {
            get { return (bool)GetValue(ReadOnlyCanManageDepartmentsProperty); }
            private set { SetValue(ReadOnlyCanManageDepartmentsPropertyKey, value); RaisePropertyChanged(); UpdateCommands(); }
        }

        private bool GetCanManageDepartmentsProperty()
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
        #region Commands

        private void UpdateCommands()
        {
            insertDepartmentCommand?.RaiseCanExecuteChanged();
        }

        private DelegateCommand insertDepartmentCommand = null;
        public ICommand InsertDepartmentCommand { get { return insertDepartmentCommand ?? (insertDepartmentCommand = new DelegateCommand(o => Insert(), o => CanManageDepartments)); } }

        private void Insert()
        {
            var newDep = new DepartmentEditViewModel(true)
            {
                Data = new Department()
                {
                    ParentId = null,
                    Name = Properties.Resources.DEPARTMENTEDIT_NewDepartmentName,
                },
                Parent = this,
                Owner = this,
                IsSelected = true,
            };
            departments.Add(newDep);
        }

        #endregion
        #region ITreeItem<DepartmentEditViewModel>

        ITreeItem<DepartmentEditViewModel, Department> ITreeItem<DepartmentEditViewModel, Department>.Parent { get { return null; } set { throw new NotImplementedException(); } }
        Department ITreeItem<DepartmentEditViewModel, Department>.Data { get { return null; } set { throw new NotImplementedException(); } }
        ObservableCollection<ITreeItem<DepartmentEditViewModel, Department>> ITreeItem<DepartmentEditViewModel, Department>.Childs => departments;
        ITreeOwner<DepartmentEditViewModel> ITreeItem<DepartmentEditViewModel, Department>.Owner => this;

        private void RaisePropertyChanged([ParenthesizePropertyName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void RunUnderDispatcher(Delegate a)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, a);
        }

        public StaffingViewModel()
        {
            worker.CopyObjectTo(this);
            worker.OnErrorChanged += (s, e) => RunUnderDispatcher(new Action(() => Error = e));
            worker.OnLoadedChanged += (s, e) => RunUnderDispatcher(new Action(() => IsLoaded = e));
            worker.OnStateChanged += (s, e) => RunUnderDispatcher(new Action(() => State = e));
            worker.OnNotification += (s, e) => RunUnderDispatcher(new Action(() => Notifications?.Add(e)));
            worker.OnDepartmentsChanged += (s,e) => RunUnderDispatcher(new Action(() => OnDepartmentsChanged(s, e)));
            worker.OnEmployeesChanged += (s, e) => RunUnderDispatcher(new Action(() => OnEmployeeChanged(s, e)));
            worker.OnCurrentChanged += (s, e) => RunUnderDispatcher(new Action(() =>
            {
                Current = e;
                CanManageDepartments = GetCanManageDepartmentsProperty();
            }));
            worker.OnRightsChanged += (s, e) => RunUnderDispatcher(new Action(() =>
            {
                OnRightsChanged(s, e);
                CanManageDepartments = GetCanManageDepartmentsProperty();
            }));
        }

        private void OnHistoryChanged(object sender, HistoryService.History e) => worker.ApplyHistoryChanges(e);

        private void OnDepartmentsChanged(object sender, StaffingListItemsEventArgs<Department> e)
        {
            if (new[] { StaffingListsAction.Add, StaffingListsAction.Change }.Contains(e.Action))
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existed = departments.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Data.Id == d.Id);
                        if (existed != null)
                        {
                            existed.Data.CopyObjectFrom(d);
                        }
                        else
                        {
                            var existedParent = departments.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Data.Id == d.ParentId);
                            if (existedParent != null)
                            {
                                var newDep = new DepartmentEditViewModel() { Data = d, Parent = existedParent, Owner = this };
                                var existedChildsInTop = departments.Where(i => i.Data.ParentId == d.Id);
                                foreach (var c in existedChildsInTop)
                                {
                                    c.Parent = newDep;
                                    newDep.Childs.Add(c);
                                }
                                existedParent.Childs.Add(newDep);
                            }
                            else
                            {
                                departments.Add(new DepartmentEditViewModel() { Data = d, Parent = this, Owner = this });
                            }
                        }
                    }
                }
            else
                foreach (var d in e.Items)
                    if (d.Id != 0)
                    {
                        var existed = departments.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Data.Id == d.Id);
                        if (existed != null)
                            existed.Parent?.Childs.Remove(existed);
                    }
        }

        private void OnRightsChanged(object s, StaffingListItemsEventArgs<Right> e)
        {
            if (e.Action == StaffingListsAction.Add)
                e.Items.ToList().ForEach(i => rights.Add(i));
            else if (e.Action == StaffingListsAction.Change)
                e.Items.Join(rights, i => i.Id, n => n.Id, (i, n) => new { New = i, Old = n }).ToList().ForEach(i => i.New.CopyObjectTo(i.Old));
            else if (e.Action == StaffingListsAction.Remove)
                e.Items.Join(rights, i => i.Id, n => n.Id, (i, n) => n).ToList().ForEach(i => rights.Remove(i));
        }

        private void OnEmployeeChanged(object s, StaffingListItemsEventArgs<Employee> e)
        {
            if (e.Action == StaffingListsAction.Add)
                e.Items.ToList().ForEach(i => employees.Add(i));
            else if (e.Action == StaffingListsAction.Change)
                e.Items.Join(employees, i => i.Id, n => n.Id, (i, n) => new { New = i, Old = n }).ToList().ForEach(i => i.New.CopyObjectTo(i.Old));
            else if (e.Action == StaffingListsAction.Remove)
                e.Items.Join(employees, i => i.Id, n => n.Id, (i, n) => n).ToList().ForEach(i => employees.Remove(i));
        }
    }
}
