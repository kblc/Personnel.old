using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.Staffing
{
    public class EmployeeViewModel : NotifyPropertyChangedBase
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

        private bool isPhotoVisible = true;
        public bool IsPhotoVisible
        {
            get { return isPhotoVisible; }
            set
            {
                if (isPhotoVisible == value)
                    return;
                isPhotoVisible = value;
                RaisePropertyChanged(() => IsPhotoVisible);
            }
        }

        private bool isDepartmentVisible = true;
        public bool IsDepartmentVisible
        {
            get { return isDepartmentVisible; }
            set
            {
                if (isDepartmentVisible == value)
                    return;
                isDepartmentVisible = value;
                RaisePropertyChanged(() => IsDepartmentVisible);
            }
        }

        private bool isAppointVisible = true;
        public bool IsAppointVisible
        {
            get { return isAppointVisible; }
            set
            {
                if (isAppointVisible == value)
                    return;
                isAppointVisible = value;
                RaisePropertyChanged(() => IsAppointVisible);
            }
        }

        public bool HasPhoto { get { return Photo != null; } }
    }
}
