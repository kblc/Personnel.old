using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.Birthdays
{
    public class LevelViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Time parts
        /// </summary>
        public ObservableCollection<TimePartViewModel> Parts { get; }
            = new ObservableCollection<TimePartViewModel>();

        private string name = string.Empty;
        /// <summary>
        /// Level name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { if (name == value) return; name = value; RaisePropertyChanged(() => Name); }
        }

        private TimeSpan period;
        /// <summary>
        /// Level period
        /// </summary>
        public TimeSpan Period
        {
            get { return period; }
            set { if (period == value) return; period = value; RaisePropertyChanged(() => Period); }
        }

    }
}
