using QRSharingApp.Contract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRSharingApp.ClientApi.Interfaces
{
    public interface IDownloadHistoryApi
    {
        Task ClearAsync();
        Task<List<DownloadHistoryContract>> GetAllAsync();
    }
}
