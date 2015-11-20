using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.Birthdays
{
    public class EmployeeViewModel : Staffing.EmployeeViewModel
    {
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

        /// <summary>
        /// Is birthday gone
        /// </summary>
        public bool IsBirthdayGone
        {
            get { return DayOfBirthday < DateTime.UtcNow; }
        }

        private DateTime dayOfBirthday;
        /// <summary>
        /// Is birthday gone
        /// </summary>
        public DateTime DayOfBirthday
        {
            get { return dayOfBirthday; }
            set { if (dayOfBirthday == value) return; dayOfBirthday = value; RaisePropertyChanged(() => DayOfBirthday); RaisePropertyChanged(() => IsBirthdayGone); }
        }
    }
}
