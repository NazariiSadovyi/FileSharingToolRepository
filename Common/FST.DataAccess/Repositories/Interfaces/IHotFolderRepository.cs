using FST.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FST.DataAccess.Repositories.Interfaces
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
