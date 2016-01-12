using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.Additional
{
    public class ObjectWrapper<T> : INotifyPropertyChanged 
    {
        public ObjectWrapper() { }
        public ObjectWrapper(T value) { Value = value; }

        private T value;
        public T Value { get { return value; } set { this.value = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))); } }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
