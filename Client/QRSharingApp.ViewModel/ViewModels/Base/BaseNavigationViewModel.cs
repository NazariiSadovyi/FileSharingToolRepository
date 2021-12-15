using Prism.Mvvm;
using Prism.Regions;

namespace QRSharingApp.ViewModel.ViewModels.Base
{
    public class BaseNavigationViewModel : BindableBase, INavigationAware
    {
        public virtual bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public virtual void OnNavigatedFrom(NavigationContext navigationContext) { }

        public virtual void OnNavigatedTo(NavigationContext navigationContext) { }
    }
}
