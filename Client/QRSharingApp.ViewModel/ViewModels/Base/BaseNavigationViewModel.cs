using ReactiveUI;
using System.Threading.Tasks;

namespace QRSharingApp.ViewModel.ViewModels.Base
{
    public abstract class BaseNavigationViewModel : ViewModelBase
    {
        public abstract Task OnLoadAsync();
    }
}
