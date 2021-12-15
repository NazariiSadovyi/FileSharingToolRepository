using QRSharingApp.ViewModel.Models;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using Prism.Mvvm;
using System.Windows.Media.Imaging;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class SharedAppDataViewModel : BindableBase, ISharedAppDataViewModel
    {
        private ActivationStatus _activationStatus;

        public ActivationStatus ActivationStatus
        {
            get { return _activationStatus; }
            set { SetProperty(ref _activationStatus, value); }
        }

        private bool _isPreviewVisible;

        public bool IsPreviewVisible
        {
            get { return _isPreviewVisible; }
            set { SetProperty(ref _isPreviewVisible, value); }
        }

        private BitmapImage _wifiQRImage;
        public BitmapImage WifiQRImage
        {
            get { return _wifiQRImage; }
            set { SetProperty(ref _wifiQRImage, value); }
        }
    }
}
