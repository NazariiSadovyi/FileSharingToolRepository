using QRSharingApp.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRSharingApp.Infrastructure.Services.Interfaces
{
    public interface IDownloadHistoryService
    {
        Task ClearAsync();
        Task<IList<DownloadHistoryModel>> GetAll();
    }
}