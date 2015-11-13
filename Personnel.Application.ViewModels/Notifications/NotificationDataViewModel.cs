using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Personnel.Application.ViewModels.Notifications
{
    public class NotificationDataViewModel : NotifyPropertyChangedBase
    {
        public NotificationDataViewModel(string header, string message, bool isError, string iconUrl)
        {
            this.header = header;
            this.message = message;
            this.isError = isError;
            this.iconUrl = iconUrl;
            OpenCommand = new Helpers.WPF.DelegateCommand((o) => OnOpenCommand?.Invoke(this, new EventArgs()));
            CloseCommand = new Helpers.WPF.DelegateCommand((o) => OnCloseCommand?.Invoke(this, new EventArgs()));
        }

        private string header = string.Empty;
        /// <summary>
        /// Notification header
        /// </summary>
        public string Header { get { return header; } private set { if (header == value) return; header = value; RaisePropertyChanged(() => Header); } }

        private string message = string.Empty;
        /// <summary>
        /// Notification body message
        /// </summary>
        public string Message { get { return message; } private set { if (message == value) return; message = value; RaisePropertyChanged(() => Message); } }

        private bool isError = false;
        /// <summary>
        /// Is error notification
        /// </summary>
        public bool IsError { get { return isError; } private set { if (isError == value) return; isError = value; RaisePropertyChanged(() => IsError); } }

        private string iconUrl = null;
        /// <summary>
        /// Icon url
        /// </summary>
        public string IconUrl { get { return iconUrl; } private set { if (iconUrl == value) return; iconUrl = value; RaisePropertyChanged(() => IconUrl); RaisePropertyChanged(() => HasIcon); } }

        /// <summary>
        /// Has notification icon
        /// </summary>
        public bool HasIcon
        {
            get
            {
                return !string.IsNullOrWhiteSpace(IconUrl);
            }
        }

        /// <summary>
        /// Has notification body message
        /// </summary>
        public bool HasMessage { get { return !string.IsNullOrWhiteSpace(message); } }

        /// <summary>
        /// Close command
        /// </summary>
        private ICommand closeCommand = null;
        public ICommand CloseCommand { get { return closeCommand; } private set { if (closeCommand == value) return; closeCommand = value; RaisePropertyChanged(() => CloseCommand); } }

        /// <summary>
        /// Open command
        /// </summary>
        private ICommand openCommand = null;
        public ICommand OpenCommand { get { return openCommand; } private set { if (openCommand == value) return; openCommand = value; RaisePropertyChanged(() => OpenCommand); } }

        public event EventHandler OnCloseCommand;
        public event EventHandler OnOpenCommand;
    }
}
