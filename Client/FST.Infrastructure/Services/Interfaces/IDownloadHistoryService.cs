using FST.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FST.Infrastructure.Services.Interfaces
{
    public interface IDownloadHistoryService
    {
        Task ClearAsync();
        Task<IList<DownloadHistoryModel>> GetAll();
    }
}