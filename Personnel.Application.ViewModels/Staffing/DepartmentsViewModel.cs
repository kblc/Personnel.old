using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Helpers;
using Helpers.Linq;
using Helpers.WPF;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Personnel.Application.ViewModels.StaffingService;
using System.Windows;

namespace Personnel.Application.ViewModels.Staffing
{
    public abstract class DataOwner : DependencyObject, INotifyPropertyChanged
    {
        public abstract bool CanManageDepartments { get; protected set; }
        public abstract bool CanManageStaffing { get; protected set; }
        public abstract bool IsDebugView { get; set; }
        public abstract bool IsStaffingVisible { get; set; }
        public abstract bool IsDragMode { get; protected set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([ParenthesizePropertyName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void DragModeBegin() { IsDragMode = true; }
        internal void DragModeEnd() { IsDragMode = false; }
    }

    public interface ITreeItem<T, TStore>
    {
        /// <summary>
        /// Tree owner
        /// </summary>
        DataOwner Owner { get; }

        /// <summary>
        /// Current item parent
        /// </summary>
        ITreeItem<T, TStore> Parent { get; set; }

        /// <summary>
        /// Childs for current item
        /// </summary>
        ObservableCollection<ITreeItem<T, TStore>> Childs { get; }

        /// <summary>
        /// Stored data
        /// </summary>
        TStore Data { get; set; }
    }

    public class EmployeeAndStaffingData : Additional.NotifyPropertyChangedBase
    {
        private string appointName = string.Empty;
        public string AppointName
        {
            get { return appointName; }
            set { if (appointName == value) return; appointName = value; RaisePropertyChanged(() => AppointName); }
        }

        public EmployeeAndStaffingData(DataOwner owner, bool createEdited = false)
        {
            Owner = owner;
            IsEditMode = createEdited;
        }

        public static EmployeeAndStaffingData GetFake(DataOwner owner) => new EmployeeAndStaffingData(owner, createEdited: false) { IsFake = true, Staffing = new StaffingService.Staffing() { Position = long.MaxValue } };

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            private set { if (isBusy == value) return; isBusy = value; RaisePropertyChanged(() => IsBusy); RaiseAllComamnds(); }
        }

        private bool isFake = false;
        public bool IsFake
        {
            get { return isFake; }
            private set { if (isFake == value) return; isFake = value; RaisePropertyChanged(() => IsFake); RaiseAllComamnds(); }
        }

        private bool isDeleted = false;
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { if (isDeleted == value) return; isDeleted = value; RaisePropertyChanged(() => IsDeleted); RaiseAllComamnds(); }
        }

        public bool HasError { get { return !string.IsNullOrWhiteSpace(Error); } }

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
            internal set { if (error == value) return; error = value; RaisePropertyChanged(() => Error); RaisePropertyChanged(() => HasError); RaiseAllComamnds(); }
        }

        private bool isEditMode = false;
        public bool IsEditMode
        {
            get { return isEditMode; }
            private set
            {
                if (isEditMode == value)
                    return;
                isEditMode = value;
                AppointName = Staffing?.Appoint;
                RaisePropertyChanged(() => IsEditMode);
                RaiseAllComamnds();
            }
        }

        private DataOwner owner = null;
        public DataOwner Owner
        {
            get { return owner; }
            private set
            {
                if (owner == value)
                    return;

                if (value == null)
                    throw new ArgumentNullException(nameof(Owner));

                if (owner != null)
                    owner.PropertyChanged -= OwnerPropertyChanged;
                owner = value;
                owner.PropertyChanged += OwnerPropertyChanged;

                RaisePropertyChanged();
            }
        }

        private void OwnerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseAllComamnds();
        }

        private StaffingService.Staffing staffing = null;
        public StaffingService.Staffing Staffing
        {
            get { return staffing; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Staffing));

                if (staffing == value)
                    return;
                staffing = value;

                if (isEditMode)
                    AppointName = staffing?.Appoint;

