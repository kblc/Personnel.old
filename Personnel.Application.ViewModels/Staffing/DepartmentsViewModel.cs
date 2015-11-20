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

namespace Personnel.Application.ViewModels.Staffing
{
    public interface ITreeOwner<T> : INotifyPropertyChanged
    {
        bool CanManageDepartments { get; }
        bool IsDebugView { get; }
        bool IsStaffingView { get; }
    }

    public interface ITreeItem<T, TStore>
    {
        /// <summary>
        /// Tree owner
        /// </summary>
        ITreeOwner<T> Owner { get; }

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
    }

    public class DepartmentAndStaffingData : Additional.NotifyPropertyChangedBase
    {
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

        private ITreeOwner<DepartmentEditViewModel> owner = null;
        public ITreeOwner<DepartmentEditViewModel> Owner
        {
            get { return owner; }
            set
            {
                if (owner == value)
                    return;

                if (owner != null)
                    throw new ArgumentException(nameof(Owner));
                owner = value;

                if (owner != null)
                    owner.PropertyChanged += (s,e) =>
                    {
                        if (e.PropertyName == nameof(owner.CanManageDepartments))
                            RaisePropertyChanged(() => CanManage);
                        if (e.PropertyName == nameof(owner.IsDebugView))
                            RaisePropertyChanged(() => IsDebugView);
                    };

                RaisePropertyChanged(() => Owner);
            }
        }

        public ObservableCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>> Childs { get; } = 
            new ObservableCollection<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>> ();

        private Helpers.WPF.DelegateCommand deleteCommand = null;
        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new Helpers.WPF.DelegateCommand((o) => { RaiseOnDelete(); }, (o) => CanManage && !IsBusy && !IsDeleted));
            }
        }

        private Helpers.WPF.DelegateCommand saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new Helpers.WPF.DelegateCommand((o) => { RaiseOnChange(); }, (o) => CanManage && !IsBusy && !IsDeleted && IsEditMode));
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
                        RaiseOnDelete();
                    else
                        IsEditMode = false;
                }, (o) => CanManage && !IsBusy && !IsDeleted && IsEditMode));
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
            get { return setToEditModeCommand ?? (setToEditModeCommand = new DelegateCommand(o => { IsEditMode = true; }, (o) => CanManage && !IsBusy && !IsDeleted)); }
        }

        private Helpers.WPF.DelegateCommand addChildCommand = null;
        public ICommand AddChildCommand
        {
            get { return addChildCommand ?? (addChildCommand = new Helpers.WPF.DelegateCommand((o) => RaiseOnAddChild(), (o) => CanManage && !IsBusy && !IsDeleted && !IsEditMode)); }
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

        public bool CanManage => Owner?.CanManageDepartments ?? false;
        public bool IsDebugView => Owner?.IsDebugView ?? false;
        public bool IsStaffingView => Owner?.IsStaffingView ?? false;

        private string GetExceptionText(string whereCatched, Exception ex)
        {
            return ex.GetExceptionText($"{GetType().Name}.{whereCatched}()"
#if !DEBUG
                , clearText: true, includeData: false, includeStackTrace: false
#endif

                );
        }

        private async void RaiseOnChange()
        {
            IsBusy = true;
            try
            {
                Data.Department.Name = DepartmentName;
                if (await RaiseOnChangeAsync())
                {
                    IsEditMode = false;
                    Error = null;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(RaiseOnChange), ex);
            }
        }
        private async Task<bool> RaiseOnChangeAsync()
        {
            var srvc = new StaffingService.StaffingServiceClient();

            var act = (Data.Department.Id == 0)
                    ? srvc.DepartmentInsertAsync(Data.Department)
                    : srvc.DepartmentUpdateAsync(Data.Department);

            var res = act
                .ContinueWith<bool>(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                            Error = GetExceptionText(nameof(RaiseOnDelete), t.Exception);
                        else
                        if (!string.IsNullOrWhiteSpace(t.Result.Error))
                            Error = t.Result.Error;
                        else
                        {
                            this.Data.Department.CopyObjectFrom(t.Result.Value);
                            return true;
                        }
                        return false;
                    }
                    finally
                    {
                        IsBusy = false;
                        try { srvc.Close(); } catch { }
                    }
                });
            return await res;
        }

        private async void RaiseOnDelete()
        {
            IsBusy = true;
            IsDeleted = true;
            try
            {
                var deleted = (Data.Department.Id == 0)
                    ? true
                    : await RaiseOnDeleteAsync();
                
                if (deleted && Parent != null && Parent.Childs.Contains(this))
                    Parent.Childs.Remove(this);

                IsEditMode = false;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Error = GetExceptionText(nameof(RaiseOnDelete), ex);
            }
        }
        private async Task<bool> RaiseOnDeleteAsync()
        {
            var allDepsToDelete = this.Traverse<ITreeItem<DepartmentEditViewModel, DepartmentAndStaffingData>>(i => i.Childs);
            var allDepsIdsToDelete = allDepsToDelete.Select(i => i.Data.Department.Id).ToArray();

            var srvc = new StaffingService.StaffingServiceClient();
            var res = srvc.DepartmentRemoveRangeAsync(allDepsIdsToDelete)
                .ContinueWith<bool>(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                            Error = GetExceptionText(nameof(RaiseOnDelete), t.Exception);
                        else
                        if (!string.IsNullOrWhiteSpace(t.Result.Error))
                            Error = t.Result.Error;
                        else
                            return true;

                        return false;
                    }
                    finally
                    {
                        IsBusy = false;
                        try { srvc.Close(); } catch { }
                    }
                });
            return await res;
        }

        private void RaiseOnAddChild()
        {
            IsBusy = true;
            try
            {
                var newDep = new DepartmentEditViewModel(true)
                {
                    Data = new DepartmentAndStaffingData()
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
