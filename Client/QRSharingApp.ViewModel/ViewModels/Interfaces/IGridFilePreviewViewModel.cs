using QRSharingApp.ViewModel.ViewModels.Base;
using System.Threading.Tasks;

namespace QRSharingApp.ViewModel.ViewModels.Interfaces
{
    public interface IGridFilePreviewViewModel
    {
        PageRequestViewModel PageRequestViewModel { get; set; }

        Task OnLoadAsync();
        void StartAutoSwitchTimer();
        void StopAutoSwitchTimer();
        void UpdateSorting(bool byDescending);
    }
}
