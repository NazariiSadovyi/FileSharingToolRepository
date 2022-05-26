using QRSharingApp.Infrastructure.Models;
using System.Windows.Media.Imaging;

namespace QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels
{
    public class PhotoFilePreviewViewModel : FilePreviewBaseViewModel
    {
        public BitmapSource Image { get; set; }

        public PhotoFilePreviewViewModel(LocalFile localFile)
            : base(localFile) { }
    }
}
