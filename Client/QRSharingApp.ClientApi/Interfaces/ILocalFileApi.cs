using QRSharingApp.Contract.LocalFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
