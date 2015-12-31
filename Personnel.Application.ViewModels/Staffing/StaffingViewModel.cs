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
    public class StaffingViewModel : DataOwner, ITreeDepartmentItem
    {
        private const string MANAGEDEPARTMENTS = "MANAGEDEPARTMENTS";
        private const string MANAGESTAFFING = "MANAGESTAFFING";
        private const string MANAGEEMPLOYES = "MANAGEEMPLOYES";
        private const string MANAGEEMPLOYEESRIGHTS = "MANAGEEMPLOYEESRIGHTS";
        private const string MANAGEEMPLOYEESLOGINS = "MANAGEEMPLOYEESLOGINS";

        private readonly ServiceWorkers.StaffingWorker worker = new ServiceWorkers.StaffingWorker();

        private NotifyCollection<Right> rights = new NotifyCollection<Right>();
        public override IReadOnlyNotifyCollection<Right> Rights => rights;

        private NotifyCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>> departments = new NotifyCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>>();
        public IReadOnlyNotifyCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>> Departments => departments;

        private NotifyCollection<EmployeeViewModel> employees = new NotifyCollection<EmployeeViewModel>();
        public IReadOnlyNotifyCollection<EmployeeViewModel> Employees => employees;

        public ICollectionView EmployeesWithoutStaffing
        {
            get
            {
                var res = System.Windows.Data.CollectionViewSource.GetDefaultView(employees);
                res.Filter = (i) => 
                {
                    var empVM = i as EmployeeViewModel;
                    return empVM?.Employee.Stuffing == null;
                };
                return res;
            }
        }

        private ICommand employeesFilterCommand = null;
        public ICommand EmployeesFilterCommand { get { return employeesFilterCommand ?? (employeesFilterCommand = new DelegateCommand(o => EmployeesFilter(o as System.Windows.Data.FilterEventArgs))); } }

        private void EmployeesFilter(System.Windows.Data.FilterEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var empVM = e.Item as EmployeeViewModel;
            e.Accepted = empVM?.Employee.Stuffing == null;
        }

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
        #region IsDragMode

        private static readonly DependencyPropertyKey ReadOnlyIsDragModePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsDragMode), typeof(bool), typeof(StaffingViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyIsDragModeProperty = ReadOnlyIsDragModePropertyKey.DependencyProperty;

        public override bool IsDragMode
        {
            get { return (bool)GetValue(ReadOnlyIsDragModeProperty); }
            protected set { SetValue(ReadOnlyIsDragModePropertyKey, value); }
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

        public override bool IsDebugView
        {
            get { return (bool)GetValue(IsDebugViewProperty); }
            set { SetValue(IsDebugViewProperty, value); RaisePropertyChanged(); }
        }
        #endregion
        #region IsStaffingView

        public static readonly DependencyProperty IsStaffingViewProperty = DependencyProperty.Register(nameof(IsStaffingVisible), typeof(bool),
            typeof(StaffingViewModel), new PropertyMetadata(true, (s, e) =>
            {
                var model = s as StaffingViewModel;
                if (model != null)
                    model.RaisePropertyChanged(e.Property.Name);
            }));

        public override bool IsStaffingVisible
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

        public override bool CanManageDepartments
        {
            get { return (bool)GetValue(ReadOnlyCanManageDepartmentsProperty); }
            protected set { SetValue(ReadOnlyCanManageDepartmentsPropertyKey, value); RaisePropertyChanged(); UpdateCommands(); }
        }

        private bool GetCanManageDepartmentsProperty()
        {
            var res = false;
            if (Rights != null && Current != null)
            {
                var canManageRight = Rights.FirstOrDefault(r => string.Compare(r.SystemName, MANAGEDEPARTMENTS, true) == 0);
                if (canManageRight != null)
                    res = Current.Rights.Any(r => r.RightId == canManageRight.Id);
            }
            return res;
        }

        #endregion
        #region CanManageStaffing

        private static readonly DependencyPropertyKey ReadOnlyCanManageStaffingPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(CanManageStaffing), typeof(bool), typeof(StaffingViewModel),
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
        public static readonly DependencyProperty ReadOnlyCanManageStaffingProperty = ReadOnlyCanManageStaffingPropertyKey.DependencyProperty;

        public override bool CanManageStaffing
        {
            get { return (bool)GetValue(ReadOnlyCanManageStaffingProperty); }
            protected set { SetValue(ReadOnlyCanManageStaffingPropertyKey, value); RaisePropertyChanged(); UpdateCommands(); }
        }

        private bool GetCanManageStaffingProperty()
        {
            var res = false;
            if (Rights != null && Current != null)
            {
                var canManageRight = Rights.FirstOrDefault(r => string.Compare(r.SystemName, MANAGESTAFFING, true) == 0);
                if (canManageRight != null)
                    res = Current.Rights.Any(r => r.RightId == canManageRight.Id);
            }
            return res;
        }

        #endregion
        #region CanManageEmployees

        private static readonly DependencyPropertyKey ReadOnlyCanManageEmployesPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(CanManageEmployes), typeof(bool), typeof(StaffingViewModel),
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
        public static readonly DependencyProperty ReadOnlyCanManageEmployesProperty = ReadOnlyCanManageStaffingPropertyKey.DependencyProperty;

        public override bool CanManageEmployes
        {
            get { return (bool)GetValue(ReadOnlyCanManageEmployesProperty); }
            protected set { SetValue(ReadOnlyCanManageEmployesPropertyKey, value); RaisePropertyChanged(); }
        }

        private bool GetCanManageEmployesProperty()
        {
            var res = false;
            if (Rights != null && Current != null)
            {
                var canManageRight = Rights.FirstOrDefault(r => string.Compare(r.SystemName, MANAGEEMPLOYES, true) == 0);
                if (canManageRight != null)
                    res = Current.Rights.Any(r => r.RightId == canManageRight.Id);
            }
            return res;
        }

        #endregion
        #region CanManageEmployeeRights

        private static readonly DependencyPropertyKey ReadOnlyCanManageEmployeeRightsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(CanManageEmployeeRights), typeof(bool), typeof(StaffingViewModel),
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
        public static readonly DependencyProperty ReadOnlyCanManageEmployeeRightsProperty = ReadOnlyCanManageEmployeeRightsPropertyKey.DependencyProperty;

        public override bool CanManageEmployeeRights
        {
            get { return (bool)GetValue(ReadOnlyCanManageEmployeeRightsProperty); }
            protected set { SetValue(ReadOnlyCanManageEmployeeRightsPropertyKey, value); RaisePropertyChanged(); }
        }

        private bool GetCanManageEmployeRightsProperty()
        {
            var res = false;
            if (Rights != null && Current != null)
            {
                var canManageEmployeeLoginsRight = Rights.FirstOrDefault(r => string.Compare(r.SystemName, MANAGEEMPLOYEESRIGHTS, true) == 0);
                if (canManageEmployeeLoginsRight != null)
                    res = Current.Rights.Any(r => r.RightId == canManageEmployeeLoginsRight.Id);
            }
            return res;
        }

        #endregion
        #region CanManageEmployeeLogins

        private static readonly DependencyPropertyKey ReadOnlyCanManageEmployeeLoginsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(CanManageEmployeeLogins), typeof(bool), typeof(StaffingViewModel),
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
        public static readonly DependencyProperty ReadOnlyCanManageEmployeeLoginsProperty = ReadOnlyCanManageEmployeeLoginsPropertyKey.DependencyProperty;

        public override bool CanManageEmployeeLogins
        {
            get { return (bool)GetValue(ReadOnlyCanManageEmployeeLoginsProperty); }
            protected set { SetValue(ReadOnlyCanManageEmployeeLoginsPropertyKey, value); RaisePropertyChanged(); }
        }
        
        private bool GetCanManageEmployeeLoginsProperty()
        {
            var res = false;
            if (Rights != null && Current != null)
            {
                var canManageEmployeeLoginsRight = Rights.FirstOrDefault(r => string.Compare(r.SystemName, MANAGEEMPLOYEESLOGINS, true) == 0);
                if (canManageEmployeeLoginsRight != null)
                    res = Current.Rights.Any(r => r.RightId == canManageEmployeeLoginsRight.Id);
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
                Data = new DepartmentAndStaffingData(this)
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

        private DelegateCommand deleteDropEmployeeCommand = null;
        public ICommand DeleteDropEmployeeCommand { get { return deleteDropEmployeeCommand ?? (deleteDropEmployeeCommand = new DelegateCommand(o => DeleteDropEmployee(o as System.Windows.DragEventArgs), o => CanManageStaffing)); } }
        private void DeleteDropEmployee(System.Windows.DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Data.GetDataPresent(typeof(EmployeeViewModel)))
            {
                var emplVM = (EmployeeViewModel)e.Data.GetData(typeof(EmployeeViewModel));

                var empData = this.Departments.AsEnumerable()
                    .Traverse(i => i.Childs)
                    .SelectMany(i => i.Data.Staffing)
                    .Where(i => i.Employee == emplVM)
                    .FirstOrDefault();

                if (emplVM != null && empData != null)
                {
                    var oldStaffing = emplVM.Employee.Stuffing;
                    emplVM.Employee.Stuffing = null;
                    empData.SaveEmployeeAsync(emplVM.Employee, null, new Action(() => { emplVM.Employee.Stuffing = oldStaffing; }));
                }
            }
        }

        private DelegateCommand deleteDragOverEmployeeCommand = null;
        public ICommand DeleteDragOverEmployeeCommand { get { return deleteDragOverEmployeeCommand ?? (deleteDragOverEmployeeCommand = new DelegateCommand(o => DragOverEmployee(o as System.Windows.DragEventArgs), o => CanManageStaffing)); } }
        private void DragOverEmployee(System.Windows.DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var employeeIds = this.Employees.Where(emp => emp.Department != null)
                .Select(emp => emp.Employee.Id)
                .ToArray();

            if (!e.Data.GetDataPresent(typeof(EmployeeViewModel)) || !employeeIds.Contains((e.Data.GetData(typeof(EmployeeViewModel)) as EmployeeViewModel).Employee.Id))
                e.Effects = System.Windows.DragDropEffects.None;

            e.Handled = true;
        }


        #endregion
        #region ITreeItem<DepartmentAndStaffingData>

        ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData> ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>.Parent { get { return null; } set { throw new NotImplementedException(); } }
        DepartmentAndStaffingData ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>.Data { get { return null; } set { throw new NotImplementedException(); } }
        ObservableCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>> ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>.Childs => departments;
        DataOwner ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>.Owner => this;

        #endregion
        #region EmployeeForEdit

        public static readonly DependencyProperty EmployeeForEditProperty = DependencyProperty.Register(nameof(EmployeeForEdit), typeof(EmployeeViewModel),
            typeof(StaffingViewModel), new PropertyMetadata(null, (s, e) => {}));

        public EmployeeViewModel EmployeeForEdit
        {
            get { return (EmployeeViewModel)GetValue(EmployeeForEditProperty); }
            set { SetValue(EmployeeForEditProperty, value); }
        }

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
                CanManageStaffing = GetCanManageStaffingProperty();
                CanManageEmployes = GetCanManageEmployesProperty();
                CanManageEmployeeRights = GetCanManageEmployeRightsProperty();
                CanManageEmployeeLogins = GetCanManageEmployeeLoginsProperty();
                OnCurrentChanged?.Invoke(this, e);
            }));
            worker.OnRightsChanged += (s, e) => RunUnderDispatcher(new Action(() =>
            {
                OnWorkerRightsChanged(s, e);
                CanManageDepartments = GetCanManageDepartmentsProperty();
                CanManageStaffing = GetCanManageStaffingProperty();
                CanManageEmployes = GetCanManageEmployesProperty();
                CanManageEmployeeRights = GetCanManageEmployeRightsProperty();
                CanManageEmployeeLogins = GetCanManageEmployeeLoginsProperty();
            }));
            CheckForFakeEmployee();
        }

        private void OnHistoryChanged(object sender, HistoryService.History e) => worker.ApplyHistoryChanges(e);

        private DepartmentAndStaffingData GetStaffingDataForDepartment(Department dep)
        {
            var res = new DepartmentAndStaffingData(this) { Department = dep };
            foreach (var sd in staffing.Where(s => s.DepartmentId == dep.Id)
                .Select(s => new EmployeeAndStaffingData(this)
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
            var res = new EmployeeViewModel(this)
            {
                Employee = emp,
                Department = deps.Where(d => d.Data.Department.Id == emp?.Stuffing?.DepartmentId)
                                .Select(d => d.Data.Department)
                                .FirstOrDefault(),
            };
            return res;
        }
        private EmployeeViewModel GetFakeEmployee()
        {
            return new EmployeeViewModel(this)
            {
                Employee = new Employee(),
                IsEmpty = true
            };
        }

        private void CheckForFakeEmployee()
        {
            if (!employees.Any(emp => emp.IsEmpty))
            {
                var newEmpVM = GetFakeEmployee();
                newEmpVM.OnEditCommandExecuted += EmployeeViewModelOnEditCommandExecuted;
                employees.Add(newEmpVM);
            }
        }

        private void OnWorkerDepartmentsChanged(object sender, ListItemsEventArgs<Department> e)
        {
            if (new[] { ChangeAction.Add, ChangeAction.Change }.Contains(e.Action))
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        Department depForUpdate = null;
                        var existed = departments.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Data.Department.Id == d.Id);
                        if (existed != null)
                        {
                            existed.Data.Department.CopyObjectFrom(d);
                            depForUpdate = existed.Data.Department;
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
                                depForUpdate = newDep.Data.Department;
                            }
                            else
                            {
                                var depVM = new DepartmentEditViewModel() { Data = GetStaffingDataForDepartment(d), Parent = this, Owner = this };
                                departments.Add(depVM);
                                depForUpdate = depVM.Data.Department;
                            }
                        }

                        if (depForUpdate != null)
                            employees
                                .Where(e2 => e2.Employee.Stuffing?.DepartmentId == depForUpdate.Id)
                                .ToList()
                                .ForEach(emp => emp.Department = depForUpdate);

                    }
                }
            else
                foreach (var d in e.Items)
                    if (d.Id != 0)
                    {
                        var existed = departments.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Data.Department.Id == d.Id);
                        if (existed != null)
                            existed.Parent?.Childs.Remove(existed);

                        employees
                            .Where(e2 => e2.Employee.Stuffing?.DepartmentId == d.Id)
                            .ToList()
                            .ForEach(emp => emp.Department = null);
                    }

            OnDepartmentsChanged?.Invoke(this, e);
        }
        private void OnWorkerRightsChanged(object s, ListItemsEventArgs<Right> e)
        {
            if (e.Action == ChangeAction.Add)
                e.Items.ToList().ForEach(i => rights.Add(i));
            else if (e.Action == ChangeAction.Change)
                e.Items.Join(rights, i => i.Id, n => n.Id, (i, n) => new { New = i, Old = n }).ToList().ForEach(i => i.New.CopyObjectTo(i.Old));
            else if (e.Action == ChangeAction.Remove)
                e.Items.Join(rights, i => i.Id, n => n.Id, (i, n) => n).ToList().ForEach(i => rights.Remove(i));

            OnRightsChanged?.Invoke(this, e);
        }
        private void OnWorkerEmployeeChanged(object s, ListItemsEventArgs<Employee> e)
        {
            if (e.Action == ChangeAction.Add || e.Action == ChangeAction.Change)
            {
                var items = e.Action == ChangeAction.Add
                    ? e.Items.Select(i => 
                    {
                        var empVM = GetViewModelForEmployee(i);
                        employees.Add(empVM);
                        empVM.OnEditCommandExecuted += EmployeeViewModelOnEditCommandExecuted;
                        return empVM;
                    }).ToList()
                    : e.Items.Join(employees, i => i.Id, n => n.Employee.Id, (i, n) => new { New = i, Old = n }).Select(i => 
                    {
                        i.Old.CopyObjectFrom(GetViewModelForEmployee(i.New));
                        return i.Old;
                    }).ToList();

                items.ToList().ForEach(empVM =>
                {
                    var fullItems = departments.AsEnumerable().Traverse(d => d.Childs).ToArray();
                    var departmentItemForItem = fullItems.FirstOrDefault(d => d.Data.Department.Id == empVM?.Department?.Id);
                    var staffingItemWithThisItem = fullItems.SelectMany(i => i.Data.Staffing).Where(sd => sd.Staffing?.Id == empVM.Employee.Stuffing?.Id).ToList();
                    var employeeItemWithThisItem = fullItems.SelectMany(i => i.Data.Staffing).Where(sd => sd.Employee?.Employee?.Id == empVM.Employee.Id).ToList();
                    employeeItemWithThisItem.ForEach(i => i.Employee = null);
                    
                    if (staffingItemWithThisItem.Count == 1)
                    {
                        staffingItemWithThisItem.ForEach(i => i.Employee = empVM);
                    }
                    else 
                    if (departmentItemForItem != null)
                    {
                        staffingItemWithThisItem.ForEach(i => i.Employee = null);
                        departmentItemForItem.Data.Staffing.Add(new EmployeeAndStaffingData(this)
                        {
                            Staffing = empVM.Employee.Stuffing,
                            Employee = empVM
                        });
                    }
                });
            }
            else if (e.Action == ChangeAction.Remove)
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
                    i.OnEditCommandExecuted -= EmployeeViewModelOnEditCommandExecuted;
                });
            }
            CheckForFakeEmployee();
            OnEmployeesChanged?.Invoke(this, e);
        }

        private void EmployeeViewModelOnEditCommandExecuted(object sender, EventArgs e)
        {
            EmployeeForEdit?.CancelCommand.Execute(null);
            EmployeeForEdit = (EmployeeViewModel)sender;
        }

        private void OnWorkerStaffingChanged(object s, ListItemsEventArgs<StaffingService.Staffing> e)
        {
            var fullItems = departments.AsEnumerable().Traverse(d => d.Childs).ToArray();
            if (e.Action == ChangeAction.Add)
            { 
                e.Items.ToList().ForEach(i =>
                {
                    staffing.Add(i);
                    var departmentItemForItem = fullItems.FirstOrDefault(d => d.Data.Department.Id == i.DepartmentId);
                    var staffingtemWithThisItem = fullItems.Where(d => d.Data.Staffing.Any(sd => sd.Staffing.Id == i.Id)).ToArray();
                    foreach (var itm in staffingtemWithThisItem)
                    {
                        var itemsToRemove = itm.Data.Staffing.Where(sd => sd.Staffing.Id == i.Id).ToArray();
                        foreach (var itm2 in itemsToRemove)
                            itm.Data.Staffing.Remove(itm2);
                    }

                    if (departmentItemForItem != null)
                    {
                        departmentItemForItem.Data.Staffing.Add(new EmployeeAndStaffingData(this)
                        {
                            Staffing = i,
                            Employee = employees.FirstOrDefault(e2 => e2.Employee.Stuffing?.Id == i.Id)
                        });
                    }
                });
            }
            else if (e.Action == ChangeAction.Change)
            { 
                e.Items.Join(staffing, i => i.Id, n => n.Id, (i, n) => new { New = i, Old = n }).ToList().ForEach(i => 
                {
                    i.New.CopyObjectTo(i.Old);

                    var departmentItemForItem = fullItems.FirstOrDefault(d => d.Data.Department.Id == i.Old.DepartmentId);
                    var staffingtemWithThisItem = fullItems.Where(d => d.Data.Staffing.Any(sd => sd.Staffing.Id == i.Old.Id)).ToArray();
                    foreach(var itm in staffingtemWithThisItem)
                    {
                        var itemsToRemove = itm.Data.Staffing.Where(sd => sd.Staffing.Id == i.Old.Id).ToArray();
                        foreach(var itm2 in itemsToRemove)
                            itm.Data.Staffing.Remove(itm2);
                    }

                    if (departmentItemForItem != null)
                    {
                        departmentItemForItem.Data.Staffing.Add(new EmployeeAndStaffingData(this)
                        {
                            Staffing = i.New,
                            Employee = employees.FirstOrDefault(e2 => e2.Employee.Stuffing?.Id == i.Old.Id)
                        });
                    }
                });
            }
            else if (e.Action == ChangeAction.Remove)
            { 
                e.Items.Join(staffing, i => i.Id, n => n.Id, (i, n) => n).ToList().ForEach(i =>
                {
                    var treeItemWithThisItem = fullItems.FirstOrDefault(d => d.Data.Staffing.Any(sd => sd.Staffing.Id == i.Id));
                    if (treeItemWithThisItem != null)
                    {
                        var existedStaffing = treeItemWithThisItem.Data.Staffing.FirstOrDefault(sd => sd.Staffing.Id == i.Id);
                        if (existedStaffing != null)
                            treeItemWithThisItem.Data.Staffing.Remove(existedStaffing);
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
        public event EventHandler<ListItemsEventArgs<StaffingService.Right>> OnRightsChanged;
        public event EventHandler<StaffingService.Employee> OnCurrentChanged;
        public event EventHandler<ListItemsEventArgs<StaffingService.Staffing>> OnStaffingChanged;
        public event EventHandler<ListItemsEventArgs<StaffingService.Employee>> OnEmployeesChanged;
        public event EventHandler<ListItemsEventArgs<StaffingService.Department>> OnDepartmentsChanged;
    }
}