                RaisePropertyChanged(() => Staffing);
            }
        }

        private EmployeeViewModel employee = null;
        public EmployeeViewModel Employee
        {
            get { return employee; }
            set
            {
                if (employee == value)
                    return;
                employee = value;
                RaisePropertyChanged(() => Employee);
            }
        }

        private string GetExceptionText(string whereCatched, Exception ex)
        {
            return ex.GetExceptionText($"{GetType().Name}.{whereCatched}()"
#if !DEBUG
                , clearText: true, includeData: false, includeStackTrace: false
#endif

                );
        }

        private DelegateCommand fakeCommand = null;
        public ICommand FakeCommand
        {
            get
            {
                return fakeCommand ?? (fakeCommand = new DelegateCommand(o =>
                {
                    FakeClick?.Invoke(this, new EventArgs());
                }, o => IsFake));
            }
        }

        private DelegateCommand cancelCommand = null;
        public ICommand CancelCommand
        {
            get
            {
                return cancelCommand ?? (cancelCommand = new DelegateCommand(o => 
                {
                    IsEditMode = false;
                    CancelClick?.Invoke(this, new EventArgs());
                }, o => IsEditMode));
            }
        }

        private DelegateCommand increasePositionCommand = null;
        public ICommand IncreasePositionCommand
        {
            get
            {
                return increasePositionCommand ?? (increasePositionCommand = new DelegateCommand(o =>
                {
                    var s = new StaffingService.Staffing() { Position = Staffing.Position + 1 };
                    s.CopyObjectFrom(Staffing, new string[] { nameof(s.Position) });
                    SaveAsync(s);
                }, o => !IsEditMode));
            }
        }

        private DelegateCommand decreasePositionCommand = null;
        public ICommand DecreasePositionCommand
        {
            get
            {
                return decreasePositionCommand ?? (decreasePositionCommand = new DelegateCommand(o =>
                {
                    if (Staffing.Position <= 1)
                        return;
                    var s = new StaffingService.Staffing() { Position = Staffing.Position - 1 };
                    s.CopyObjectFrom(Staffing, new string[] { nameof(s.Position) });
                    SaveAsync(s);
                }, o => !IsEditMode && Staffing.Position > 1));
            }
        }

        private DelegateCommand saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new DelegateCommand(o => 
                {
                    var s = new StaffingService.Staffing() { Appoint = AppointName };
                    s.CopyObjectFrom(Staffing, new string[] { nameof(s.Appoint) });
                    SaveAsync(s);
                    SaveClick?.Invoke(this, new EventArgs());
                }, o => Owner.CanManageStaffing && !IsBusy && !IsDeleted && IsEditMode));
            }
        }

        private async void SaveAsync(StaffingService.Staffing staffingToSave)
        {
            IsBusy = true;
            try
            {
                var sc = new StaffingServiceClient();
                var waittask = Staffing.Id == 0
                    ? sc.StaffingInsertAsync(staffingToSave)
                    : sc.StaffingUpdateAsync(staffingToSave);
                await waittask.ContinueWith(t => 
                    {
                        try
                        {
                            if (t.Exception != null)
                            {
                                Error = GetExceptionText(nameof(SaveAsync), t.Exception);
                            } else
                            {
                                if (!string.IsNullOrWhiteSpace(t.Result.Error))
                                { 
                                    Error = t.Result.Error;
                                }
                                else
                                {
                                    Error = null;
                                    this.Staffing.CopyObjectFrom(t.Result.Value);
                                    RaiseAllComamnds();
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            Error = GetExceptionText(nameof(SaveAsync), ex);
                        }
                        finally
                        {
                            try { sc.Close(); } catch { }
                            AppointName = Staffing.Appoint;
                            IsBusy = false;
                            RaiseAllComamnds();
                        }
                    }, 
                    System.Threading.CancellationToken.None, 
                    TaskContinuationOptions.AttachedToParent,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(SaveAsync), ex);
            }
        }

        private DelegateCommand deleteCommand = null;
        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new DelegateCommand(o => 
                {
                    DeleteAsync();
                    RemoveClick?.Invoke(this, new EventArgs());
                }, o => Owner.CanManageStaffing && !IsBusy && !IsDeleted && !IsEditMode));
            }
        }

        private async void DeleteAsync()
        {
            if (Staffing.Id == 0)
            { 
                IsDeleted = true;
                return;
            }

            IsBusy = true;
            try
            {
                var sc = new StaffingServiceClient();
                var waittask = sc.StaffingRemoveAsync(Staffing.Id);
                await waittask.ContinueWith(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                        {
                            Error = GetExceptionText(nameof(SaveAsync), t.Exception);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(t.Result.Error))
                            {
                                Error = t.Result.Error;
                            }
                            else
                            {
                                Error = null;
                                IsDeleted = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = GetExceptionText(nameof(SaveAsync), ex);
                    }
                    finally
                    {
                        try { sc.Close(); } catch { }
                        IsBusy = false;
                    }
                },
                    System.Threading.CancellationToken.None,
                    TaskContinuationOptions.AttachedToParent,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(SaveAsync), ex);
            }
            
        }

        private Helpers.WPF.DelegateCommand setToEditModeCommand = null;
        public ICommand SetToEditModeCommand
        {
            get { return setToEditModeCommand ?? (setToEditModeCommand = new DelegateCommand(o => { IsEditMode = true; }, (o) => Owner.CanManageStaffing && !IsBusy && !IsDeleted)); }
        }

        private DelegateCommand dropEmployeeCommand = null;
        public ICommand DropEmployeeCommand { get { return dropEmployeeCommand ?? (dropEmployeeCommand = new DelegateCommand(o => DropEmployee(o as System.Windows.DragEventArgs), o => Owner.CanManageStaffing)); } }
        private void DropEmployee(System.Windows.DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Data.GetDataPresent(typeof(EmployeeViewModel)))
            {
                var emplVM = e.Data.GetData(typeof(EmployeeViewModel)) as EmployeeViewModel;
                if (emplVM != null && emplVM != this.Employee)
                {
                    var setNewAction = new Action(() =>
                    {
                        this.Employee = null;
                        emplVM.Employee.Stuffing = this.Staffing;
                        SaveEmployeeAsync(emplVM.Employee, new Action(() => { this.Employee = emplVM; }), new Action(() => { this.Employee = null; emplVM.Employee.Stuffing = null; }));
                    });

                    if (this.Employee != null)
                    {
                        this.Employee.Employee.Stuffing = null;
                        SaveEmployeeAsync(emplVM.Employee, setNewAction, new Action(() => { this.Employee.Employee.Stuffing = this.Staffing; }));
                    }
                    else
                        setNewAction();
                }
            }
        }

        private DelegateCommand dragOverEmployeeCommand = null;
        public ICommand DragOverEmployeeCommand { get { return dragOverEmployeeCommand ?? (dragOverEmployeeCommand = new DelegateCommand(o => DragOverEmployee(o as System.Windows.DragEventArgs), o => Owner.CanManageStaffing)); } }
        private void DragOverEmployee(System.Windows.DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (!e.Data.GetDataPresent(typeof(EmployeeViewModel)) || e.Data.GetData(typeof(EmployeeViewModel)) == this.Employee)
            { 
                e.Effects = System.Windows.DragDropEffects.None;
                e.Handled = true;
            }
        }

        //private DelegateCommand queryContinueDragEmployeeCommand = null;
        //public ICommand QueryContinueDragEmployeeCommand { get { return queryContinueDragEmployeeCommand ?? (queryContinueDragEmployeeCommand = new DelegateCommand(o => QueryContinueDragEmployee(o as System.Windows.QueryContinueDragEventArgs), o => Owner.CanManageStaffing)); } }
        //private void QueryContinueDragEmployee(System.Windows.QueryContinueDragEventArgs e)
        //{
        //    //if (e == null)
        //    //    throw new ArgumentNullException(nameof(e));

        //    //if (e.Action  == System.Windows.DragAction.Drop)
        //    //{
        //    //    this.Employee = null;
        //    //}
        //}

        private DelegateCommand mouseDownEmployeeCommand = null;
        public ICommand MouseDownEmployeeCommand { get { return mouseDownEmployeeCommand ?? (mouseDownEmployeeCommand = new DelegateCommand(o => MouseDownEmployee(o as MouseButtonEventArgs), o => Owner.CanManageStaffing)); } }
        private void MouseDownEmployee(MouseButtonEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Owner.DragModeBegin();
                DragDrop.DoDragDrop((DependencyObject)e.Source, this.Employee, DragDropEffects.Move);
                Owner.DragModeEnd();
            }
        }

        internal async void SaveEmployeeAsync(StaffingService.Employee employeeToSave, Action onSuccess, Action onError)
        {
            IsBusy = true;
            try
            {
                var sc = new StaffingServiceClient();
                var waittask = sc.EmployeeUpdateAsync(employeeToSave);
                await waittask.ContinueWith(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                        {
                            Error = GetExceptionText(nameof(SaveEmployeeAsync), t.Exception);
                            onError?.Invoke();
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(t.Result.Error))
                            {
                                Error = t.Result.Error;
                                onError?.Invoke();
                            }
                            else
                            {
                                Error = null;
                                onSuccess?.Invoke();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = GetExceptionText(nameof(SaveEmployeeAsync), ex);
                    }
                    finally
                    {
                        try { sc.Close(); } catch { }
                        IsBusy = false;
                    }
                },
                    System.Threading.CancellationToken.None,
                    TaskContinuationOptions.AttachedToParent,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(SaveAsync), ex);
            }

        }

        private void RaiseAllComamnds()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                saveCommand?.RaiseCanExecuteChanged();
                cancelCommand?.RaiseCanExecuteChanged();
                deleteCommand?.RaiseCanExecuteChanged();
                setToEditModeCommand?.RaiseCanExecuteChanged();
                decreasePositionCommand?.RaiseCanExecuteChanged();
                increasePositionCommand?.RaiseCanExecuteChanged();
                dropEmployeeCommand?.RaiseCanExecuteChanged();
                dragOverEmployeeCommand?.RaiseCanExecuteChanged();
                mouseDownEmployeeCommand?.RaiseCanExecuteChanged();
            }));
        }

        public event EventHandler CancelClick;
        public event EventHandler SaveClick;
        public event EventHandler RemoveClick;
        public event EventHandler FakeClick;
    }

    public class DepartmentAndStaffingData : Additional.NotifyPropertyChangedBase
    {
        public DepartmentAndStaffingData(DataOwner owner)
        {
            Owner = owner;
            Staffing.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null && e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    e.NewItems.Cast<EmployeeAndStaffingData>().ToList().ForEach(i =>
                    {
                        i.CancelClick += (s2, e2) =>
                        {
                            var item = ((EmployeeAndStaffingData)s2);
                            if (item.Staffing.Id == 0)
                            Staffing.Remove(item);
                        };
                        i.FakeClick += (s2, e2) => AddChild();
                    });
            };
            Staffing.Add(EmployeeAndStaffingData.GetFake(owner));
        }

        private DataOwner owner = null;
        public DataOwner Owner
        {
            get { return owner; }
            private set
            {
                if (owner == value)
                    return;

                if (value == null)
                    throw new ArgumentNullException(nameof(Owner));
                if (owner != null)
                    owner.PropertyChanged -= OwnerPropertyChanged;
                owner = value;
                owner.PropertyChanged += OwnerPropertyChanged;
                RaisePropertyChanged();
            }
        }

        private void OwnerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseAllComamnds();
        }

        private void RaiseAllComamnds()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                addChildCommand?.RaiseCanExecuteChanged();
            }));
        }

        private StaffingService.Department department = null;
        public StaffingService.Department Department
        {
            get { return department; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Department));

                if (department == value)
                    return;
                department = value;
                RaisePropertyChanged(() => Department);
            }
        }

        private void AddChild()
        {
            var pos = Staffing
                .Where(i => i.Staffing.Position < long.MaxValue)
                .Select(i => i.Staffing.Position)
                .Union(new long[] { 0 })
                .Max() + 1;
            var n = new EmployeeAndStaffingData(owner, true) { Staffing = new StaffingService.Staffing() { DepartmentId = Department.Id, Position = pos, Appoint = Properties.Resources.STAFFINGEDIT_NewAppointName } };
            Staffing.Add(n);
        }

        private DelegateCommand addChildCommand = null;
        public ICommand AddChildCommand { get { return addChildCommand ?? (addChildCommand = new DelegateCommand(o => AddChild(), o => Owner.CanManageStaffing)); } }

        public ObservableCollection<EmployeeAndStaffingData> Staffing { get; }
            = new ObservableCollection<EmployeeAndStaffingData>();
    }

    public interface ITreeDepartmentItem : ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData> { }

    public class DepartmentEditViewModel : Additional.NotifyPropertyChangedBase, ITreeDepartmentItem
    {
        public DepartmentEditViewModel() : this(false) { }
        public DepartmentEditViewModel(bool createEdited) { IsEditMode = createEdited; }

        private DepartmentAndStaffingData data = null;
        public DepartmentAndStaffingData Data
        {
            get { return data; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Data));

                if (data == value)
                    return;
                data = value;
                if (data != null)
                    DepartmentName = data?.Department?.Name;
                RaisePropertyChanged(() => Data);
                RaiseAllComamnds();
            }
        }

        private ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData> parent = null;
        public ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData> Parent
        {
            get { return parent; }
            set
            {
                if (parent == value)
                    return;
                parent = value;
                RaisePropertyChanged(() => Parent);
            }
        }

        private DataOwner owner = null;
        public DataOwner Owner
        {
            get { return owner; }
            set
            {
                if (owner == value)
                    return;

                if (owner != null)
                    throw new ArgumentException(nameof(Owner));

                if (owner != null)
                    owner.PropertyChanged -= OwnerPropertyChanged;
                owner = value;
                owner.PropertyChanged += OwnerPropertyChanged;
                RaisePropertyChanged(() => Owner);
            }
        }

        private void OwnerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseAllComamnds();
        }

        public ObservableCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>> Childs { get; } = 
            new ObservableCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>> ();

        private Helpers.WPF.DelegateCommand deleteCommand = null;
        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new Helpers.WPF.DelegateCommand((o) => { DeleteAsync(); }, (o) => Owner.CanManageDepartments && !IsBusy && !IsDeleted));
            }
        }

        private Helpers.WPF.DelegateCommand saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new Helpers.WPF.DelegateCommand((o) => 
                {
                    var dep = new StaffingService.Department() { Name = DepartmentName };
                    dep.CopyObjectFrom(Data.Department, new string[] { nameof(dep.Name) });
                    SaveAsync(dep);
                }, (o) => Owner.CanManageDepartments && !IsBusy && !IsDeleted && IsEditMode));
            }
        }

        private Helpers.WPF.DelegateCommand cancelCommand = null;
        public ICommand CancelCommand
        {
            get
            {
                return cancelCommand ?? (cancelCommand = new Helpers.WPF.DelegateCommand(o =>
                {
                    if (Data.Department.Id == 0)
                        DeleteAsync();
                    else
                        IsEditMode = false;
                }, (o) => Owner.CanManageDepartments && !IsBusy && !IsDeleted && IsEditMode));
            }
        }

        private Helpers.WPF.DelegateCommand copyErrorCommand = null;
        public ICommand CopyErrorCommand
        {
            get
            {
                return copyErrorCommand ?? (copyErrorCommand = new Helpers.WPF.DelegateCommand(o =>
                {
                    System.Windows.Clipboard.SetText(Error);
                }));
            }
        }

        private Helpers.WPF.DelegateCommand setToEditModeCommand = null;
        public ICommand SetToEditModeCommand
        {
            get { return setToEditModeCommand ?? (setToEditModeCommand = new DelegateCommand(o => { IsEditMode = true; }, (o) => Owner.CanManageDepartments && !IsBusy && !IsDeleted)); }
        }

        private Helpers.WPF.DelegateCommand addChildCommand = null;
        public ICommand AddChildCommand
        {
            get { return addChildCommand ?? (addChildCommand = new Helpers.WPF.DelegateCommand((o) => RaiseOnAddChild(), (o) => Owner.CanManageDepartments && !IsBusy && !IsDeleted && !IsEditMode)); }
        }

        private void RaiseAllComamnds()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                saveCommand?.RaiseCanExecuteChanged();
                cancelCommand?.RaiseCanExecuteChanged();
                deleteCommand?.RaiseCanExecuteChanged();
                addChildCommand?.RaiseCanExecuteChanged();
                setToEditModeCommand?.RaiseCanExecuteChanged();
            }));
        }

        private bool isEditMode = false;
        public bool IsEditMode
        {
            get { return isEditMode; }
            private set
            {
                if (isEditMode == value)
                    return;
                isEditMode = value;
                DepartmentName = Data?.Department?.Name;
                RaisePropertyChanged(() => IsEditMode);
                RaiseAllComamnds();
            }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            private set { if (isBusy == value) return; isBusy = value; RaisePropertyChanged(() => IsBusy); RaiseAllComamnds(); }
        }

        private bool isDeleted = false;
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { if (isDeleted == value) return; isDeleted = value; RaisePropertyChanged(() => IsDeleted); RaiseAllComamnds(); }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set { if (isSelected == value) return; isSelected = value; RaisePropertyChanged(() => IsSelected); }
        }

        public bool HasError { get { return !string.IsNullOrWhiteSpace(Error); }}

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
            internal set { if (error == value) return; error = value; RaisePropertyChanged(() => Error); RaisePropertyChanged(() => HasError); RaiseAllComamnds(); }
        }

        private string departmentName = string.Empty;
        public string DepartmentName
        {
            get { return departmentName; }
            set { if (departmentName == value) return; departmentName = value; RaisePropertyChanged(() => DepartmentName); }
        }

        private string GetExceptionText(string whereCatched, Exception ex)
        {
            return ex.GetExceptionText($"{GetType().Name}.{whereCatched}()"
#if !DEBUG
                , clearText: true, includeData: false, includeStackTrace: false
#endif

                );
        }

        private async void SaveAsync(StaffingService.Department dapartmentToSave)
        {
            IsBusy = true;
            try
            {
                var sc = new StaffingServiceClient();
                var waittask = (Data.Department.Id == 0)
                    ? sc.DepartmentInsertAsync(dapartmentToSave)
                    : sc.DepartmentUpdateAsync(dapartmentToSave);

                await waittask.ContinueWith(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                        {
                            Error = GetExceptionText(nameof(SaveAsync), t.Exception);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(t.Result.Error))
                            {
                                Error = t.Result.Error;
                            }
                            else
                            {
                                Error = null;
                                IsEditMode = false;
                                Data.Department.CopyObjectFrom(t.Result.Value);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = GetExceptionText(nameof(SaveAsync), ex);
                    }
                    finally
                    {
                        try { sc.Close(); } catch { }
                        IsBusy = false;
                    }
                },
                System.Threading.CancellationToken.None,
                TaskContinuationOptions.AttachedToParent,
                TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(SaveAsync), ex);
            }
        }

        private async void DeleteAsync()
        {
            if (Data.Department.Id == 0)
            {
                IsDeleted = true;
                DetachFromParent();
                return;
            }

            IsBusy = true;
            try
            {
                var sc = new StaffingServiceClient();
                var waittask = sc.DepartmentRemoveAsync(Data.Department.Id);
                await waittask.ContinueWith(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                        {
                            Error = GetExceptionText(nameof(DeleteAsync), t.Exception);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(t.Result.Error))
                            {
                                Error = t.Result.Error;
                            }
                            else
                            {
                                Error = null;
                                IsDeleted = true;
                                DetachFromParent();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = GetExceptionText(nameof(DeleteAsync), ex);
                    }
                    finally
                    {
                        try { sc.Close(); } catch { }
                        IsBusy = false;
                    }
                },
                System.Threading.CancellationToken.None,
                TaskContinuationOptions.AttachedToParent,
                TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(DeleteAsync), ex);
            }
        }

        private void DetachFromParent()
        {
            if (Parent != null && Parent.Childs.Contains(this))
                Parent.Childs.Remove(this);
        }

        private void RaiseOnAddChild()
        {
            IsBusy = true;
            try
            {
                var newDep = new DepartmentEditViewModel(true)
                {
                    Data = new DepartmentAndStaffingData(this.Owner)
                    {
                        Department = new StaffingService.Department()
                        {
                            ParentId = Data.Department.Id,
                            Name = Properties.Resources.DEPARTMENTEDIT_NewDepartmentName
                        }
                    },
                    Parent = this,
                    Owner = this.Owner,
                    IsSelected = true
                };
                Childs.Add(newDep);
                Error = null;
            }
            catch (Exception ex)
            {
                Error = GetExceptionText(nameof(RaiseOnAddChild), ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
