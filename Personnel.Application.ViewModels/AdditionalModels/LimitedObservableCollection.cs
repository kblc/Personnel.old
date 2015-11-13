using Helpers.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Windows.Threading;

namespace Personnel.Application.ViewModels.AdditionalModels
{
    public class MTObservableCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        //protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
        //    if (CollectionChanged != null)
        //        foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
        //        {
        //            DispatcherObject dispObj = nh.Target as DispatcherObject;
        //            if (dispObj != null)
        //            {
        //                Dispatcher dispatcher = dispObj.Dispatcher;
        //                if (dispatcher != null && !dispatcher.CheckAccess())
        //                {
        //                    dispatcher.BeginInvoke(
        //                        (Action)(() => nh.Invoke(this,
        //                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
        //                        DispatcherPriority.DataBind);
        //                    continue;
        //                }
        //            }
        //            nh.Invoke(this, e);
        //        }
        //}

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var eh = CollectionChanged;
            if (eh != null)
            {
                var dispatcher = (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
                                    let dpo = nh.Target as DispatcherObject
                                    where dpo != null
                                    select dpo.Dispatcher).FirstOrDefault();

                if (dispatcher != null && dispatcher.CheckAccess() == false)
                {
                    dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
                }
                else
                {
                    foreach (NotifyCollectionChangedEventHandler nh in eh.GetInvocationList())
                        nh.Invoke(this, e);
                }
            }
        }

    }

    public class LimitedObservableCollection<T> : MTObservableCollection<T>
    {
        private long maxItemsCount = 5;
        public long MaxItemsCount
        {
            get { return maxItemsCount; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException(nameof(value));

                if (maxItemsCount == value)
                    return;
                maxItemsCount = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(MaxItemsCount)));
                ClearToMaxCount();
            }
        }

        public LimitedObservableCollection() : this(5) { }

        public LimitedObservableCollection(long maxItemsCount)
        {
            MaxItemsCount = maxItemsCount;
        }

        private int ClearToMaxCount()
        {
            var res = 0;
            while (Count > 0 && Count > maxItemsCount - 1)
            { 
                RemoveAt(0);
                res++;
            }
            return res;
        }

        protected override void InsertItem(int index, T item)
        {
            var delCount = ClearToMaxCount();
            base.InsertItem(index - delCount, item);
        }

        //protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    base.OnCollectionChanged(e);
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        while (Count > 0 && Count + e.NewItems.Count - 1 > MaxItemsCount)
        //            RemoveAt(0);
        //    }
        //}
    }
}
