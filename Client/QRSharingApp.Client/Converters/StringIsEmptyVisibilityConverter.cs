using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QRSharingApp.Client.Converters
{
    public class StringIsEmptyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _value = (string)value;
            if (string.IsNullOrEmpty(_value))
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
