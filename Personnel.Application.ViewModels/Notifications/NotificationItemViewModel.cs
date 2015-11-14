using Personnel.Application.ViewModels.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.Notifications
{
    public class NotificationItemViewModel : AdditionalModels.TempData<NotificationDataViewModel>
    {
        public NotificationItemViewModel() : base() { }
        public NotificationItemViewModel(NotificationDataViewModel data) : base(TimeSpan.FromSeconds(15), data) { }

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected == value)
                    return;
                value = isSelected;
                RaisePropertyChanged(() => IsSelected);
            }
        }

        private DateTime created = DateTime.Now;
        public DateTime Created
        {
            get { return created; }
            set
            {
                if (created == value)
                    return; created = value;
                RaisePropertyChanged(() => Created);
            }
        }
    }
}
