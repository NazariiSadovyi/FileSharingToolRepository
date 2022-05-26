using QRSharingApp.Infrastructure.Models;

namespace QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels
{
    public class VideoFilePreviewViewModel : FilePreviewBaseViewModel
    {
        public bool IsPlaying { get; set; }

        public VideoFilePreviewViewModel(LocalFile localFile)
            : base(localFile) { }
    }
}
