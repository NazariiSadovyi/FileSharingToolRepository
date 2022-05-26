using QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace QRSharingApp.ViewModel.ViewModels.Interfaces
{
    public interface IGridFilePreviewViewModel
    {
        ObservableCollection<FilePreviewBaseViewModel> Files { get; set; }
        Task LoadDataAsync();
        void StartAutoSwitchTimer();
        void StopAutoSwitchTimer();
    }
}
