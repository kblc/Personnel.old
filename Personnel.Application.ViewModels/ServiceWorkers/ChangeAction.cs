using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.ServiceWorkers
{
    public enum ChangeAction
    {
        Add,
        Change,
        Remove
    }

    public class ListItemsEventArgs<T> : EventArgs
    {
        public readonly T[] Items;
        public readonly ChangeAction Action;

        public ListItemsEventArgs(T[] items, ChangeAction action)
        {
            Items = items;
            Action = action;
        }
    }
}
