using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Personnel.Appn.Converters
{
    public class MaxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intParam = 0;
            int intValue = 0;
            int.TryParse((parameter?.ToString() ?? 0.ToString()), out intParam);
            int.TryParse((value?.ToString() ?? 0.ToString()), out intValue);

            return Math.Max(intParam, intValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
