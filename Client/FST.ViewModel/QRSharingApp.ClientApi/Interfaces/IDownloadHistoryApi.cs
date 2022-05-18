using QRSharingApp.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRSharingApp.ClientApi.Interfaces
{
    public interface IDownloadHistoryApi
    {
        Task ClearAsync();
        Task<List<DownloadHistoryContract>> GetAllAsync();
    }
}
