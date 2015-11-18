using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.AdditionalModels
{
    public interface IReadOnlyNotifyCollection<out T> 
        : IReadOnlyCollection<T>, INotifyCollectionChanged { }
}
