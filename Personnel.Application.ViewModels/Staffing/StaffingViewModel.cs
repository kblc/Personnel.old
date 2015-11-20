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

        private NotifyCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>> departments = new NotifyCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>>();
        public IReadOnlyNotifyCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>> Departments => departments;

        private NotifyCollection<EmployeeViewModel> employees = new NotifyCollection<EmployeeViewModel>();
        public IReadOnlyNotifyCollection<EmployeeViewModel> Employees => employees;

        private NotifyCollection<StaffingService.Staffing> staffing = new NotifyCollection<StaffingService.Staffing>();
        //public IReadOnlyNotifyCollection<StaffingService.Staffing> Staffing => staffing;

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
                    new PropertyChangedCallback((s, e) => 
                    {
                        var model = s as StaffingViewModel;
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
        #region IsStaffingView

        public static readonly DependencyProperty IsStaffingViewProperty = DependencyProperty.Register(nameof(IsStaffingView), typeof(bool),
            typeof(StaffingViewModel), new PropertyMetadata(true, (s, e) =>
            {
                var model = s as StaffingViewModel;
                if (model != null)
                    model.RaisePropertyChanged(e.Property.Name);
            }));

        public bool IsStaffingView
        {
            get { return (bool)GetValue(IsStaffingViewProperty); }
            set { SetValue(IsStaffingViewProperty, value); RaisePropertyChanged(); }
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
                Data = new DepartmentAndStaffingData()
                { 
                    Department = new Department()
                    {
                        ParentId = null,
                        Name = Properties.Resources.DEPARTMENTEDIT_NewDepartmentName,
                    }
                },
                Parent = this,
                Owner = this,
                IsSelected = true,
            };
            departments.Add(newDep);
        }

        #endregion
        #region ITreeItem<DepartmentAndStaffingData>

        ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData> ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>.Parent { get { return null; } set { throw new NotImplementedException(); } }
        DepartmentAndStaffingData ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>.Data { get { return null; } set { throw new NotImplementedException(); } }
        ObservableCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>> ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>.Childs => departments;
        ITreeOwner<DepartmentEditViewModel> ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>.Owner => this;

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
            worker.OnDepartmentsChanged += (s,e) => RunUnderDispatcher(new Action(() => OnWorkerDepartmentsChanged(s, e)));
            worker.OnEmployeesChanged += (s, e) => RunUnderDispatcher(new Action(() => OnWorkerEmployeeChanged(s, e)));
            worker.OnStaffingChanged += (s, e) => RunUnderDispatcher(new Action(() => OnWorkerStaffingChanged(s, e)));
            worker.OnCurrentChanged += (s, e) => RunUnderDispatcher(new Action(() =>
            {
                Current = e;
                CanManageDepartments = GetCanManageDepartmentsProperty();
            }));
            worker.OnRightsChanged += (s, e) => RunUnderDispatcher(new Action(() =>
            {
                OnWorkerRightsChanged(s, e);
                CanManageDepartments = GetCanManageDepartmentsProperty();
            }));
        }

        private void OnHistoryChanged(object sender, HistoryService.History e) => worker.ApplyHistoryChanges(e);

        private DepartmentAndStaffingData GetStaffingDataForDepartment(Department dep)
        {
            var res = new DepartmentAndStaffingData() { Department = dep };
            foreach (var sd in staffing.Where(s => s.DepartmentId == dep.Id)
                .Select(s => new EmployeeAndStaffingData()
                {
                    Staffing = s,
                    Employee = employees.FirstOrDefault(e => e.Employee.Stuffing?.Id == s.Id)
                }))
                res.Staffing.Add(sd);
            return res;
        }
        private EmployeeViewModel GetViewModelForEmployee(Employee emp)
        {
            var deps = departments.AsEnumerable().Traverse(d => d.Childs).ToArray();
            var res = new EmployeeViewModel()
            {
                Employee = emp,
                Department = deps.Where(d => d.Data.Department.Id == emp?.Stuffing?.Id)
                                .Select(d => d.Data.Department)
                                .FirstOrDefault(),
                Photo = emp.Photos
                    .Where(p => p.Picture.PictureType == PictureType.Avatar64)
                    .OrderBy(p => p.Picture?.File?.Date)
                    .Select(p => p.Picture)
                    .FirstOrDefault()
            };
            return res;
        }

        private void OnWorkerDepartmentsChanged(object sender, StaffingListItemsEventArgs<Department> e)
        {
            if (new[] { StaffingListsAction.Add, StaffingListsAction.Change }.Contains(e.Action))
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existed = departments.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Data.Department.Id == d.Id);
                        if (existed != null)
                        {
                            existed.Data.CopyObjectFrom(d);
                        }
                        else
                        {
                            var existedParent = departments.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Data.Department.Id == d.ParentId);
                            if (existedParent != null)
                            {
                                var newDep = new DepartmentEditViewModel() { Data = GetStaffingDataForDepartment(d), Parent = existedParent, Owner = this };
                                var existedChildsInTop = departments.Where(i => i.Data.Department.ParentId == d.Id);
                                foreach (var c in existedChildsInTop)
                                {
                                    c.Parent = newDep;
                                    newDep.Childs.Add(c);
                                }
                                existedParent.Childs.Add(newDep);
                            }
                            else
                            {
                                departments.Add(new DepartmentEditViewModel() { Data = GetStaffingDataForDepartment(d), Parent = this, Owner = this });
                            }
                        }
                    }
                }
            else
                foreach (var d in e.Items)
                    if (d.Id != 0)
                    {
                        var existed = departments.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Data.Department.Id == d.Id);
                        if (existed != null)
                            existed.Parent?.Childs.Remove(existed);
                    }

            OnDepartmentsChanged?.Invoke(this, e);
        }
        private void OnWorkerRightsChanged(object s, StaffingListItemsEventArgs<Right> e)
        {
            if (e.Action == StaffingListsAction.Add)
                e.Items.ToList().ForEach(i => rights.Add(i));
            else if (e.Action == StaffingListsAction.Change)
                e.Items.Join(rights, i => i.Id, n => n.Id, (i, n) => new { New = i, Old = n }).ToList().ForEach(i => i.New.CopyObjectTo(i.Old));
            else if (e.Action == StaffingListsAction.Remove)
                e.Items.Join(rights, i => i.Id, n => n.Id, (i, n) => n).ToList().ForEach(i => rights.Remove(i));

            OnRightsChanged?.Invoke(this, e);
        }
        private void OnWorkerEmployeeChanged(object s, StaffingListItemsEventArgs<Employee> e)
        {
            if (e.Action == StaffingListsAction.Add)
            {
                e.Items.ToList().ForEach(n =>
                {
                    var empVM = GetViewModelForEmployee(n);
                    employees.Add(empVM);

                    var fullItems = departments.AsEnumerable().Traverse(d => d.Childs).ToArray();
                    var treeItemForItem = fullItems.FirstOrDefault(d => d.Data.Department.Id == empVM?.Department?.Id);
                    var treeItemWithThisItem = fullItems.FirstOrDefault(d => d.Data.Staffing.Any(sd => sd.Employee?.Employee?.Id == empVM.Employee.Id));
                    if (treeItemWithThisItem != null)
                    {
                        var existedStaffing = treeItemWithThisItem.Data.Staffing.FirstOrDefault(sd => sd.Employee.Employee.Id == empVM.Employee.Id);
                        existedStaffing.Employee = null;
                    }
                    if (treeItemForItem != null)
                    {
                        treeItemForItem.Data.Staffing.Add(new EmployeeAndStaffingData()
                        {
                            Staffing = empVM.Employee.Stuffing,
                            Employee = empVM
                        });
                    }
                });
            }
            else if (e.Action == StaffingListsAction.Change)
            {
                e.Items.Join(employees, i => i.Id, n => n.Employee.Id, (i, n) => new { New = i, Old = n }).ToList().ForEach(i =>
                {
                    var empVM = GetViewModelForEmployee(i.New);
                    empVM.CopyObjectTo(i.Old);

                    var fullItems = departments.AsEnumerable().Traverse(d => d.Childs).ToArray();
                    var treeItemForItem = fullItems.FirstOrDefault(d => d.Data.Department.Id == empVM?.Department?.Id);
                    var treeItemWithThisItem = fullItems.FirstOrDefault(d => d.Data.Staffing.Any(sd => sd.Employee?.Employee?.Id == empVM.Employee.Id));
                    if (treeItemWithThisItem != null)
                    {
                        var existedStaffing = treeItemWithThisItem.Data.Staffing.FirstOrDefault(sd => sd.Employee?.Employee?.Id == empVM.Employee.Id);
                        existedStaffing.Employee = null;
                    }
                    if (treeItemForItem != null)
                    {
                        treeItemForItem.Data.Staffing.Add(new EmployeeAndStaffingData()
                        {
                            Staffing = empVM.Employee.Stuffing,
                            Employee = empVM
                        });
                    }
                });
            }
            else if (e.Action == StaffingListsAction.Remove)
            { 
                e.Items.Join(employees, i => i.Id, n => n.Employee.Id, (i, n) => n).ToList().ForEach(i => 
                {
                    var fullItems = departments.AsEnumerable().Traverse(d => d.Childs).ToArray();
                    var treeItemWithThisItem = fullItems.FirstOrDefault(d => d.Data.Staffing.Any(sd => sd.Employee.Employee.Id == i.Employee.Id));
                    if (treeItemWithThisItem != null)
                    {
                        var existedStaffing = treeItemWithThisItem.Data.Staffing.FirstOrDefault(sd => sd.Employee.Employee.Id == i.Employee.Id);
                        existedStaffing.Employee = null;
                    }
                    employees.Remove(i);
                });
            }
            OnEmployeesChanged?.Invoke(this, e);
        }
        private void OnWorkerStaffingChanged(object s, StaffingListItemsEventArgs<StaffingService.Staffing> e)
        {
            if (e.Action == StaffingListsAction.Add)
            { 
                e.Items.ToList().ForEach(i =>
                {
                    staffing.Add(i);
                    var fullItems = departments.AsEnumerable().Traverse(d => d.Childs).ToArray();
                    var treeItemForItem = fullItems.FirstOrDefault(d => d.Data.Department.Id == i.DepartmentId);
                    var treeItemWithThisItem = fullItems.FirstOrDefault(d => d.Data.Staffing.Any(sd => sd.Staffing.Id == i.Id));

                    if (treeItemWithThisItem != null)
                    {
                        var existedStaffing = treeItemWithThisItem.Data.Staffing.FirstOrDefault(sd => sd.Staffing.Id == i.Id);
                        if (existedStaffing != null)
                            treeItemWithThisItem.Data.Staffing.Remove(existedStaffing);
                    }

                    if (treeItemForItem != null)
                    {
                        treeItemForItem.Data.Staffing.Add(new EmployeeAndStaffingData()
                        {
                            Staffing = i,
                            Employee = employees.FirstOrDefault(e2 => e2.Employee.Stuffing?.Id == i.Id)
                        });
                    }
                });
            }
            else if (e.Action == StaffingListsAction.Change)
            { 
                e.Items.Join(staffing, i => i.Id, n => n.Id, (i, n) => new { New = i, Old = n }).ToList().ForEach(i => 
                {
                    i.New.CopyObjectTo(i.Old);

                    var fullItems = departments.AsEnumerable().Traverse(d => d.Childs).ToArray();
                    var treeItemForItem = fullItems.FirstOrDefault(d => d.Data.Department.Id == i.Old.DepartmentId);
                    var treeItemWithThisItem = fullItems.FirstOrDefault(d => d.Data.Staffing.Any(sd => sd.Staffing.Id == i.Old.Id));

                    if (treeItemWithThisItem != null)
                    {
                        var existedStaffing = treeItemWithThisItem.Data.Staffing.FirstOrDefault(sd => sd.Staffing.Id == i.Old.Id);
                        if (existedStaffing != null)
                            treeItemWithThisItem.Data.Staffing.Remove(existedStaffing);
                    }

                    if (treeItemForItem != null)
                    {
                        treeItemForItem.Data.Staffing.Add(new EmployeeAndStaffingData()
                        {
                            Staffing = i.Old,
                            Employee = employees.FirstOrDefault(e2 => e2.Employee.Stuffing?.Id == i.Old.Id)
                        });
                    }
                });
            }
            else if (e.Action == StaffingListsAction.Remove)
            { 
                e.Items.Join(staffing, i => i.Id, n => n.Id, (i, n) => n).ToList().ForEach(i =>
                {
                    var fullItems = departments.AsEnumerable().Traverse(d => d.Childs).ToArray();
                    var treeItemForItem = fullItems.FirstOrDefault(d => d.Data.Department.Id == i.DepartmentId);
                    var treeItemWithThisItem = fullItems.FirstOrDefault(d => d.Data.Staffing.Any(sd => sd.Staffing.Id == i.Id));
                    if (treeItemWithThisItem != null)
                    {
                        var existedStaffing = treeItemWithThisItem.Data.Staffing.FirstOrDefault(sd => sd.Staffing.Id == i.Id);
                        if (existedStaffing != null)
                            treeItemForItem.Data.Staffing.Remove(existedStaffing);
                    }
                    staffing.Remove(i);
                });
            }
            OnStaffingChanged?.Invoke(this, e);
        }
        private void RaiseOnIsLoadedChanged(bool value)
        {
            RunUnderDispatcher(new Action(() => OnIsLoadedChanged?.Invoke(this, value)));
        }

        public event EventHandler<bool> OnIsLoadedChanged;
        public event EventHandler<StaffingListItemsEventArgs<StaffingService.Right>> OnRightsChanged;
        public event EventHandler<StaffingListItemsEventArgs<StaffingService.Staffing>> OnStaffingChanged;
        public event EventHandler<StaffingListItemsEventArgs<StaffingService.Employee>> OnEmployeesChanged;
        public event EventHandler<StaffingListItemsEventArgs<StaffingService.Department>> OnDepartmentsChanged;
    }
}
