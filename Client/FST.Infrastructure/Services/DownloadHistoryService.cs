using FST.DataAccess.Repositories.Interfaces;
using FST.Infrastructure.Models;
using FST.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FST.Infrastructure.Services
{
    public class DownloadHistoryService : IDownloadHistoryService
    {
        private readonly IDownloadHistoryRepository _downloadHistoryRepository;

        public DownloadHistoryService(IDownloadHistoryRepository downloadHistoryRepository)
        {
            _downloadHistoryRepository = downloadHistoryRepository;
        }

        public async Task<IList<DownloadHistoryModel>> GetAll()
        {
            var entities = await _downloadHistoryRepository.GetAll();
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
            await _downloadHistoryRepository.ClearAsync();
        }
    }
}
