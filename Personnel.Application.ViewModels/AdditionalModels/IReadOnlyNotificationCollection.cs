using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.AdditionalModels
{
    public interface INotifyCollection<T>
       : ICollection<T>,
         INotifyCollectionChanged
    { }

    public interface IReadOnlyNotifyCollection<out T>
           : IReadOnlyCollection<T>,
             INotifyCollectionChanged
    { }

    public class NotifyCollection<T> 
        : ObservableCollection<T>, INotifyCollection<T>, IReadOnlyNotifyCollection<T>
    { }
}
