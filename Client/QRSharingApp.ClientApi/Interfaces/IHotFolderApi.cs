using QRSharingApp.Contract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRSharingApp.ClientApi.Interfaces
{
    public interface IHotFolderApi
    {
        Task<HotFolderContract> Create(string folderPath);
        Task<HotFolderContract> Delete(int folderId);
        Task<List<HotFolderContract>> GetAll();
        Task<HotFolderContract> GetByPath(string folderPath);
    }
}
