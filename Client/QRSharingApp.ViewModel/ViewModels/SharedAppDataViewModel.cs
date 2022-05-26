using QRSharingApp.ViewModel.Models;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class SharedAppDataViewModel : ReactiveObject, ISharedAppDataViewModel
    {
        public ActivationStatus ActivationStatus { get; set; }
        public bool IsPreviewVisible { get; set; }
        public BitmapImage WifiQRImage { get; set; }
    }
}
