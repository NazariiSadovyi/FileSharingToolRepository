using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FST.Client.Converters
{
    public class PagesToChunkListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value = values[0];
            if (value == null)
            {
                return null;
            }

            var list = value as ICollection<int>;
            var chunkSize = int.Parse(parameter as string);
            var result = list.Select((x, i) => new { Index = i, Value = x })
                             .GroupBy(x => x.Index / chunkSize)
                             .Select(x => x.Select(v => v.Value).ToList())
                             .ToList();

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
