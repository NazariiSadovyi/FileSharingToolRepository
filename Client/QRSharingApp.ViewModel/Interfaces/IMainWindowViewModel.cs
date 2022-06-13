using QRSharingApp.ViewModel.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace QRSharingApp.ViewModel.Interfaces
{
    public interface IMainWindowViewModel
    {
        WindowStyle WindowStyle { get; set; }
        WindowState WindowState { get; set; }
        ResizeMode ResizeMode { get; set; }

        Task OnLoadAsync();
        Task OpenFilePreviewAsync(ThumbnailViewModel thumbnailViewModel);
    }
}
