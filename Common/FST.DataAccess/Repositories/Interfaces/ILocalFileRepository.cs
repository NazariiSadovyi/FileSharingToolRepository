using FST.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FST.DataAccess.Repositories.Interfaces
{
    public interface ILocalFileRepository
    {
        Task<LocalFile> Add(string localFilePath);
        Task<IEnumerable<LocalFile>> GetAll();
        Task<LocalFile> GetByFullPath(string fileFullPath);
        Task<LocalFile> GetById(string fileId);
        Task Remove(string fileId);
    }
}