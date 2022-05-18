using QRSharingApp.ClientApi.Interfaces;
using QRSharingApp.Contract;
using System.Threading.Tasks;

namespace QRSharingApp.ClientApi.Implementations
{
    public class SettingApi : ISettingApi
    {
        private readonly IClientProvider _clientProvider;

        public SettingApi(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public async Task<string> GetSettingAsync(string key)
        {
            return await _clientProvider.GetAsync<string>($"api/setting/{key}");
        }

        public async Task SetSettingAsync(string key, string value)
        {
            var contract = new UpdateSettingContract()
            {
                Key = key,
                Value = value
            };
            await _clientProvider.PostAsync("api/setting", contract);
        }

        public string GetSetting(string key)
        {
            return _clientProvider.Get<string>($"api/setting/{key}");
        }

        public void SetSetting(string key, string value)
        {
            var contract = new UpdateSettingContract()
            {
                Key = key,
                Value = value
            };
            _clientProvider.Post("api/setting", contract);
        }
    }
}
