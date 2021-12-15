using System;
using System.Globalization;
using System.Windows.Data;

namespace QRSharingApp.Client.Converters
{
    public class ReversBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _value = (bool)value;
            if (_value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;

        }
    }
}
