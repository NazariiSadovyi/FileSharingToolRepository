using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
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
            var createLocalFile = new CreateLocalFiles()
            {
                Pathes = new List<string>() { filePath }
            };
            var response = await _clientProvider.PostWithResponseAsync<CreateLocalFiles, List<LocalFileContract>>("api/localFile", createLocalFile);

            return response[0];
        }

        public async Task<List<LocalFileContract>> CreateFiles(List<string> filePathes)
        {
            var createLocalFile = new CreateLocalFiles()
            {
                Pathes = new List<string>(filePathes)
            };
            return await _clientProvider.PostWithResponseAsync<CreateLocalFiles, List<LocalFileContract>>("api/localFile", createLocalFile);
        }

        public async Task<LocalFileContract> GetFile(string filePath)
        {
            return await _clientProvider.GetAsync<LocalFileContract>($"api/localFile/{HttpUtility.UrlEncodeUnicode(filePath)}");
        }
    }
}
