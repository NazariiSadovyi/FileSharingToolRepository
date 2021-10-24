using FST.Infrastructure.Services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FST.Infrastructure.Services
{
    public class LocalFileCacheService : ILocalFileCacheService
    {
        private readonly string _eternalDirectoryPath;
        private readonly string _temporaryDirectoryPath;

        public LocalFileCacheService()
        {
            var tempDirectoryName = "Event QR Sender";
            var tempDirectoryPath = Path.GetTempPath();
            _eternalDirectoryPath = Path.Combine(tempDirectoryPath, tempDirectoryName, "Eternal");
            _temporaryDirectoryPath = Path.Combine(tempDirectoryPath, tempDirectoryName, "Temporary");
            Directory.CreateDirectory(_eternalDirectoryPath);
            Directory.CreateDirectory(_temporaryDirectoryPath);
        }

        public async Task<string> SaveFile(Func<FileStream, Task> fileStreamFunc, string fileName, bool temporary = true)
        {
            string filePath;
            if (temporary)
            {
                filePath = Path.Combine(_temporaryDirectoryPath, fileName);
            }
            else
            {
                filePath = Path.Combine(_eternalDirectoryPath, fileName);
            }

            try
            {
                using (var fstream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    await fileStreamFunc(fstream);
                }
            }
            catch (IOException e)
            {
                // todo: logger
            }

            return filePath;
        }

        public async Task<BitmapImage> GetBitmapImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            var filePath = GetFilePathIfExist(fileName);
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            BitmapImage image = null;

            await Task.Run(() =>
            {
                if (new FileInfo(filePath).Length != 0)
                {
                    image = new BitmapImage(new Uri(filePath))
                    {
                        CacheOption = BitmapCacheOption.None
                    };
                    image.Freeze();
                }
            });

            return image;
        }

        public void ClearTemporary()
        {
            if (!Directory.Exists(_temporaryDirectoryPath))
            {
                return;
            }

            foreach (var filePath in Directory.GetFiles(_temporaryDirectoryPath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch { }
            }
        }

        public string GetFilePathIfExist(string fileName)
        {
            var temporary = Path.Combine(_temporaryDirectoryPath, fileName);
            var eternal = Path.Combine(_eternalDirectoryPath, fileName);

            if (File.Exists(temporary))
            {
                return temporary;
            }
            else if (File.Exists(eternal))
            {
                return eternal;
            }

            return string.Empty;
        }
    }
}