using QRSharingApp.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRSharingApp.DataAccess.Repositories.Interfaces
{
    public interface IDownloadHistoryRepository
    {
        Task Add(DownloadHistory downloadHistory);
        Task<IEnumerable<DownloadHistory>> GetAll();
        Task ClearAsync();
    }
}
