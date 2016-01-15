using Helpers.WPF;
using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Personnel.Application.ViewModels.Vacation
{
    public class VacationListItemPartViewModel : NotifyPropertyChangedBase
    {
        #region Constructor

        public VacationListItemPartViewModel(VacationListItemViewModel owner, VacationService.Vacation vacation)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));
            if (vacation == null)
                throw new ArgumentNullException(nameof(vacation));

            Owner = owner;
            Vacation = vacation;

            Owner.Owner.PropertyChanged += (_, e) => 
            {
                if (e.PropertyName == nameof(VacationsViewModel.CanManageVacations))
                {
                    RaisePropertyChanged(() => CanManage);
                    UpdateCommands();
                }
            };

            Owner.Owner.Staffing.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Staffing.StaffingViewModel.Current))
                { 
                    RaisePropertyChanged(() => CanManage);
                    UpdateCommands();
                }
            };
        }

        #endregion
        #region Properties

        public VacationListItemViewModel Owner { get; private set; }
        public VacationService.Vacation Vacation { get; private set; }

        private bool GetCanManage()
        {
            return (Owner.Owner.CanManageVacations || Owner.Owner.Staffing.Current.Id == Vacation.EmployeeId) && !IsDeleted && !IsBusy;
        }

        public bool CanManage { get { return GetCanManage(); } }

        private bool isBusy = false;
        public bool IsBusy { get { return isBusy; } set { isBusy = value; RaisePropertyChanged(); UpdateCommands(); } }

        private bool isDeleted = false;
        public bool IsDeleted { get { return isDeleted; } set { isDeleted = value; RaisePropertyChanged(); UpdateCommands(); } }

        private string error = string.Empty;
        public string Error { get { return error; } set { error = value; RaisePropertyChanged(); RaisePropertyChanged(() => HasError); UpdateCommands(); } }

        public bool HasError { get { return !string.IsNullOrWhiteSpace(error); } }

        private void UpdateCommands()
        {
            removeCommand?.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => CanManage);
        }

        private DelegateCommand removeCommand = null;
        public ICommand RemoveCommand { get { return removeCommand ?? (removeCommand = new DelegateCommand(o => DeleteAsync(Vacation.Id), o => CanManage && !IsBusy && !IsDeleted)); } }

        public async void DeleteAsync(long vacationId)
        {
            IsBusy = true;
            try
            {
                var srvc = new VacationService.VacationServiceClient();
                var resultTask = srvc.VacationRemoveAsync(vacationId).ContinueWith((t) => 
                    {
                        if (t.Exception != null)
                            throw t.Exception;

                        if (!string.IsNullOrEmpty(t.Result.Error))
                            throw new Exception(t.Result.Error);
                    }, 
                    CancellationToken.None, TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());

                await resultTask;

                IsDeleted = true;
            }
            catch(Exception ex)
            {
                Error = ex.ToString();
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}
