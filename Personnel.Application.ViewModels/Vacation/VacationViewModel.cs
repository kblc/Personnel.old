using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Helpers.WPF;
using System.Windows.Input;
using Helpers;

namespace Personnel.Application.ViewModels.Vacation
{
    public class VacationViewModel : NotifyPropertyChangedBase
    {
        #region Constructor

        public VacationViewModel(VacationsViewModel owner) : this(owner, null, false) { }
        public VacationViewModel(VacationsViewModel owner, long employeeId, long levelId, bool createEdited)
            : this(owner, new VacationService.Vacation() { EmployeeId = employeeId, VacationLevelId = levelId }, createEdited) { }
        public VacationViewModel(VacationsViewModel owner, VacationService.Vacation vacation, bool createEdited)
        {
            Owner = owner;
            Vacation = vacation ?? new VacationService.Vacation();
            if (createEdited)
            {
                IsEditMode = true;
            }
        }

        #endregion
        #region Properties

        private VacationsViewModel owner;
        public VacationsViewModel Owner
        {
            get { return owner; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Owner));

                if (owner != null)
                    owner.PropertyChanged -= Owner_PropertyChanged;

                owner = value;

                if (owner != null)
                    owner.PropertyChanged += Owner_PropertyChanged;

                RaisePropertyChanged();
                RaiseAllComamnds();
            }
        }

        private VacationService.Vacation vacation = null;
        public VacationService.Vacation Vacation
        {
            get { return vacation; }
            set
            {
                if (vacation == value)
                    return;

                if (vacation != null)
                    vacation.PropertyChanged -= Vacation_PropertyChanged;

                vacation = value;

                if (vacation != null)
                    vacation.PropertyChanged += Vacation_PropertyChanged;

                RaisePropertyChanged();
            }
        }

        private VacationService.Vacation vacationForEdit = null;
        public VacationService.Vacation VacationForEdit
        {
            get { return vacationForEdit; }
            private set
            {
                if (vacationForEdit == value)
                    return;
                vacationForEdit = value;
                RaisePropertyChanged();
            }
        }

        public Staffing.EmployeeViewModel Employee
        {
            get
            {
                return Owner
                    .Staffing
                    .Employees
                    .Where(e => !e.IsEmpty)
                    .FirstOrDefault(e => e.Employee.Id == (VacationForEdit?.EmployeeId ?? Vacation.EmployeeId));
            }
        }

        public bool IsEmpty
        {
            get { return (Vacation?.Id == 0); }
        }

        private bool isDeleted = false;
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { if (isDeleted == value) return; isDeleted = value; RaisePropertyChanged(); RaiseAllComamnds(); }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            private set { if (isBusy == value) return; isBusy = value; RaisePropertyChanged(); RaiseAllComamnds(); }
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

                if (isEditMode)
                    StartEdit();
                else
                    StopEdit();

                RaisePropertyChanged();
                RaiseAllComamnds();
            }
        }

        public bool HasError { get { return !string.IsNullOrWhiteSpace(Error); } }

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
            internal set { if (error == value) return; error = value; RaisePropertyChanged(); RaisePropertyChanged(() => HasError); RaiseAllComamnds(); }
        }

        #endregion

        private void Owner_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseAllComamnds();
        }

        private void Vacation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseAllComamnds();
            RaisePropertyChanged(() => Employee);
            RaisePropertyChanged(() => IsEmpty);
        }

        private bool GetCanManageCurrentVacation()
        {
            return (owner.CanManageVacations || (owner.Staffing.Current.Id == Vacation.EmployeeId && (Vacation.Agreements?.Length ?? 0) == 0));
        }

        private void RaiseAllComamnds()
        {
            deleteCommand?.RaiseCanExecuteChanged();
            cancelCommand?.RaiseCanExecuteChanged();
            saveCommand?.RaiseCanExecuteChanged();
            editCommand?.RaiseCanExecuteChanged();
        }

        private void StartEdit()
        {
            var vacation = new VacationService.Vacation();
            vacation.CopyObjectFrom(Vacation);
            VacationForEdit = vacation;
        }
        private void StopEdit()
        {
            VacationForEdit = null;
        }

        private string GetExceptionText(string whereCatched, Exception ex)
        {
            return ex.GetExceptionText($"{GetType().Name}.{whereCatched}()"
#if !DEBUG
                , clearText: true, includeData: false, includeStackTrace: false
#endif

                );
        }

        #region Commands

        private DelegateCommand deleteCommand = null;
        public ICommand DeleteCommand { get { return deleteCommand ?? (deleteCommand = new DelegateCommand(o => DeleteAsync(), o => GetCanManageCurrentVacation() && !IsDeleted && !IsBusy && !IsEmpty)); } }

        private DelegateCommand cancelCommand = null;
        public ICommand CancelCommand { get { return cancelCommand ?? (cancelCommand = new DelegateCommand(o => { IsEditMode = false; }, o => !IsDeleted && !IsBusy && IsEditMode)); } }

        private DelegateCommand saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new DelegateCommand(o =>
                {
                    SaveAsync(VacationForEdit);
                }, o => GetCanManageCurrentVacation() && !IsDeleted && !IsBusy && IsEditMode));
            }
        }

        private DelegateCommand editCommand = null;
        public ICommand EditCommand
        {
            get
            {
                return editCommand ?? (editCommand = new DelegateCommand(o =>
                {
                    RaiseOnEditCommandExecuted();
                    IsEditMode = true;
                }, o => GetCanManageCurrentVacation() && !IsDeleted && !IsBusy && !IsEditMode));
            }
        }

        #endregion
        #region Service methods

        private async void DeleteAsync()
        {
            if (Vacation.Id == 0)
            {
                IsDeleted = true;
                return;
            }

            IsBusy = true;
            try
            {
                var sc = new VacationService.VacationServiceClient();
                var waittask = sc.VacationRemoveAsync(Vacation.Id);
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
                                VacationForEdit = null;
                                IsDeleted = true;
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

        private async void SaveAsync(VacationService.Vacation vacationToSave)
        {
            IsBusy = true;
            try
            {
                var task = Task.Factory.StartNew(() => {
                    var sc = new VacationService.VacationServiceClient();
                    try
                    {
                        var updateRes = Vacation.Id == 0
                            ? sc.VacationInsert(vacationToSave)
                            : sc.VacationUpdate(vacationToSave);

                        if (!string.IsNullOrWhiteSpace(updateRes.Error))
                            throw new Exception(updateRes.Error);

                        return updateRes.Value;
                    }
                    finally
                    {
                        try { sc.Close(); } catch { }
                    }
                });

                Vacation.CopyObjectFrom(await task);
                
                Error = null;
                IsBusy = false;
                IsEditMode = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(SaveAsync), ex);
            }
        }

        #endregion
        #region Events

        private void RaiseOnEditCommandExecuted() => OnEditCommandExecuted?.Invoke(this, new EventArgs());

        public event EventHandler OnEditCommandExecuted;

        #endregion
    }
}
