using QRSharingApp.Infrastructure.Models;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class ThumbnailViewModel : ViewModelBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsLoading { get; set; }
        public string LocalPath { get; set; }
        public string FullLocalPath { get; set; }
        public BitmapImage QRImage { get; set; }
        public BitmapSource Thumbnail { get; set; }
        public string SharedLink { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsVideo { get; set; }

        public FilePreviewBaseViewModel ToPreviewViewModel()
        {
            if (IsVideo)
            {
                return new VideoFilePreviewViewModel(this);
            }

            return new PhotoFilePreviewViewModel(this);
        }

        public static ThumbnailViewModel Compose(LocalFile localFile)
        {
            return new ThumbnailViewModel
            {
                Name = localFile.Name,
                LocalPath = localFile.Path,
                FullLocalPath = Path.Combine(localFile.Path, localFile.Name),
                CreationDate = localFile.CreationDate,
                IsVideo = localFile.IsVideo
            };
        }
    }
}
