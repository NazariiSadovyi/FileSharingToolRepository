using FST.ViewModel.ViewModels.Interfaces;
using Prism.Mvvm;
using System.Windows.Media.Imaging;

namespace FST.ViewModel.ViewModels
{
    public class SharedAppDataViewModel : BindableBase, ISharedAppDataViewModel
    {
        private bool _isActivated;

        public bool IsActivated
        {
            get { return _isActivated; }
            set { SetProperty(ref _isActivated, value); }
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
