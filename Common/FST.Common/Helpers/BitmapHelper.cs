using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FST.Common.Helpers
{
    public static class BitmapHelper
    {
        public static byte[] ToBytes(this Bitmap image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }
    }
}
