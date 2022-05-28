using QRSharingApp.ViewModel.Models;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using System.Windows.Media.Imaging;
using QRSharingApp.ViewModel.ViewModels.Base;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class SharedAppDataViewModel : ViewModelBase, ISharedAppDataViewModel
    {
        public ActivationStatus ActivationStatus { get; set; }
        public bool IsPreviewVisible { get; set; }
        public BitmapImage WifiQRImage { get; set; }
    }
}
