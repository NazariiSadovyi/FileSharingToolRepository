using FST.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FST.DataAccess.Repositories.Interfaces
{
    public interface IDownloadHistoryRepository
    {
        Task Add(DownloadHistory downloadHistory);
        Task<IEnumerable<DownloadHistory>> GetAll();
        Task ClearAsync();
    }
}
