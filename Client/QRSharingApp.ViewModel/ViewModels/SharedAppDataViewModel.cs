using QRSharingApp.ViewModel.Models;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using System.Windows.Media.Imaging;
using QRSharingApp.ViewModel.ViewModels.Base;
using System.Reactive.Subjects;
using System.Reactive;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class SharedAppDataViewModel : ViewModelBase, ISharedAppDataViewModel
    {
        public ActivationStatus ActivationStatus { get; set; }
        public bool IsPreviewVisible { get; set; }
        public bool IsFilePreviewOpened { get; set; }
        public BitmapImage WifiQRImage { get; set; }
        public BitmapImage WebUrlQRImage { get; set; }
        public Subject<string> NetworkChanged { get; set; }

        public SharedAppDataViewModel()
        {
            NetworkChanged = new Subject<string>();
        }
    }
}
