using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Personnel.Appn.Converters
{
    /*
    public class SizesToHalfPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Point result = new Point(0, 0);

            var sizes = values.Take(2).Cast<double>();
            result.X = sizes.FirstOrDefault() / 2;
            result.Y = sizes.LastOrDefault() / 2;

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }*/

    public class SizesToPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Point result = new Point(0, 0);

            var sizes = values.Take(2).Cast<double>();
            result.X = sizes.FirstOrDefault();
            result.Y = sizes.LastOrDefault();

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
