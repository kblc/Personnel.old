using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Personnel.Appn.Converters
{
    public class ImagePathToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var image = new BitmapImage();
            var str = value as string;
            if (!string.IsNullOrWhiteSpace(str))
            {
                image.BeginInit();
                if (!string.IsNullOrWhiteSpace(str))
                {
                    image.UriSource = new Uri((string)value);
                }
                image.DecodePixelWidth = 50;
                image.EndInit();
            }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
