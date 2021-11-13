using System.Drawing;
using System.IO;

namespace FST.Common.Services.Interfaces
{
    public interface IQRCodeGeneratorService
    {
        string Base64Image(string data);
        Bitmap BitmapImage(string data);
        void SaveToStream(string data, Stream stream);
    }
}
