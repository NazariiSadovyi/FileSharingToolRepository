using System.Threading.Tasks;

namespace QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels
{
    public class VideoFilePreviewViewModel : FilePreviewBaseViewModel
    {
        public bool IsPlaying { get; set; }

        public VideoFilePreviewViewModel(ThumbnailViewModel thumbnailViewModel)
            : base(thumbnailViewModel) { }

        public override Task OnLoadDataAsync()
        {
            return Task.CompletedTask;
        }
    }
}
