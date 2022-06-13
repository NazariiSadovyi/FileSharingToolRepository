using QRSharingApp.DataAccess.Repositories.Interfaces;

namespace QRSharingApp.WebApplication.Settings
{
    public class WifiSetting : Common.Settings.WifiSetting
    {
        private readonly ISettingRepository _settingRepository;

        public WifiSetting(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public override string WifiLogin
        {
            get => _settingRepository.GetStringSetting(WifiLoginKey);
            set => _settingRepository.SetSetting(WifiLoginKey, value);
        }

        public override string WifiPassword
        {
            get => _settingRepository.GetStringSetting(WifiPasswordKey);
            set => _settingRepository.SetSetting(WifiPasswordKey, value);
        }

        public override int WifiAuthenticationType
        {
            get => _settingRepository.GetIntSetting(WifiAuthenticationTypeKey);
            set => _settingRepository.SetSetting(WifiAuthenticationTypeKey, value);
        }

        public override bool WifiIsHidden
        {
            get => _settingRepository.GetBoolSetting(WifiIsHiddenKey);
            set => _settingRepository.SetSetting(WifiIsHiddenKey, value);
        }
    }
}
