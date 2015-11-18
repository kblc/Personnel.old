using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using Personnel.Application.ViewModels.Notifications;
using System.Globalization;

namespace Personnel.Application.ViewModels.History
{
    public class ViewModel : Additional.AbstractBaseViewModel
    {
        private void RaiseExceptionCatched(Exception ex) => Static.Notifications.AddExceptionNotification(ex);
        private void RaiseHistoryChanged(HistoryService.History history)
        {
            (new NotificationDataViewModel[] { })
                .Union(RaiseAddNotificationForChange(history.Add, Properties.Resources.NOTIFICATION))
                .Union(RaiseAddNotificationForChange(history.Change, Properties.Resources.NOTIFICATION))
                .Union(RaiseAddNotificationForRemove(history.Remove, Properties.Resources.NOTIFICATION))
                .ToList()
                .ForEach(n => Static.Notifications.Notifications.Add(new Notifications.NotificationItemViewModel(n)));
            Changed?.Invoke(this, history);
        }
        private void RaiseIsLoaded(bool value)
        {
            IsLoaded = value;
        }
        private void RaiseIsError(bool value)
        {
            IsError = value;
        }
        private void RaiseIsWaiting(bool value)
        {
            IsWaiting = value;
        }
        private void RaiseIsConnecting(bool value)
        {
            IsConnecting = value;
        }

        private NotificationDataViewModel[] RaiseAddNotificationForChange(HistoryService.HistoryUpdateInfo updateInfo, string defaultHeader, string message = null)
        {
            if (updateInfo == null)
                return new NotificationDataViewModel[] { };

            var getArrayItems = new Func<object, Type[]>((obj) =>
            {
                if (obj == null)
                    return new Type[] { };
                return obj
                    .GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType.IsArray)
                    .Where(p => p.GetValue(obj) != null)
                    .Select(p => p.PropertyType.GetElementType())
                    .ToArray();
            });

            return getArrayItems(updateInfo)
                .Select(t => new NotificationDataViewModel(Static.Notifications.GetEntityNameForType(t.Name) ?? defaultHeader, message, false, Static.Notifications.GetIconForType(t.Name)))
                .ToArray();
        }
        private NotificationDataViewModel[] RaiseAddNotificationForRemove(HistoryService.HistoryRemoveInfo removeInfo, string defaultHeader, string message = null)
        {
            if (removeInfo == null)
                return new NotificationDataViewModel[] { };

            var getArrayItems = new Func<object, string[]>((obj) =>
            {
                if (obj == null)
                    return new string[] { };
                return obj
                    .GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType.IsArray)
                    .Where(p => p.GetValue(obj) != null)
                    .Select(p => p.Name)
                    .ToArray();
            });

            return getArrayItems(removeInfo)
                .Select(t => new NotificationDataViewModel(Static.Notifications.GetEntityNameForType(t) ?? defaultHeader, message, false, Static.Notifications.GetIconForType(t)))
                .ToArray();
        }

        private Thread historyThread = null;
        protected override void Init()
        {
            historyThread = new Thread(new ParameterizedThreadStart(InfinityHistoryThread));
            historyThread.IsBackground = true;
            historyThread.Start(Context);
        }
        protected override void OnDisposing()
        {
            if (historyThread != null)
            {
                historyThread.Abort();
                historyThread = null;
            }
        }

        private CultureInfo serviceCultureInfo = Thread.CurrentThread.CurrentUICulture;
        public CultureInfo ServiceCultureInfo { get { return serviceCultureInfo; } protected set { if (serviceCultureInfo == value) return; serviceCultureInfo = value; RaisePropertyChanged(() => ServiceCultureInfo); } }

        private TimeSpan connectionTimeInterval = TimeSpan.FromSeconds(5);
        public TimeSpan ConnectionTimeInterval { get { return connectionTimeInterval; } protected set { if (connectionTimeInterval == value) return; connectionTimeInterval = value; RaisePropertyChanged(() => ConnectionTimeInterval); } }

        private bool isError = false;
        public bool IsError { get { return isError; } protected set { if (isError == value) return; isError = value; RaisePropertyChanged(() => IsError); } }

        private bool isWaiting = false;
        public bool IsWaiting { get { return isWaiting; } protected set { if (isWaiting == value) return; isWaiting = value; RaisePropertyChanged(() => IsWaiting); } }

        private bool isConnecting = false;
        public bool IsConnecting { get { return isConnecting; } protected set { if (isConnecting == value) return; isConnecting = value; RaisePropertyChanged(() => IsConnecting); } }

        private void InfinityHistoryThread(object obj)
        {
            var modelLevelThContext = (System.Runtime.Remoting.Contexts.Context)obj;
            long eventId = 0;

            #region Infinity try to connect
            bool inited = false;
            do
            {
                try
                {
                    using (var hClient = new HistoryService.HistoryServiceClient())
                    {
                        hClient.ChangeLanguage(serviceCultureInfo.Name);

                        var initRes = hClient.Get();
                        if (!string.IsNullOrEmpty(initRes.Error))
                            throw new Exception(initRes.Error);
                        eventId = initRes.Value.EventId;
                        inited = true;
                    }
                }
                catch (Exception ex)
                {
                    modelLevelThContext.DoCallBack(() => RaiseExceptionCatched(ex));
                    modelLevelThContext.DoCallBack(() => RaiseIsError(true));
                    Thread.Sleep(connectionTimeInterval);
                }
            } while (!inited);

            #endregion

            modelLevelThContext.DoCallBack(() => RaiseIsLoaded(true));
            modelLevelThContext.DoCallBack(() => RaiseIsError(false));

            #region Infinity work after connect
            do
            {
                modelLevelThContext.DoCallBack(() => RaiseIsConnecting(true));
                try
                {
                    using (var hClient = new HistoryService.HistoryServiceClient())
                        while (true)
                            try
                            {
                                hClient.ChangeLanguage(serviceCultureInfo.Name);

                                modelLevelThContext.DoCallBack(() => RaiseIsConnecting(false));
                                modelLevelThContext.DoCallBack(() => RaiseIsWaiting(true));

                                var initRes = hClient.GetFrom(eventId);

                                if (!string.IsNullOrEmpty(initRes.Error))
                                    throw new Exception(initRes.Error);

                                eventId = initRes.Value.EventId;
                                modelLevelThContext.DoCallBack(() => RaiseIsError(false));
                                modelLevelThContext.DoCallBack(() => RaiseHistoryChanged(initRes.Value));
                            }
                            catch (TimeoutException) { modelLevelThContext.DoCallBack(() => RaiseIsConnecting(true)); }
                }
                catch (Exception ex)
                {
                    modelLevelThContext.DoCallBack(() => RaiseIsConnecting(true));
                    modelLevelThContext.DoCallBack(() => RaiseIsError(true));
                    modelLevelThContext.DoCallBack(() => RaiseIsWaiting(false));
                    modelLevelThContext.DoCallBack(() => RaiseExceptionCatched(ex));
                    Thread.Sleep(connectionTimeInterval);
                }
            } while (true);
            #endregion
        }

        public event EventHandler<HistoryService.History> Changed;
    }
}
