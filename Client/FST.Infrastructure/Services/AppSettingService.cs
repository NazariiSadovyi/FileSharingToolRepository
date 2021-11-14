using FST.DataAccess.Repositories.Interfaces;
using FST.Infrastructure.Services.Interfaces;

namespace FST.Infrastructure.Services
{
    public class AppSettingService : IAppSettingService
    {
        private readonly ISettingRepository _settingRepository;

        private readonly string _localizationKey = "Localization";
        private readonly string _backgroundImagePathKey = "BackgroundImagePath";
        private readonly string _sortingDisplayFilesKey = "SortingDisplayFiles";
        private readonly string _wifiLogin = "WifiLogin";
        private readonly string _wifiPassword = "WifiPassword";
        private readonly string _wifiAuthenticationType = "wifiAuthenticationType";
        private readonly string _wifiIsHidden = "wifiIsHidden";
        private readonly string _autoSwitchSeconds = "AutoSwitchSeconds";

        public string CultureName
        {
            get { return _settingRepository.GetStringSetting(_localizationKey) ?? string.Empty; }
            set { _settingRepository.SetSetting(_localizationKey, value); }
        }

        public string BackgroundImagePath
        {
            get { return _settingRepository.GetStringSetting(_backgroundImagePathKey) ?? string.Empty; }
            set { _settingRepository.SetSetting(_backgroundImagePathKey, value); }
        }

        public string WifiLogin
        {
            get { return _settingRepository.GetStringSetting(_wifiLogin) ?? string.Empty; }
            set { _settingRepository.SetSetting(_wifiLogin, value); }
        }

        public string WifiPassword
        {
            get { return _settingRepository.GetStringSetting(_wifiPassword) ?? string.Empty; }
            set { _settingRepository.SetSetting(_wifiPassword, value); }
        }

        public int WifiAuthenticationType
        {
            get { return _settingRepository.GetIntSetting(_wifiAuthenticationType); }
            set { _settingRepository.SetSetting(_wifiAuthenticationType, value); }
        }

        public bool WifiIsHidden
        {
            get { return _settingRepository.GetBoolSetting(_wifiIsHidden, false); }
            set { _settingRepository.SetSetting(_wifiIsHidden, value); }
        }

        public bool SortingDisplayFiles 
        {
            get { return _settingRepository.GetBoolSetting(_sortingDisplayFilesKey, true); }
            set { _settingRepository.SetSetting(_sortingDisplayFilesKey, value); }
        }

        public int AutoSwitchSeconds
        {
            get { return _settingRepository.GetIntSetting(_autoSwitchSeconds, 15); }
            set { _settingRepository.SetSetting(_autoSwitchSeconds, value); }
        }

        public AppSettingService(ISettingRepository settingRepository)
        {
           _settingRepository = settingRepository;
        }
    }
}
