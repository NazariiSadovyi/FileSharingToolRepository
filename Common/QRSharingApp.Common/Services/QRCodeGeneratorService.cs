using QRSharingApp.Common.Helpers;
using QRSharingApp.Common.Services.Interfaces;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace QRSharingApp.Common.Services
{
    public class QRCodeGeneratorService : IQRCodeGeneratorService
    {
        private static readonly QRCodeGenerator _qrGenerator = new QRCodeGenerator();

        public Bitmap BitmapImage(string data)
        {
            var qrCodeData = _qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            return qrCode.GetGraphic(5);
        }

        public string Base64Image(string data)
        {
            var bitmap = BitmapImage(data);
            var bytes = bitmap.ToBytes();
            return string.Format("data:image/png;base64,{0}", Convert.ToBase64String(bytes));
        }

        public void SaveToStream(string data, Stream stream)
        {
            var bitmap = BitmapImage(data);
            bitmap.Save(stream, ImageFormat.Jpeg);
            stream.Position = 0;
        }
    }
}
