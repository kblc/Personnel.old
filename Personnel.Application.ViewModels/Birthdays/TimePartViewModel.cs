using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.Birthdays
{
    public class TimePartViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Employees
        /// </summary>
        public ObservableCollection<EmployeeViewModel> Employee { get; }
            = new ObservableCollection<EmployeeViewModel>();

        private bool isCurrent = false;
        /// <summary>
        /// Is current day part
        /// </summary>
        public bool IsCurrent
        {
            get { return isCurrent; }
            set { if (isCurrent == value) return; isCurrent = value; RaisePropertyChanged(() => IsCurrent); }
        }

        private DateTime start;
        /// <summary>
        /// Part start
        /// </summary>
        public DateTime Start
        {
            get { return start; }
            set { if (start == value) return; start = value; RaisePropertyChanged(() => Start); }
        }

        private DateTime end;
        /// <summary>
        /// Part end
        /// </summary>
        public DateTime End
        {
            get { return end; }
            set { if (end == value) return; end = value; RaisePropertyChanged(() => End); }
        }

        private string name = string.Empty;
        /// <summary>
        /// Part name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { if (name == value) return; name = value; RaisePropertyChanged(() => Name); }
        }
    }
}
