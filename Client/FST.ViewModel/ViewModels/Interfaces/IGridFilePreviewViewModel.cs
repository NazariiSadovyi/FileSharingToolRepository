using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FST.ViewModel.ViewModels.Interfaces
{
    public interface IGridFilePreviewViewModel
    {
        ObservableCollection<FilePreviewViewModel> Files { get; set; }
        Task LoadDataAsync();
    }
}
