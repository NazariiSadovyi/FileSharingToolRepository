using System;

namespace FST.ViewModel.ViewModels.Interfaces
{
    public interface ISharedAppDataViewModel
    {
        bool IsActivated { get; set; }
        bool IsInImportState { get; set; }
        bool IsPreviewVisible { get; set; }
        string UploadingMessage { get; set; }
    }
}