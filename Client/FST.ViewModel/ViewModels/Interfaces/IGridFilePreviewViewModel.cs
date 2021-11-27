using FST.ViewModel.ViewModels.FilePreviewVIewModels;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FST.ViewModel.ViewModels.Interfaces
{
    public interface IGridFilePreviewViewModel
    {
        ObservableCollection<FilePreviewBaseViewModel> Files { get; set; }
        Task LoadDataAsync();
        void OnNavigatedFrom(NavigationContext navigationContext);
        void StartAutoSwitchTimer();
        void StopAutoSwitchTimer();
    }
}
