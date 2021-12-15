using System;
using System.IO;
using System.Linq;

namespace QRSharingApp.WebApplication.Helpers
{
    public static class FileNameHelper
    {
        private static readonly string[] ImageExtensions = new[] { ".jpg", ".jpe", ".bmp", ".jpeg", ".png" };
        private static readonly string[] VideoExtensions = new[] { ".mpg", ".mp2", ".mpeg", ".mpe,", ".mpv", ".ogg", ".mp4", ".m4p", ".m4v", ".avi", ".wmv", ".mov", ".flv", ".swf", ".avchd" };

        public static bool IsPhoto(string fileName)
        {
            return ImageExtensions.Contains(Path.GetExtension(fileName).ToLower());
        }

        public static bool IsVideo(string fileName)
        {
            return VideoExtensions.Contains(Path.GetExtension(fileName).ToLower());
        }
    }
}
