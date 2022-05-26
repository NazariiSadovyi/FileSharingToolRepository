using System.Collections.Generic;
using System.Threading.Tasks;
using QRSharingApp.Contract.LocalFile;

namespace QRSharingApp.ClientApi.Interfaces
{
    public interface ILocalFileApi
    {
        Task<LocalFileContract> GetFile(string filePath);
        Task<LocalFileContract> CreateFile(string filePath);
        Task<List<LocalFileContract>> CreateFiles(List<string> filePathes);
        Task<List<LocalFileContract>> GetFiles();
    }
}
