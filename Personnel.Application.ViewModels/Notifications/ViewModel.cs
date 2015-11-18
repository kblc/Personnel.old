using Personnel.Application.ViewModels.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Helpers.WPF;
using System.Windows.Input;

namespace Personnel.Application.ViewModels.Notifications
{
    public class ViewModel : Additional.AbstractBaseViewModel
    {
        private static IDictionary<Type, string> IconUrlForTypes = new Dictionary<Type, string>()
        {
            { typeof(HistoryService.Appoint) , "" },
            { typeof(HistoryService.Department) , "" },
            { typeof(HistoryService.Employee) , "" },
            { typeof(HistoryService.EmployeeLogin) , "" },
            { typeof(HistoryService.EmployeePhoto) , "" },
            { typeof(HistoryService.EmployeeRight) , "" },
            { typeof(HistoryService.File) , "" },
            { typeof(HistoryService.Picture) , "" },
            { typeof(HistoryService.Staffing) , "" },
        };

        private static IDictionary<Type, string> EntityNamesForTypes = new Dictionary<Type, string>()
        {
            { typeof(HistoryService.Appoint) , "" },
            { typeof(HistoryService.Department) , "" },
            { typeof(HistoryService.Employee) , "" },
            { typeof(HistoryService.EmployeeLogin) , "" },
            { typeof(HistoryService.EmployeePhoto) , "" },
            { typeof(HistoryService.EmployeeRight) , "" },
            { typeof(HistoryService.File) , "" },
            { typeof(HistoryService.Picture) , "" },
            { typeof(HistoryService.Staffing) , "" },
        };

        public const string IconUrlError = "";

        public LimitedObservableCollection<NotificationItemViewModel> Notifications { get; } = new LimitedObservableCollection<NotificationItemViewModel>(15);

        private NotificationItemViewModel selectedNotification = null;
        public NotificationItemViewModel SelectedNotification
        {
            get { return selectedNotification; }
            private set
            {
                if (selectedNotification == value)
                    return;
                selectedNotification = value;
                RaisePropertyChanged(() => SelectedNotification);
            }
        }

        private void ChangeSelectedTo(NotificationItemViewModel selected)
        {
            SelectedNotification = selected;
            foreach (var n in Notifications)
                n.IsSelected = n == SelectedNotification;
        }

        private DelegateCommand clearSelectionCommand = null;
        public ICommand ClearSelectionCommand { get { return clearSelectionCommand ?? (clearSelectionCommand = new DelegateCommand((o) => ChangeSelectedTo(null))); } }

        protected override void Init()
        {
            var getItemFunc = new Func<NotificationDataViewModel, NotificationItemViewModel>((data) => Notifications.Where(i => i.Data == data).FirstOrDefault());
            var removeFunc = new Action<NotificationItemViewModel, bool>((itm, andSelected) =>
            {
                if (itm != null)
                    try
                    {
                        if (Notifications.Contains(itm))
                            Notifications.Remove(itm);

                        if (andSelected && SelectedNotification == itm)
                            ChangeSelectedTo(null);
                    }
                    catch { }
            });

            Notifications.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (var item in e.NewItems.OfType<NotificationItemViewModel>())
                    {
                        item.OnEnd += (s2, e2) => removeFunc(s2 as NotificationItemViewModel, false);
                        if (item.Data != null)
                        { 
                            item.Data.OnCloseClick += (s2, e2) => removeFunc(getItemFunc(s2 as NotificationDataViewModel), true);
                            item.Data.OnOpenClick += (s2, e2) =>
                            {
                                var itm = getItemFunc(s2 as NotificationDataViewModel);
                                ChangeSelectedTo(itm);
                                ShowNotification?.Invoke(this, itm);
                            };
                        }
                    }
            };
            IsLoaded = true;
        }

        internal void AddExceptionNotification(Exception ex) => AddExceptionNotification(ex, null);
        internal void AddExceptionNotification(Exception ex, string entityTypeName)
        {
            var notification = new NotificationDataViewModel(
                GetEntityNameForType(entityTypeName) ?? Properties.Resources.ERROR, 
                ex.GetExceptionText(), 
                true,
                GetIconForType(entityTypeName) ?? IconUrlError);

            var data = new NotificationItemViewModel(notification);
            Static.Notifications.Notifications.Add(data);
        }

        public event EventHandler<NotificationItemViewModel> ShowNotification;

        public string GetIconForType(string typeName)
        {
            var keyType = IconUrlForTypes.Keys.FirstOrDefault(k => string.Compare(k.Name, typeName, true) == 0);
            if (keyType != null)
                return IconUrlForTypes[keyType];
            return null;
        }

        public string GetEntityNameForType(string typeName)
        {
            var keyType = IconUrlForTypes.Keys.FirstOrDefault(k => string.Compare(k.Name, typeName, true) == 0);
            if (keyType != null)
                return IconUrlForTypes[keyType];
            return null;
        }
    }
}
