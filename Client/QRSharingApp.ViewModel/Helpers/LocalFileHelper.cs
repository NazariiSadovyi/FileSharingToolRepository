using QRSharingApp.Infrastructure.Models;
using QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace QRSharingApp.ViewModel.Helpers
{
    public static class LocalFileHelper
    {
        private static readonly IList<string> ImageExtensions = new List<string> { ".jpg", ".jpe", ".bmp", ".jpeg", ".png" };
        private static readonly IList<string> VideoExtensions = new List<string> { ".mpg", ".mp2", ".mpeg", ".mpe,", ".mpv", ".ogg", ".mp4", ".m4p", ".m4v", ".avi", ".wmv", ".mov", ".flv", ".swf", ".avchd" };

        public static bool IsPhoto(string fileName)
        {
            return ImageExtensions.Contains(Path.GetExtension(fileName).ToLower());
        }

        public static bool IsVideo(string fileName)
        {
            return VideoExtensions.Contains(Path.GetExtension(fileName).ToLower());
        }

        public static FilePreviewBaseViewModel ToFilePreviewViewModel(this LocalFile localFile)
        {
            if (localFile.IsPhoto)
            {
                return new PhotoFilePreviewViewModel(localFile);
            }

            if (localFile.IsVideo)
            {
                return new VideoFilePreviewViewModel(localFile);
            }

            throw new Exception("File is not in photo or video format");
        }
    }
}