using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels
{
    public class PhotoFilePreviewViewModel : FilePreviewBaseViewModel
    {
        public BitmapSource Image { get; set; }

        public PhotoFilePreviewViewModel(ThumbnailViewModel thumbnailViewModel)
            : base(thumbnailViewModel) { }

        public override async Task OnLoadDataAsync()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Image = CreateBitmapImage();
                });
            });
        }

        private BitmapImage CreateBitmapImage()
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(FullLocalPath);
            image.EndInit();

            return image;
        }
    }
}
