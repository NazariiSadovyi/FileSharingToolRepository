using QRSharingApp.ClientApi.Interfaces;
using QRSharingApp.Contract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRSharingApp.ClientApi.Implementations
{
    public class DownloadHistoryApi : IDownloadHistoryApi
    {
        private readonly IClientProvider _clientProvider;

        public DownloadHistoryApi(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public async Task<List<DownloadHistoryContract>> GetAllAsync()
        {
            return await _clientProvider.GetAsync<List<DownloadHistoryContract>>("downloadHistory/api/getAll");
        }

        public async Task ClearAsync()
        {
            await _clientProvider.DeleteAsync("downloadHistory/api/delete");
        }
    }
}
