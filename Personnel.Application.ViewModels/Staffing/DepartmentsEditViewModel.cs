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

namespace Personnel.Application.ViewModels.Staffing
{
    public class DepartmentEditViewModel : Additional.NotifyPropertyChangedBase
    {
        public DepartmentEditViewModel()
        {
            Static.Departments
                .PropertyChanged += (s, e) => { if (e.PropertyName == nameof(Static.Departments.CanManage)) RaiseAllComamnds(); };
        }

        private StaffingService.Department department = null;
        public StaffingService.Department Department { get { return department; } set { if (department == value) return; Unsubscribe(department); department = value; Subscribe(department); RaisePropertyChanged(() => Department); RaiseAllComamnds(); } }

        public ObservableCollection<DepartmentEditViewModel> Childs { get; } = new MTObservableCollection<DepartmentEditViewModel>();

        private Helpers.WPF.DelegateCommand deleteCommand = null;
        private ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new Helpers.WPF.DelegateCommand((o) => { RaiseOnDelete(); OnDelete?.Invoke(this, o); }, (o) => Static.Departments.CanManage && !IsBusy && !IsDeleted));
            }
        }

        private Helpers.WPF.DelegateCommand saveCommand = null;
        private ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new Helpers.WPF.DelegateCommand((o) => { RaiseOnChange(); OnChange?.Invoke(this, o); }, (o) => Static.Departments.CanManage && !IsBusy && !IsDeleted));
            }
        }

        private Helpers.WPF.DelegateCommand addChildCommand = null;        private ICommand AddChildCommand
        {
            get
            {
                return addChildCommand ?? (addChildCommand = 
                    new Helpers.WPF.DelegateCommand((o) => 
                    {
                        var child = RaiseOnAddChild();
                        if (child != null)
                            OnAddChild?.Invoke(this, child);
                    }
                    , (o) => Static.Departments.CanManage && !IsBusy && !IsNew && !IsDeleted));
            }
        }

        private void RaiseAllComamnds()
        {
            saveCommand?.RaiseCanExecuteChanged();
            deleteCommand?.RaiseCanExecuteChanged();
            addChildCommand?.RaiseCanExecuteChanged();
        }

        private DepartmentEditViewModel parent = null;
        public DepartmentEditViewModel Parent
        {
            get { return parent; }
            internal set { if (parent == value) return; parent = value; RaisePropertyChanged(() => Parent); }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            internal set { if (isBusy == value) return; isBusy = value; RaisePropertyChanged(() => IsBusy); RaiseAllComamnds(); }
        }

        public bool IsNew { get { return Department?.Id != 0; } }

        private bool isDeleted = false;
        public bool IsDeleted
        {
            get { return isDeleted; }
            internal set { if (isDeleted == value) return; isDeleted = value; RaisePropertyChanged(() => IsDeleted); RaiseAllComamnds(); }
        }

        public bool HasError { get { return !string.IsNullOrWhiteSpace(Error); }}

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
            internal set { if (error == value) return; error = value; RaisePropertyChanged(() => Error); RaisePropertyChanged(() => HasError); RaiseAllComamnds(); }
        }

        private void Unsubscribe(StaffingService.Department department)
        {
            if (department == null)
                return;
            department.PropertyChanged -= Department_PropertyChanged;
        }
        private void Subscribe(StaffingService.Department department)
        {
            if (department == null)
                return;
            department.PropertyChanged += Department_PropertyChanged;
        }

        private void Department_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaiseOnChange();
            OnChange?.Invoke(this, e);
        }

        private void RaiseOnChange()
        {
            IsBusy = true;
            using (var srvc = new StaffingService.StaffingServiceClient())
                try
                {
                    var res = (Department.Id == 0)
                        ? srvc.DepartmentInsertAsync(Department)
                        : srvc.DepartmentUpdateAsync(Department);

                    res.ContinueWith((dep) =>
                    {
                        try
                        { 
                            if (res.Exception != null)
                            {
                                Error = res.Exception.GetExceptionText(clearText: true, includeStackTrace: false);
                            } else
                            {
                                if (dep.Result.Error != null)
                                {
                                    Error = dep.Result.Error;
                                }
                                else
                                {
                                    Unsubscribe(this.Department);
                                    try
                                    {
                                        this.Department.CopyObjectFrom(dep.Result.Value);
                                        addChildCommand?.RaiseCanExecuteChanged();
                                        deleteCommand?.RaiseCanExecuteChanged();
                                    }
                                    finally
                                    {
                                        Subscribe(this.Department);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IsBusy = false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    Error = ex.GetExceptionText(clearText: true, includeStackTrace: false);
                }
        }
        private void RaiseOnDelete()
        {
            IsBusy = true;

            if (IsNew)
            {
                if (Parent != null)
                    Parent.Childs.Remove(this);
            } else
            using (var srvc = new StaffingService.StaffingServiceClient())
                try
                {
                    var allDepsToDelete = this.Traverse(i => i.Childs);
                    var allDepsIdsToDelete = allDepsToDelete.Select(i => i.Department.Id).ToArray();
                    var res = srvc.DepartmentRemoveRangeAsync(allDepsIdsToDelete);
                    res.ContinueWith((dep) =>
                    {
                        try
                        {
                            if (res.Exception != null)
                            {
                                Error = res.Exception.GetExceptionText(clearText: true, includeStackTrace: false);
                            }
                            else
                            {
                                if (dep.Result.Error != null)
                                {
                                    Error = dep.Result.Error;
                                }
                                else
                                {
                                    foreach (var i in allDepsToDelete)
                                    {
                                        i.IsDeleted = true;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IsBusy = false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    Error = ex.GetExceptionText(clearText: true, includeStackTrace: false);
                    IsBusy = false;
                }
        }
        private DepartmentEditViewModel RaiseOnAddChild()
        {
            IsBusy = true;
            try
            {
                var newDep = new DepartmentEditViewModel()
                {
                    Department = new StaffingService.Department()
                    {
                        ParentId = Department.Id,
                        Name = Properties.Resources.DEPARTMENTEDIT_NewDepartmentName
                    }
                };
                Childs.Add(newDep);
                return newDep;
            }
            catch (Exception ex)
            {
                Error = ex.GetExceptionText(clearText: true, includeStackTrace: false);
                IsBusy = false;
                return null;
            }
        }

        internal event EventHandler<object> OnDelete;
        internal event EventHandler<object> OnChange;
        internal event EventHandler<DepartmentEditViewModel> OnAddChild;
    }

    public class DepartmentsEditViewModel : Additional.AbstractBaseViewModel
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
                var canDeleteRightId = Static.Staffing.Rights.Where(r => r.SystemName == MANAGEDEPARTMENTS).Select(r => r.Id).FirstOrDefault();
                return (Static.Staffing.Current != null && Static.Staffing.Current.Rights.Any(r => r.RightId == canDeleteRightId));
            }
        }

        public ObservableCollection<DepartmentEditViewModel> Tree { get; } = new MTObservableCollection<DepartmentEditViewModel>();

        protected override void Init()
        {
            if (true || this.IsDesignMode())
            { 
                LoadTest();
            }
            else
            { 
                Static.Staffing.IsLoadedChanged += (s, isLoaded) => { if (isLoaded) Load(); };
                if (Static.Staffing.IsLoaded)
                    Load();
            }
        }
        private void Load()
        {
            IsLoaded = false;
            try
            {
                if (Static.Staffing.Current != null)
                    Static.Staffing.Current.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName.StartsWith(nameof(Static.Staffing.Current.Rights)))
                            RaisePropertyChanged(() => CanManage);
                    };

                var depArray = Static.Staffing.Department.ToArray();
                Static.Staffing.Department.Where(d => d.ParentId == null)
                    .Select(d => new DepartmentEditViewModel() { Department = d })
                    .ToList()
                    .ForEach(i =>
                    {
                        LoadChilds(depArray, i);
                        Tree.Add(i);
                    });

                Static.Staffing.Department.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                        foreach(var d in e.NewItems.Cast<StaffingService.Department>())
                        {
                            if (d.Id != 0)
                            {
                                var existed = Tree.AsEnumerable().Traverse(i => i.Childs).FirstOrDefault(i => i.Department.Id == d.Id);
                                if (existed != null)
                                {
                                    existed.Department = d;
                                } else
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

                                        existedParent.Childs.Add(newDep);
                                    }
                                    else
                                    {
                                        Tree.Add(new DepartmentEditViewModel() { Department = d });
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
                                        existed.Parent.Childs.Remove(existed);
                                    else if (Tree.Contains(existed))
                                        Tree.Remove(existed);
                                }
                            }
                };

                IsLoaded = true;
            }
            catch(Exception ex)
            {
                Error = ex.GetExceptionText(clearText: true, includeStackTrace: false);
            }
        }

        private void LoadTest()
        {
            var top1 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Top 1", Id = 1 } };
            //var top2 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Top 2", Id = 2 } };

            //var sTop1 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Sub 1 top 1", Id = 3, ParentId = 1 }, Parent = top1 };
            //var sTop2 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Sub 3 top 1", Id = 4, ParentId = 1 }, Parent = top1 };
            //var sTop3 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Sub 3 top 1", Id = 5, ParentId = 1 }, Parent = top1 };

            //var sTop21 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Sub 1 top 1", Id = 6, ParentId = 2 }, Parent = top2 };
            //var sTop22 = new DepartmentEditViewModel() { Department = new StaffingService.Department() { Name = "Sub 3 top 1", Id = 7, ParentId = 2 }, Parent = top2 };

            //top1.Childs.Add(sTop1);
            //top1.Childs.Add(sTop2);
            //top1.Childs.Add(sTop3);

            //top2.Childs.Add(sTop21);
            //top2.Childs.Add(sTop22);

            Tree.Add(top1);
            //Tree.Add(top2);

            IsLoaded = true;
        }

        private void LoadChilds(IEnumerable<StaffingService.Department> allDeps, DepartmentEditViewModel viewModel)
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
