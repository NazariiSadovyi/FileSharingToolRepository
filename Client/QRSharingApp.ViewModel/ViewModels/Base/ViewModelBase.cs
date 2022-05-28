using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
