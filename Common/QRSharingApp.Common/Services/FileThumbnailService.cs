using QRSharingApp.Common.Services.Interfaces;
using Microsoft.WindowsAPICodePack.Shell;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace QRSharingApp.Common.Services
{
    public class FileThumbnailService : IFileThumbnailService
    {
        public async Task<Bitmap> GetBitmapAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            Bitmap bitmap = null;
            await Task.Run(() =>
            {
                var shellFile = ShellFile.FromFilePath(filePath);
                bitmap = shellFile.Thumbnail.ExtraLargeBitmap;
            });

            return bitmap;
        }

        public async Task SaveToStreamAsync(string filePath, Stream stream)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            Bitmap bitmap = null;
            await Task.Run(() =>
            {
                var shellFile = ShellFile.FromFilePath(filePath);
                bitmap = shellFile.Thumbnail.ExtraLargeBitmap;
            });

            if (bitmap != null)
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
            }
        }

        public void SaveToStream(string filePath, Stream stream)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            var shellFile = ShellFile.FromFilePath(filePath);
            var bitmap = shellFile.Thumbnail.ExtraLargeBitmap;

            if (bitmap != null)
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
            }
        }
    }
}
