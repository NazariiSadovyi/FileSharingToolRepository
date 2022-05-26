using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QRSharingApp.Contract.LocalFile;
using QRSharingApp.ClientApi.Interfaces;
using System.Web;

namespace QRSharingApp.ClientApi.Implementations
{
    public class LocalFileApi : ILocalFileApi
    {
        private readonly IClientProvider _clientProvider;
        
        public LocalFileApi(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public async Task<List<LocalFileContract>> GetFiles()
        {
            return await _clientProvider.GetAsync<List<LocalFileContract>>("api/localFile");
        }

        public async Task<LocalFileContract> CreateFile(string filePath)
        {
            var createLocalFile = new CreateLocalFile()
            {
                Path = filePath
            };
            return await _clientProvider.PostWithResponseAsync<CreateLocalFile, LocalFileContract>("api/localFile", createLocalFile);
        }

        public Task<List<LocalFileContract>> CreateFiles(List<string> filePathes)
        {
            throw new NotImplementedException();
        }

        public async Task<LocalFileContract> GetFile(string filePath)
        {
            return await _clientProvider.GetAsync<LocalFileContract>($"api/localFile/{HttpUtility.UrlEncodeUnicode(filePath)}");
        }
    }
}
