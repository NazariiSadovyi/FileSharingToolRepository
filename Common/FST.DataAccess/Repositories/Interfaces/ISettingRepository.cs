using System.Threading.Tasks;

namespace FST.DataAccess.Repositories.Interfaces
{
    public interface ISettingRepository
    {
        Task<int?> GetIntSettingAsync(string key);
        Task<string> GetStringSettingAsync(string key);
        Task SetSettingAsync(string key, int value);
        Task SetSettingAsync(string key, string value);
        int GetIntSetting(string key, int defaultValue = 0);
        string GetStringSetting(string key, string defaultValue = null);
        bool GetBoolSetting(string key, bool defaultBool = false);
        void SetSetting(string key, int value);
        void SetSetting(string key, string value);
        void SetSetting(string key, bool value);
    }
}
