using FST.Infrastructure.Models;
using Prism.Mvvm;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace FST.ViewModel.ViewModels
{
    public class FilePreviewViewModel : BindableBase
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

        private bool _isPhoto;
        public bool IsPhoto
        {
            get { return _isPhoto; }
            set { SetProperty(ref _isPhoto, value); }
        }

        private bool _isVideo;
        public bool IsVideo
        {
            get { return _isVideo; }
            set { SetProperty(ref _isVideo, value); }
        }

        private BitmapSource _image;
        public BitmapSource Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
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

        public FilePreviewViewModel()
        {
            IsLoading = true;
        }

        public FilePreviewViewModel(LocalFile localFile) : this()
        {
            Name = localFile.Name;
            LocalPath = localFile.Path;
            IsVideo = localFile.IsVideo;
            IsPhoto = localFile.IsPhoto;
            FullLocalPath = Path.Combine(LocalPath, Name);
        }
    }
}
