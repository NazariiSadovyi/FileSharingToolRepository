using FST.Infrastructure.Models;

namespace FST.ViewModel.ViewModels.FilePreviewVIewModels
{
    public class VideoFilePreviewViewModel : FilePreviewBaseViewModel
    {
        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { SetProperty(ref _isPlaying, value); }
        }

        public VideoFilePreviewViewModel(LocalFile localFile)
            : base(localFile) { }
    }
}
