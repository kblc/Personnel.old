using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.ServiceWorkers
{
    internal class NotificationStoredItem : AdditionalModels.TempData<NotificationItem>
    {
        public NotificationStoredItem(NotificationItem data, TimeSpan timeToLive) : base(timeToLive, data) { }
    }

    public enum NotificationListAction
    {
        Add,
        Remove
    }

    public class NotificationItemEventArgs : EventArgs
    {
        public NotificationItem[] Items { get; private set; }
        public NotificationListAction Action { get; private set; }
        public NotificationItemEventArgs(NotificationItem[] items, NotificationListAction action)
        {
            Items = items;
            Action = action;
        }
    }

    public class NotificationsWorker : AbstractBaseWorker
    {
        public const int NotificationLifeTimeInSeconds = 15;
        public const int MaxNotificationCount = 50;

        private List<NotificationStoredItem> notifications = new List<NotificationStoredItem>();

        private int maxNotifications = MaxNotificationCount;
        public int MaxNotifications
        {
            get { return maxNotifications; }
            set
            {
                if (maxNotifications == value)
                    return;
                maxNotifications = value;

                var delItems = SetListMaxCount(notifications, MaxNotifications).Select(i => i.Data);
                RaiseOnChanged(delItems, NotificationListAction.Remove);

                RaisePropertyChanged();
                RaiseMaxNotificationsChanged();
            }
        }

        protected override bool DoStart() => true;
        protected override bool DoStop() => true;

        public IEnumerable<NotificationItem> Get()
        {
            lock(notifications)
                return notifications.Select(i => i.Data).AsEnumerable();
        }

        private static IEnumerable<T> SetListMaxCount<T>(IList<T> list, int maxCount)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            while(list.Count > maxCount && list.Count > 0)
            {
                var item = list[0];
                yield return item;
                list.RemoveAt(0);
            }
        }

        internal static Helpers.Linq.GenericEqualityComparer<NotificationItem> GenericComparer
            = Helpers.Linq.GenericEqualityComparer<NotificationItem>.Get(i => new { i.Created, i.Header, i.IconUrl, i.IsError, i.Message }, true);

        private static Helpers.Linq.GenericEqualityComparer<NotificationStoredItem> GenericStoredComparer
            = Helpers.Linq.GenericEqualityComparer<NotificationStoredItem>.Get(i => new { i.Data.Created, i.Data.Header, i.Data.IconUrl, i.Data.IsError, i.Data.Message }, true);

        public void Add(IEnumerable<NotificationItem> items)
        {
            if (State != WorkerState.Started)
                return;

            lock(notifications)
            {
                var addItems = items.ToArray();
                var delStoredItems = SetListMaxCount(notifications, MaxNotifications - addItems.Length);
                var addStoredItems = addItems.Select(i => new NotificationStoredItem(i, ConnectionTimeInterval));
                notifications.AddRange(addStoredItems);
                delStoredItems = delStoredItems.Union(SetListMaxCount(notifications, MaxNotifications));
                var delItems = delStoredItems.Select(i => i.Data).ToArray();
                addItems = addItems.Except(delItems, GenericComparer).ToArray();
                addStoredItems = addStoredItems.Except(delStoredItems, GenericStoredComparer);

                addStoredItems.ToList()
                    .ForEach(i => i.OnEnd += (s,e) => 
                    {
                        lock (notifications)
                        {
                            var itm = s as NotificationStoredItem;
                            if (itm != null && notifications.Contains(itm))
                            { 
                                notifications.Remove(itm);
                                RaiseOnChanged(new[] { itm.Data }, NotificationListAction.Remove);
                            }
                        }
                    });

                RaiseOnChanged(delItems, NotificationListAction.Remove);
                RaiseOnChanged(addItems, NotificationListAction.Add);
            }
        }
        public void Add(NotificationItem item) => Add(new[] { item });

        public void Remove(IEnumerable<NotificationItem> items)
        {
            if (State != WorkerState.Started)
                return;

            lock (notifications)
            {
                var delItemsArray = items
                    .Join(notifications, i => i, n => n.Data, (i, n) => n, GenericComparer)
                    .ToArray();

                foreach (var item in delItemsArray)
                    notifications.Remove(item);
                
                RaiseOnChanged(delItemsArray.Select(i => i.Data), NotificationListAction.Remove);
            }
        }
        public void Remove(NotificationItem item) => Remove(new[] { item });

        private void RaiseOnChanged(IEnumerable<NotificationItem> items, NotificationListAction action) 
            => Context.DoCallBack(() =>
            {
                var itemsArray = items.ToArray();
                if (itemsArray.Length > 0)
                    OnChanged?.Invoke(this, new NotificationItemEventArgs(itemsArray, action));
            }
            );
        private void RaiseMaxNotificationsChanged() 
            => Context.DoCallBack(() => OnMaxNotificationsChanged?.Invoke(this, MaxNotifications));

        private void RaiseNotificationLifeTimeChanged()
            => Context.DoCallBack(() => OnNotificationLifeTimeChanged?.Invoke(this, ConnectionTimeInterval));

        public event EventHandler<NotificationItemEventArgs> OnChanged;
        public event EventHandler<int> OnMaxNotificationsChanged;
        public event EventHandler<TimeSpan> OnNotificationLifeTimeChanged;
    }
}
