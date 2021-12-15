using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace QRSharingApp.Client.Converters
{
    public class StringToStarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var textLength = ((string)value).Length;
            var stars = Enumerable.Range(0, textLength).Select(_ => '*');
            var starsText = string.Join(string.Empty, stars);

            return starsText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
