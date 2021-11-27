using FST.Infrastructure.Models;
using System.Windows.Media.Imaging;

namespace FST.ViewModel.ViewModels.FilePreviewVIewModels
{
    public class PhotoFilePreviewViewModel : FilePreviewBaseViewModel
    {
        private BitmapSource _image;
        public BitmapSource Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        public PhotoFilePreviewViewModel(LocalFile localFile)
            : base(localFile) { }
    }
}
