using QRSharingApp.Infrastructure.Settings.Interfaces;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Unity;

namespace QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels
{
    public abstract class FilePreviewBaseViewModel : ViewModelBase
    {
        [Dependency]
        public IAppSetting AppSetting { get; set; }
        [Dependency]
        public ISharedAppDataViewModel SharedAppDataViewModel { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsLoading { get; set; }
        public string LocalPath { get; set; }
        public string FullLocalPath { get; set; }
        public BitmapImage QRImage { get; set; }
        public string SharedLink { get; set; }

        public string BackgroundImagePath { get; set; }

        public abstract Task OnLoadDataAsync();

        public FilePreviewBaseViewModel(ThumbnailViewModel thumbnailViewModel)
        {
            Id = thumbnailViewModel.Name;
            Name = thumbnailViewModel.Name;
            LocalPath = thumbnailViewModel.LocalPath;
            FullLocalPath = thumbnailViewModel.FullLocalPath;
            QRImage = thumbnailViewModel.QRImage;
            SharedLink = thumbnailViewModel.SharedLink;
        }

        public async Task OnLoadAsync()
        {
            IsLoading = true;
            try
            {
                BackgroundImagePath = AppSetting.BackgroundImagePath;
                await OnLoadDataAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
