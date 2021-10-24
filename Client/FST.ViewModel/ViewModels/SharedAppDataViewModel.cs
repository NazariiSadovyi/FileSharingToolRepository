using FST.ViewModel.ViewModels.Interfaces;
using Prism.Mvvm;

namespace FST.ViewModel.ViewModels
{
    public class SharedAppDataViewModel : BindableBase, ISharedAppDataViewModel
    {
        private bool _isActivated;

        public bool IsActivated
        {
            get { return _isActivated; }
            set { SetProperty(ref _isActivated, value); }
        }

        private bool _isInImportState;

        public bool IsInImportState
        {
            get { return _isInImportState; }
            set { SetProperty(ref _isInImportState, value); }
        }

        private bool _isPreviewVisible;

        public bool IsPreviewVisible
        {
            get { return _isPreviewVisible; }
            set { SetProperty(ref _isPreviewVisible, value); }
        }

        private string _uploadingMessage;

        public string UploadingMessage
        {
            get { return _uploadingMessage; }
            set { SetProperty(ref _uploadingMessage, value); }
        }
    }
}
