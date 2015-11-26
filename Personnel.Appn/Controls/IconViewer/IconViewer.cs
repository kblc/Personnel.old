using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Personnel.Appn.Controls
{
    public enum ViewStyle
    {
        IconTop,
        IconLeft
    }

    public class IconViewer : Control
    {
        static IconViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconViewer), 
                new FrameworkPropertyMetadata(typeof(IconViewer)));
        }

        public IconViewer()
        {
            DefaultStyleKey = typeof(IconViewer);
        }

        #region HorizontalHeaderAlignment

        public static readonly DependencyProperty HorizontalHeaderAlignmentProperty = DependencyProperty.Register(nameof(HorizontalHeaderAlignment), typeof(HorizontalAlignment),
            typeof(IconViewer), new PropertyMetadata(HorizontalAlignment.Left, (s, e) => { }));

        [Bindable(true)]
        [Category("Layout")]
        public HorizontalAlignment HorizontalHeaderAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalHeaderAlignmentProperty); }
            set { SetValue(HorizontalHeaderAlignmentProperty, value); }
        }

        #endregion
        #region VerticalHeaderAlignment

        public static readonly DependencyProperty VerticalHeaderAlignmentProperty = DependencyProperty.Register(nameof(VerticalHeaderAlignment), typeof(VerticalAlignment),
            typeof(IconViewer), new PropertyMetadata(VerticalAlignment.Top, (s, e) => { }));

        [Bindable(true)]
        [Category("Layout")]
        public VerticalAlignment VerticalHeaderAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalHeaderAlignmentProperty); }
            set { SetValue(VerticalHeaderAlignmentProperty, value); }
        }

        #endregion
        #region Image

        public static readonly DependencyProperty PictureProperty = DependencyProperty.Register(nameof(Image), typeof(ImageSource),
            typeof(IconViewer), new PropertyMetadata(null, (s, e) => { }));

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(PictureProperty); }
            set { SetValue(PictureProperty, value); }
        }

        #endregion
        #region ViewStyle

        public static readonly DependencyProperty ViewStyleProperty = DependencyProperty.Register(nameof(ViewStyle), typeof(ViewStyle),
            typeof(IconViewer), new PropertyMetadata(ViewStyle.IconLeft, (s, e) => { }));

        public ViewStyle ViewStyle
        {
            get { return (ViewStyle)GetValue(ViewStyleProperty); }
            set { SetValue(ViewStyleProperty, value); }
        }

        #endregion
        #region Header

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object),
            typeof(IconViewer), new PropertyMetadata(null, (s, e) => { }));

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        #endregion
        #region Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object),
            typeof(IconViewer), new PropertyMetadata(null, (s, e) => { }));

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion
        #region IsImageVisible

        public static readonly DependencyProperty IsImageVisibleProperty = DependencyProperty.Register(nameof(IsImageVisible), typeof(bool),
            typeof(IconViewer), new PropertyMetadata(true, (s, e) => { }));

        public bool IsImageVisible
        {
            get { return (bool)GetValue(IsImageVisibleProperty); }
            set { SetValue(IsImageVisibleProperty, value); }
        }

        #endregion
    }
}
