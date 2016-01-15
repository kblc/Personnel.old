using Personnel.Application.ViewModels.Additional;
using Personnel.Application.ViewModels.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.Vacation
{
    public class VacationListItemViewModel : NotifyPropertyChangedBase
    {
        #region Constructor

        public VacationListItemViewModel(Staffing.EmployeeViewModel employee, VacationsViewModel owner)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            Employee = employee;
            Owner = owner;
            Balance = owner.VacationBalances.FirstOrDefault(b => b.EmployeeId == Employee.Employee.Id);

            owner.VacationBalances.CollectionChanged += (_, e) =>
            {
                var itm = e.OldItems?
                    .Cast<VacationService.VacationBalance>()
                    .FirstOrDefault(i => i.EmployeeId == employee.Employee.Id);
                if (itm != null)
                    Balance = null;

               itm = e.NewItems?
                    .Cast<VacationService.VacationBalance>()
                    .FirstOrDefault(i => i.EmployeeId == employee.Employee.Id);
                if (itm != null)
                    Balance = itm;
            };

            foreach (var item in Owner.Vacations.Where(v => v.EmployeeId == Employee.Employee.Id))
                vacations.Add(new VacationListItemPartViewModel(this, item));

            Owner.Vacations.CollectionChanged += (_, e) => 
            {
                if (e.OldItems != null)
                    foreach (var oldItem in e.OldItems
                        .Cast<VacationService.Vacation>()
                        .Where(i => i.EmployeeId == employee.Employee.Id)
                        .Join(vacations, v => v, vm => vm.Vacation, (v, vm) => vm))
                        vacations.Remove(oldItem);

                if (e.NewItems != null)
                    foreach (var newItem in e.NewItems
                        .Cast<VacationService.Vacation>()
                        .Where(i => i.EmployeeId == employee.Employee.Id)
                        .Select(i => new VacationListItemPartViewModel(this, i)))
                        vacations.Add(newItem);
            };
        }

        #endregion
        #region Properties

        public VacationsViewModel Owner { get; private set; }
        public Staffing.EmployeeViewModel Employee { get; private set; }
        public VacationService.VacationBalance Balance { get; private set; }

        private NotifyCollection<VacationListItemPartViewModel> vacations = new NotifyCollection<VacationListItemPartViewModel>();
        public IReadOnlyNotifyCollection<VacationListItemPartViewModel> Vacations => vacations;

        #endregion
    }
}
