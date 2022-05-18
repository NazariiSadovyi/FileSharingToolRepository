using QRSharingApp.ClientApi.Interfaces;
using QRSharingApp.Infrastructure.Models;
using QRSharingApp.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QRSharingApp.Infrastructure.Services
{
    public class DownloadHistoryService : IDownloadHistoryService
    {
        private readonly IDownloadHistoryApi _downloadHistoryApi;

        public DownloadHistoryService(IDownloadHistoryApi downloadHistoryApi)
        {
            _downloadHistoryApi = downloadHistoryApi;
        }

        public async Task<IList<DownloadHistoryModel>> GetAll()
        {
            var entities = await _downloadHistoryApi.GetAllAsync();
            return entities.Select(_ => new DownloadHistoryModel()
            {
                Id = _.Id,
                FileId = _.FileId,
                FileName = _.FileName,
                FilePath = _.FilePath,
                UserEmail = _.UserEmail,
                UserName = _.UserName,
                UserPhone = _.UserPhone,
                Date = DateTime.Parse(_.Date),
            }).ToList();
        }

        public async Task ClearAsync()
        {
            await _downloadHistoryApi.ClearAsync();
        }
    }
}
