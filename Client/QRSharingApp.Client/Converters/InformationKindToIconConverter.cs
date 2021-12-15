using QRSharingApp.ViewModel.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows.Data;

namespace QRSharingApp.Client.Converters
{
    public class InformationKindToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var informationKind = (InformationKind)value;
            switch (informationKind)
            {
                case InformationKind.Info:
                    return PackIconKind.Information;
                case InformationKind.Success:
                    return PackIconKind.CheckboxMarkedCircle;
                case InformationKind.Error:
                    return PackIconKind.CloseCircle;
                case InformationKind.Warning:
                    return PackIconKind.AlertOutline;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
