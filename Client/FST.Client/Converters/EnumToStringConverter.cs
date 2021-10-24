using System;
using System.Globalization;
using System.Windows.Data;

namespace FST.Client.Converters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((int)value) == 0)
            {
                return "-";
            }

            var cloudKind = (Enum)value;
            return cloudKind.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
