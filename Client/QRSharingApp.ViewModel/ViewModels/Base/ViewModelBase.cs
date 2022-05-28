using ReactiveUI;

namespace QRSharingApp.ViewModel.ViewModels.Base
{
    public class ViewModelBase : ReactiveObject
    {
        public void RaisePropertyChanged(string propertyName)
        {
            IReactiveObjectExtensions.RaisePropertyChanged(this, propertyName);
        }
    }
}
