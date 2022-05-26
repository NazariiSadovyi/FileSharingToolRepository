using QRSharingApp.ViewModel.Models;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using Prism.Mvvm;
using System.Windows.Media.Imaging;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class SharedAppDataViewModel : BindableBase, ISharedAppDataViewModel
    {
        public ActivationStatus ActivationStatus { get; set; }
        public bool IsPreviewVisible { get; set; }
        public BitmapImage WifiQRImage { get; set; }
    }
}
