using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Personnel.Application.ViewModels.Notifications
{
    public class NotificationDataViewModel
    {
        public NotificationDataViewModel(ServiceWorkers.NotificationItem notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));
            Notification = notification;
        }

        public ServiceWorkers.NotificationItem Notification { get; private set; }

        /// <summary>
        /// Has notification icon
        /// </summary>
        public bool HasIcon => !string.IsNullOrWhiteSpace(Notification.IconUrl);

        /// <summary>
        /// Has notification body message
        /// </summary>
        public bool HasMessage => !string.IsNullOrWhiteSpace(Notification.Message);

        /// <summary>
        /// Close command
        /// </summary>
        private ICommand closeCommand = null;
        public ICommand CloseCommand { get { return closeCommand ?? (closeCommand = new Helpers.WPF.DelegateCommand((o) => OnCloseClick?.Invoke(this, new EventArgs()))); } }

        /// <summary>
        /// Open command
        /// </summary>
        private ICommand openCommand = null;
        public ICommand OpenCommand { get { return openCommand ?? (openCommand = new Helpers.WPF.DelegateCommand((o) => OnOpenClick?.Invoke(this, new EventArgs()))); } }

        public event EventHandler OnCloseClick;
        public event EventHandler OnOpenClick;
    }
}
