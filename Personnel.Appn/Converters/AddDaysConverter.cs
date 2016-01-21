using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Personnel.Appn.Converters
{
    public class AddDaysConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return null;

            return ((DateTime)values.First()) + TimeSpan.FromDays(
                values
                .Skip(1)
                .Select(o => 
                {
                    int res = 0;
                    if (int.TryParse(o?.ToString() ?? "0", out res))
                        return res;
                    else
                        return 0;
                })
                .Union(new[] { 0 })
                .Sum()
                );
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return ((DateTime)value) + TimeSpan.FromDays(int.Parse((parameter?.ToString() ?? 0.ToString())));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
