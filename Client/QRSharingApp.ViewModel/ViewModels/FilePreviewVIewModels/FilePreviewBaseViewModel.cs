using QRSharingApp.Infrastructure.Models;
using System.IO;
using System.Windows.Media.Imaging;
using QRSharingApp.ViewModel.ViewModels.Base;
using System;

namespace QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels
{
    public abstract class FilePreviewBaseViewModel : ViewModelBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string WebPath { get; set; }
        public bool IsLoading { get; set; }
        public string LocalPath { get; set; }
        public string FullLocalPath { get; set; }
        public bool ShowQRCode { get; set; }
        public BitmapImage QRImage { get; set; }
        public BitmapSource Thumbnail { get; set; }
        public string SharedLink { get; set; }
        public DateTime CreationDate { get; set; }

        public FilePreviewBaseViewModel(LocalFile localFile)
        {
            Name = localFile.Name;
            LocalPath = localFile.Path;
            FullLocalPath = Path.Combine(LocalPath, Name);
            CreationDate = localFile.CreationDate;
        }
    }
}
