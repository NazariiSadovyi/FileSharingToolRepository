using QRSharingApp.ClientApi.Interfaces;

namespace QRSharingApp.Infrastructure.Settings
{
    partial class WifiSetting : Common.Settings.WifiSetting
    {
        private readonly ISettingApi _settingApi;

        public WifiSetting(ISettingApi settingApi)
        {
            _settingApi = settingApi;
        }

        public override string WifiLogin
        {
            get { return _settingApi.GetSetting(WifiLoginKey) ?? string.Empty; }
            set { _settingApi.SetSetting(WifiLoginKey, value); }
        }

        public override string WifiPassword
        {
            get { return _settingApi.GetSetting(WifiPasswordKey) ?? string.Empty; }
            set { _settingApi.SetSetting(WifiPasswordKey, value); }
        }

        public override int WifiAuthenticationType
        {
            get
            {
                var value = _settingApi.GetSetting(WifiAuthenticationTypeKey);
                if (string.IsNullOrEmpty(value))
                    return default;

                return int.Parse(value);
            }
            set { _settingApi.SetSetting(WifiAuthenticationTypeKey, value.ToString()); }
        }

        public override bool WifiIsHidden
        {
            get
            {
                var value = _settingApi.GetSetting(WifiIsHiddenKey);
                if (string.IsNullOrEmpty(value))
                    return false;

                return value == "1";
            }
            set { _settingApi.SetSetting(WifiIsHiddenKey, value ? "1" : "0"); }
        }
    }
}
