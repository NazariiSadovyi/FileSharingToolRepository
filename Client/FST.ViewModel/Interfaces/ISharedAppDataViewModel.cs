using System.Windows.Media.Imaging;

namespace FST.ViewModel.ViewModels.Interfaces
{
    public interface ISharedAppDataViewModel
    {
        bool IsActivated { get; set; }
        bool IsPreviewVisible { get; set; }
        BitmapImage WifiQRImage { get; set; }
    }
}