using System.Threading.Tasks;

namespace QRSharingApp.ClientApi.Interfaces
{
    public interface ISettingApi
    {
        string GetSetting(string key);
        Task<string> GetSettingAsync(string key);
        void SetSetting(string key, string value);
        Task SetSettingAsync(string key, string value);
    }
}
