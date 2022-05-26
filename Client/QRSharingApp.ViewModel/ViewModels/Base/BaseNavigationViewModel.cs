using ReactiveUI;
using System.Threading.Tasks;

namespace QRSharingApp.ViewModel.ViewModels.Base
{
    public abstract class BaseNavigationViewModel : ReactiveObject
    {
        public abstract Task OnLoadAsync();
    }
}
