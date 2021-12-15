using QRSharingApp.ViewModel.Models;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace QRSharingApp.ViewModel.ViewModels.Interfaces
{
    public interface ISharedAppDataViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;
        ActivationStatus ActivationStatus { get; set; }
        bool IsPreviewVisible { get; set; }
        BitmapImage WifiQRImage { get; set; }
    }
}