using Helpers.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Personnel.Appn.Additional
{
    public class DragEventEventArgs : EventArgs
    {
        public DependencyObject Source { get; private set; }
        public object Data { get; private set; }
        internal DragEventEventArgs(DependencyObject source, object data)
        {
            Source = source;
            Data = data;
        }
    }

    public static partial class Locator
    {
        private static ICommand startDraggingCommand = null;
        public static ICommand StartDraggingCommand { get { return startDraggingCommand ?? (startDraggingCommand = new DelegateCommand(o =>
        {
            var args = o as EventArgsParameter;
            if (args != null)
            { 
                var e = args.EventArgs as MouseButtonEventArgs;
                if (e == null)
                    throw new ArgumentNullException(nameof(e));

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    BeginDragging?.Invoke(null, new DragEventEventArgs((DependencyObject)e.Source, args.CommandParameter));
                    DragDrop.DoDragDrop((DependencyObject)e.Source, args.CommandParameter, DragDropEffects.Move);
                    EndDragging?.Invoke(null, new DragEventEventArgs((DependencyObject)e.Source, args.CommandParameter));
                }
            }
        })); } }

        private static event EventHandler<DragEventEventArgs> BeginDragging;
        private static event EventHandler<DragEventEventArgs> EndDragging;
    }
}
