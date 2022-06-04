using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QRSharingApp.Client.Converters
{
    public class IsStringsNotEqualToVisibilityConverterInvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var firstValue = value.ToString();
            var secondValue = parameter.ToString();

            return firstValue == secondValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
