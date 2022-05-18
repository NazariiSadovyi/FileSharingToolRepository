using QRSharingApp.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRSharingApp.DataAccess.Repositories.Interfaces
{
    public interface IHotFolderRepository
    {
        Task<HotFolder> Add(string folderPath);
        Task<IEnumerable<HotFolder>> GetAll();
        Task<HotFolder> GetById(int folderId);
        Task<HotFolder> GetByPath(string folderPath);
        Task Remove(int folderId);
    }
}
