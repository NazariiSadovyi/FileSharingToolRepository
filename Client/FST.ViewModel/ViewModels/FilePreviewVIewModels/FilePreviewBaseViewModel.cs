using FST.Infrastructure.Models;
using Prism.Mvvm;
using System.IO;
using System.Windows.Media.Imaging;

namespace FST.ViewModel.ViewModels.FilePreviewVIewModels
{
    public abstract class FilePreviewBaseViewModel : BindableBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string WebPath { get; set; }

        private bool _isLoaded;
        public bool IsLoading
        {
            get { return _isLoaded; }
            set { SetProperty(ref _isLoaded, value); }
        }

        private string _localPath;
        public string LocalPath
        {
            get { return _localPath; }
            set { SetProperty(ref _localPath, value); }
        }

        private string _fullLocalPath;
        public string FullLocalPath
        {
            get { return _fullLocalPath; }
            set { SetProperty(ref _fullLocalPath, value); }
        }

        private bool _ShowQRCode;
        public bool ShowQRCode
        {
            get { return _ShowQRCode; }
            set { SetProperty(ref _ShowQRCode, value); }
        }

        private BitmapImage _qrImage;
        public BitmapImage QRImage
        {
            get { return _qrImage; }
            set { SetProperty(ref _qrImage, value); }
        }

        private string _sharedLink;
        public string SharedLink
        {
            get { return _sharedLink; }
            set { SetProperty(ref _sharedLink, value); }
        }

        public FilePreviewBaseViewModel(LocalFile localFile)
        {
            Name = localFile.Name;
            LocalPath = localFile.Path;
            FullLocalPath = Path.Combine(LocalPath, Name);
        }
    }
}
