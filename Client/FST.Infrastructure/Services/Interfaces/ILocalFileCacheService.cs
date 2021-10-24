using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FST.Infrastructure.Services.Interfaces
{
    public interface ILocalFileCacheService
    {
        void ClearTemporary();
        Task<BitmapImage> GetBitmapImage(string fileName);
        string GetFilePathIfExist(string fileName);
        Task<string> SaveFile(Func<FileStream, Task> fileStreamFunc, string fileName, bool temporary = true);
    }
}