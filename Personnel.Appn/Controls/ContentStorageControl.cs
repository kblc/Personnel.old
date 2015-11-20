using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.WPF;
using System.Windows;

namespace Personnel.Appn.Controls
{
    public class ContentStorageControl : DependencyObject
    {
        #region Caption

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(nameof(Caption), typeof(object),
            typeof(ContentStorageControl), new PropertyMetadata(default(string)));

        public object Caption
        {
            get { return (string)this.GetValue(CaptionProperty); }
            set { this.SetValue(CaptionProperty, value); }
        }

        #endregion
        #region Header

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object),
            typeof(ContentStorageControl), new PropertyMetadata(default(object)));

        public object Header
        {
            get { return this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        #endregion
        #region Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object),
            typeof(ContentStorageControl), new PropertyMetadata(default(object)));

        public object Content
        {
            get { return this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        #endregion
        #region IsLoaded

        public static readonly DependencyProperty IsLoadedProperty = DependencyProperty.Register(nameof(IsLoaded), typeof(bool),
            typeof(ContentStorageControl), new PropertyMetadata(default(bool)));

        public bool IsLoaded
        {
            get { return (bool)this.GetValue(IsLoadedProperty); }
            set { this.SetValue(IsLoadedProperty, value); }
        }

        #endregion
    }
}
