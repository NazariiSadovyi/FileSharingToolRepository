using QRSharingApp.Infrastructure.Models;
using Prism.Mvvm;
using System.IO;
using System.Windows.Media.Imaging;

namespace QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels
{
    public abstract class FilePreviewBaseViewModel : BindableBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string WebPath { get; set; }
        public bool IsLoading { get; set; }
        public string LocalPath { get; set; }
        public string FullLocalPath { get; set; }
        public bool ShowQRCode { get; set; }
        public BitmapImage QRImage { get; set; }
        public string SharedLink { get; set; }

        public FilePreviewBaseViewModel(LocalFile localFile)
        {
            Name = localFile.Name;
            LocalPath = localFile.Path;
            FullLocalPath = Path.Combine(LocalPath, Name);
        }
    }
}
