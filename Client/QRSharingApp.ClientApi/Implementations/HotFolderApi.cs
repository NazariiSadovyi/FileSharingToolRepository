using QRSharingApp.ClientApi.Interfaces;
using QRSharingApp.Contract;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace QRSharingApp.ClientApi.Implementations
{
    internal class HotFolderApi : IHotFolderApi
    {
        private readonly IClientProvider _clientProvider;

        public HotFolderApi(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public async Task<List<HotFolderContract>> GetAll()
        {
            return await _clientProvider.GetAsync<List<HotFolderContract>>("api/hotfolder");
        }

        public async Task<HotFolderContract> GetByPath(string folderPath)
        {
            return await _clientProvider.GetAsync<HotFolderContract>($"api/hotfolder/{HttpUtility.UrlEncodeUnicode(folderPath)}");
        }

        public async Task<HotFolderContract> Create(string folderPath)
        {
            return await _clientProvider.PostWithResponseAsync<string, HotFolderContract>("api/hotfolder", folderPath);
        }

        public async Task<HotFolderContract> Delete(int folderId)
        {
            return await _clientProvider.DeleteWithReponseAsync<HotFolderContract>($"api/hotfolder/{folderId}");
        }
    }
}
