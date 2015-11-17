using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Personnel.Appn.Converters
{
    public class ScaleFromActualSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resD = new ResourceDictionary() { Source = new Uri("/GeoControls;component/Styles/ShiftButtonStyle.xaml", UriKind.Relative) };
            var res = resD["drawSize"];
            if (res != null)
            {
                return (double)value / (double)res;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ScaleFromActualSizesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            { 
                double value = values.Cast<double>().FirstOrDefault();
                double drawSize = values.Cast<double>().LastOrDefault();
                return value / drawSize;
            }
            catch { return 1.0; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
