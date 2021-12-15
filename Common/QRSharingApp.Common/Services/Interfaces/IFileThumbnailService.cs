using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace QRSharingApp.Common.Services.Interfaces
{
    public interface IFileThumbnailService
    {
        Task<Bitmap> GetBitmapAsync(string filePath);
        void SaveToStream(string filePath, Stream stream);
        Task SaveToStreamAsync(string filePath, Stream stream);
    }
}