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

        public bool IsLoading { get; set; }
        public ThumbnailViewModel ThumbnailViewModel { get; set; }
        public string BackgroundImagePath { get; set; }

        public abstract Task OnLoadDataAsync();

        public FilePreviewBaseViewModel(ThumbnailViewModel thumbnailViewModel)
        {
            ThumbnailViewModel = thumbnailViewModel;
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
