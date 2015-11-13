using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.Birthdays
{
    public class EmployeeViewModel : NotifyPropertyChangedBase
    {
        private StaffingService.Employee employee = null;
        /// <summary>
        /// Employee
        /// </summary>
        public StaffingService.Employee Employee
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

        private int age = default(int);
        /// <summary>
        /// Age at this part
        /// </summary>
        public int Age
        {
            get { return age; }
            set
            {
                if (age == value)
                    return;
                age = value;
                RaisePropertyChanged(() => Age);
                RaisePropertyChanged(() => IsAnniversary);
            }
        }

        /// <summary>
        /// Is anniversary
        /// </summary>
        public bool IsAnniversary { get { return Age % 5 == 0; } }

        private bool isBirthdayGone = true;
        /// <summary>
        /// Is birthday gone
        /// </summary>
        public bool IsBirthdayGone
        {
            get { return isBirthdayGone; }
            set { if (isBirthdayGone == value) return; isBirthdayGone = value; RaisePropertyChanged(() => IsBirthdayGone); }
        }
    }
}
