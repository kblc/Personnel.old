using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.Staffing
{
    public class EmployeeInfoViewMode : NotifyPropertyChangedBase
    {
        private StaffingService.Employee employee = null;
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

        private StaffingService.Department department = null;
        public StaffingService.Department Department
        {
            get { return department; }
            set
            {
                if (department == value)
                    return;
                department = value;
                RaisePropertyChanged(() => Department);
            }
        }

        private StaffingService.Appoint appoint = null;
        public StaffingService.Appoint Appoint
        {
            get { return appoint; }
            set
            {
                if (appoint == value)
                    return;
                appoint = value;
                RaisePropertyChanged(() => Appoint);
            }
        }

        private StaffingService.Picture photo = null;
        public StaffingService.Picture Photo
        {
            get { return photo; }
            set
            {
                if (photo == value)
                    return;
                photo = value;
                RaisePropertyChanged(() => Photo);
                RaisePropertyChanged(() => HasPhoto);
            }
        }

        public bool HasPhoto { get { return Photo != null; } }
    }
}
