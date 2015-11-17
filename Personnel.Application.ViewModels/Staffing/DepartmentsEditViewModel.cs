using Personnel.Application.ViewModels.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Helpers;
using Helpers.Linq;
using Helpers.WPF;
using System.ComponentModel;
using Swordfish.NET.Collections;

namespace Personnel.Application.ViewModels.Staffing
{
    public interface ITreeItem<T> : INotifyPropertyChanged
    {
        ITreeItem<T> Parent { get; set; }
        ConcurrentObservableCollection<ITreeItem<T>> Childs { get; }
        StaffingService.Department Department { get; set; }
        bool CanManage { get; }
        bool ShowDebugInfo { get; }
    }

    public class DepartmentEditViewModel : Additional.NotifyPropertyChangedBase, ITreeItem<DepartmentEditViewModel>
    {
        public DepartmentEditViewModel() : this(false) { }
        public DepartmentEditViewModel(bool createEdited) { IsEditMode = createEdited; }

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
                if (department != null)
                    DepartmentName = department.Name;
                RaisePropertyChanged(() => Department);
                RaiseAllComamnds();
            }
        }

        private ITreeItem<DepartmentEditViewModel> parent = null;
        public ITreeItem<DepartmentEditViewModel> Parent
        {
            get { return parent; }
            set
            {
                if (parent == value)
                    return;

                if (parent != null)
                    parent.PropertyChanged -= Parent_PropertyChanged;
                parent = value;
                if (parent != null)
                    parent.PropertyChanged += Parent_PropertyChanged;

                RaisePropertyChanged(() => Parent);
            }
        }

        private void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CanManage))
                RaisePropertyChanged(() => CanManage);
            if (e.PropertyName == nameof(ShowDebugInfo))
                RaisePropertyChanged(() => ShowDebugInfo);
        }

        public ConcurrentObservableCollection<ITreeItem<DepartmentEditViewModel>> Childs { get; } = new ConcurrentObservableCollection<ITreeItem<DepartmentEditViewModel>> ();

        private Helpers.WPF.DelegateCommand deleteCommand = null;
        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new Helpers.WPF.DelegateCommand((o) => { RaiseOnDelete(); }, (o) => Static.Departments.CanManage && !IsBusy && !IsDeleted));
            }
        }

        private Helpers.WPF.DelegateCommand saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new Helpers.WPF.DelegateCommand((o) => { RaiseOnChange(); }, (o) => Static.Departments.CanManage && !IsBusy && !IsDeleted && IsEditMode));
            }
        }

        private Helpers.WPF.DelegateCommand cancelCommand = null;
        public ICommand CancelCommand
        {
            get
            {
                return cancelCommand ?? (cancelCommand = new Helpers.WPF.DelegateCommand(o =>
                {
                    if (IsNew)
                        RaiseOnDelete();
                    else
                        IsEditMode = false;
                }, (o) => Static.Departments.CanManage && !IsBusy && !IsDeleted && IsEditMode));
            }
        }

        private Helpers.WPF.DelegateCommand setToEditModeCommand = null;
        public ICommand SetToEditModeCommand
        {
            get { return setToEditModeCommand ?? (setToEditModeCommand = new DelegateCommand(o => { IsEditMode = true; }, (o) => Static.Departments.CanManage && !IsBusy && !IsDeleted)); }
        }

        private Helpers.WPF.DelegateCommand addChildCommand = null;
        public ICommand AddChildCommand
        {
            get { return addChildCommand ?? (addChildCommand = new Helpers.WPF.DelegateCommand((o) => RaiseOnAddChild(), (o) => Static.Departments.CanManage && !IsBusy && !IsNew && !IsDeleted && !IsEditMode)); }
        }

        private void RaiseAllComamnds()
        {
            saveCommand?.RaiseCanExecuteChanged();
            cancelCommand?.RaiseCanExecuteChanged();
            deleteCommand?.RaiseCanExecuteChanged();
            addChildCommand?.RaiseCanExecuteChanged();
            setToEditModeCommand?.RaiseCanExecuteChanged();
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
                DepartmentName = Department?.Name;
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

        public bool IsNew { get { return Department?.Id == 0; } }

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

        public bool CanManage => Parent?.CanManage ?? false;
        public bool ShowDebugInfo => Parent?.ShowDebugInfo ?? false;

        private string GetExceptionText(string whereCatched, Exception ex)
        {
            return ex.GetExceptionText($"{GetType().Name}.{whereCatched}()"
#if !DEBUG
                , clearText: true, includeData: false, includeStackTrace: false
#endif

                );
        }

        private void RaiseOnChange()
        {
            IsBusy = true;
            var srvc = new StaffingService.StaffingServiceClient();
            try
            {
                Department.Name = DepartmentName;

                var res = (Department.Id == 0)
                    ? srvc.DepartmentInsertAsync(Department)
                    : srvc.DepartmentUpdateAsync(Department);

                var depAction = new Action<Task<StaffingService.DepartmentResult>>(dep =>
                {
                    try
                    {
                        if (dep.Exception != null)
                        {
                            Error = GetExceptionText(nameof(RaiseOnChange), res.Exception);
                        }
                        else
                        {
                            if (dep.Result.Error != null)
                            {
                                Error = dep.Result.Error;
                            }
                            else
                            {
                                this.Department.CopyObjectFrom(dep.Result.Value);
                                IsEditMode = false;
                                Error = null;
                            }
                        }
                    }
                    finally
                    {
                        IsBusy = false;
                        try { srvc.Close(); } catch { }
                    }
                });

                var ui = TaskScheduler.FromCurrentSynchronizationContext();
                if (ui != null)
                    res.ContinueWith(depAction, ui);
                else
                    res.ContinueWith(depAction);
            }
            catch (Exception ex)
            {
                try { srvc.Close(); } catch { }
                IsBusy = false;
                Error = GetExceptionText(nameof(RaiseOnChange), ex);
            }
        }
        private void RaiseOnDelete()
        {
            IsBusy = true;

            if (IsNew)
            {
                IsDeleted = true;

                var delTask = Task.Factory.StartNew(() =>
                {
                    if (Parent != null && Parent.Childs.Contains(this))
                        Parent.Childs.Remove(this);
                });

                var afterDelTask = new Action<Task>(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                        {
                            Error = GetExceptionText(nameof(RaiseOnDelete), t.Exception);
                        }
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                });

                var ui = TaskScheduler.FromCurrentSynchronizationContext();
                if (ui != null)
                    delTask.ContinueWith(afterDelTask, ui);
                else
                    delTask.ContinueWith(afterDelTask);
            }


            var srvc = new StaffingService.StaffingServiceClient();
            try
            {
                var allDepsToDelete = this.Traverse<ITreeItem<DepartmentEditViewModel>>(i => i.Childs);
                var allDepsIdsToDelete = allDepsToDelete.Select(i => i.Department.Id).ToArray();
                var res = srvc.DepartmentRemoveRangeAsync(allDepsIdsToDelete);

                var depAction = new Action<Task<StaffingService.Result>>(dep =>
                {
                    try
                    {
                        if (dep.Exception != null)
                        {
                            Error = GetExceptionText(nameof(RaiseOnDelete), res.Exception);
                        }
                        else
                        {
                            if (dep.Result.Error != null)
                            {
                                Error = dep.Result.Error;
                            }
                            else
                            {
                                if (Parent != null && Parent.Childs.Contains(this))
                                    Parent.Childs.Remove(this);

                                Error = null;
                            }
                        }
                    }
                    finally
                    {
                        IsBusy = false;
                        try { srvc.Close(); } catch { }
                    }
                });

                var ui = TaskScheduler.FromCurrentSynchronizationContext();
                if (ui != null)
                    res.ContinueWith(depAction, ui);
                else
                    res.ContinueWith(depAction);
            }
            catch (Exception ex)
            {
                try { srvc.Close(); } catch { }
                Error = GetExceptionText(nameof(RaiseOnDelete), ex);
                IsBusy = false;
            }
        }
        private void RaiseOnAddChild()
        {
            IsBusy = true;
            try
            {
                var newDep = new DepartmentEditViewModel(true)
                {
                    Department = new StaffingService.Department()
                    {
                        ParentId = Department.Id,
                        Name = Properties.Resources.DEPARTMENTEDIT_NewDepartmentName
                    },
                    Parent = this,
                    IsSelected = true
                };
                Childs.Add(newDep);
                Error = null;
            }
            catch (Exception ex)
            {
                Error = GetExceptionText(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    public class DepartmentsEditViewModel : Additional.AbstractBaseViewModel, ITreeItem<DepartmentEditViewModel>
    {
        private const string MANAGEDEPARTMENTS = "MANAGEDEPARTMENTS";

        public bool HasError { get { return !string.IsNullOrWhiteSpace(Error); } }

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
            internal set { if (error == value) return; error = value; RaisePropertyChanged(() => Error); RaisePropertyChanged(() => HasError); }
        }

        public bool CanManage
        {
            get
            {
                return true;

                var canDeleteRightId = Static.Staffing.Rights.Where(r => r.SystemName == MANAGEDEPARTMENTS).Select(r => r.Id).FirstOrDefault();
                return (Static.Staffing.Current != null && Static.Staffing.Current.Rights.Any(r => r.RightId == canDeleteRightId));
            }
        }

        private bool showDebugInfo
#if DEBUG
            = true;
#else
            = false;
#endif
        public bool ShowDebugInfo
        {
            get { return showDebugInfo; }
            set { if (showDebugInfo == value) return; showDebugInfo = value; RaisePropertyChanged(() => ShowDebugInfo); }
        }

        private DelegateCommand insertCommand = null;
        public ICommand InsertCommand { get { return insertCommand ?? (insertCommand = new DelegateCommand(o => Insert(), o => CanManage )); } }

        private ConcurrentObservableCollection<ITreeItem<DepartmentEditViewModel>> tree = null;
        public ConcurrentObservableCollection<ITreeItem<DepartmentEditViewModel>> Tree
        {
            get { return tree ?? (tree = new Swordfish.NET.Collections.ConcurrentObservableCollection<ITreeItem<DepartmentEditViewModel>>()); }
            private set { if (tree == value) return; tree = value; RaisePropertyChanged(() => Tree); }
        }

        #region ITreeItem<DepartmentEditViewModel>

        ITreeItem<DepartmentEditViewModel> ITreeItem<DepartmentEditViewModel>.Parent { get { return null; } set { throw new NotImplementedException(); } }
        StaffingService.Department ITreeItem<DepartmentEditViewModel>.Department { get { return null; } set { throw new NotImplementedException(); } }
        ConcurrentObservableCollection<ITreeItem<DepartmentEditViewModel>> ITreeItem<DepartmentEditViewModel>.Childs => Tree;

        #endregion

        private void Insert()
        {
            var newDep = new DepartmentEditViewModel(true)
            {
                Department = new StaffingService.Department()
                {
                    ParentId = null,
                    Name = Properties.Resources.DEPARTMENTEDIT_NewDepartmentName,
                },
                Parent = this,
                IsSelected = true,
            };
            Tree.Add(newDep);
        }

        private void RaiseInsertCommandUpdate()
        {
            try
            {
                ExecuteCommandAsDispatcher(() => insertCommand?.RaiseCanExecuteChanged(),
                    (t) => {
                        if (t.Exception != null)
                            Error = t.Exception.GetExceptionText();
                    });
            }
            catch (Exception ex)
            {
                Error = ex.GetExceptionText();
            }
        }

        private string GetExceptionText(Exception ex)
        {
#if DEBUG
            return ex.GetExceptionText();
#else
            return ex.GetExceptionText(clearText: true, includeStackTrace: false);
#endif
        }

        protected override void Init()
        {
            IsLoaded = false;

            if (this.IsDesignMode())
                Task.Factory.StartNew(() => LoadTest());

            SubscribeToStaffing();
        }

        private void SubscribeToStaffing()
        {
            Static.Staffing.IsLoadedChanged += (s, isLoaded) => { if (isLoaded) SubscribeToStaffingAfterLoading(); };
            if (Static.Staffing.IsLoaded)
                SubscribeToStaffingAfterLoading();
        }
        private void SubscribeToStaffingAfterLoading()
        {
            try
            {
                if (Static.Staffing.Current != null)
                    Static.Staffing.Current.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName.StartsWith(nameof(Static.Staffing.Current.Rights)))
                        {
                            ExecuteCommandAsDispatcher(() => RaisePropertyChanged(() => CanManage));
                            RaiseInsertCommandUpdate();
                        }
                    };

                Static.Staffing.Department.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                        foreach (var d in e.NewItems.Cast<StaffingService.Department>())
                        {
                            if (d.Id != 0)
                            {
                                var existed = Tree.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Department.Id == d.Id);
                                if (existed != null)
                                {
                                    existed.Department = d;
                                }
                                else
                                {
                                    var existedParent = Tree.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Department.Id == d.ParentId);
                                    if (existedParent != null)
                                    {
                                        var newDep = new DepartmentEditViewModel() { Department = d, Parent = existedParent };
                                        var existedChildsInTop = Tree.Where(i => i.Department.ParentId == d.Id);
                                        foreach (var c in existedChildsInTop)
                                        {
                                            c.Parent = newDep;
                                            newDep.Childs.Add(c);
                                        }
                                        ExecuteCommandAsDispatcher(() => existedParent.Childs.Add(newDep));
                                    }
                                    else
                                    {
                                        ExecuteCommandAsDispatcher(() => Tree.Add(new DepartmentEditViewModel() { Department = d, Parent = this }));
                                    }
                                }
                            }
                        }

                    if (e.OldItems != null)
                        foreach (var d in e.OldItems.Cast<StaffingService.Department>())
                            if (d.Id != 0)
                            {
                                var existed = Tree.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Department.Id == d.Id);
                                if (existed != null)
                                {
                                    if (existed.Parent != null)
                                        ExecuteCommandAsDispatcher(() => existed.Parent.Childs.Remove(existed));
                                    else if (Tree.Contains(existed))
                                        ExecuteCommandAsDispatcher(() => Tree.Remove(existed));
                                }
                            }
                };


                ExecuteCommandAsDispatcher(() => Tree = new ConcurrentObservableCollection<ITreeItem<DepartmentEditViewModel>>(Load(Static.Staffing.Department.ToArray(), this)));
            }
            catch(Exception ex)
            {
                Error = GetExceptionText(ex);
            }
            finally
            {
                IsLoaded = true;
            }
        }

        private static IEnumerable<ITreeItem<DepartmentEditViewModel>> Load(StaffingService.Department[] departments, ITreeItem<DepartmentEditViewModel> parent)
        {
            var res = departments.Where(d => d.ParentId == null)
                .Select(d => new DepartmentEditViewModel() { Department = d, Parent = parent })
                .ToList();
            res.ForEach(i => LoadChilds(departments, i));

            return res;
        }

        private void LoadTest()
        {
            ExecuteCommandAsDispatcher(() =>
            {
                var top1 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Top 1", Id = 1 }, Parent = this };
                var top2 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Top 2", Id = 2 }, Parent = this };

                var sTop1 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Sub 1 top 1", Id = 3, ParentId = 1 }, Parent = top1 };
                var sTop2 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Sub 3 top 1", Id = 4, ParentId = 1 }, Parent = top1 };
                var sTop3 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Sub 3 top 1", Id = 5, ParentId = 1 }, Parent = top1 };

                var sTop21 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Sub 1 top 1", Id = 6, ParentId = 2 }, Parent = top2 };
                var sTop22 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Sub 3 top 1", Id = 7, ParentId = 2 }, Parent = top2 };

                top1.Childs.Add(sTop1);
                top1.Childs.Add(sTop2);
                top1.Childs.Add(sTop3);

                top2.Childs.Add(sTop21);
                top2.Childs.Add(sTop22);

                Tree.Add(top1);
                Tree.Add(top2);

                IsLoaded = true;
            });
        }

        private static void LoadChilds(IEnumerable<StaffingService.Department> allDeps, DepartmentEditViewModel viewModel)
        {
            var childs = allDeps.Where(i => i.ParentId == viewModel.Department.Id);
            foreach(var c in childs)
            {
                var item = new DepartmentEditViewModel() { Department = c, Parent = viewModel };
                LoadChilds(allDeps, item);
                viewModel.Childs.Add(item);
            }
        }
    }
}
