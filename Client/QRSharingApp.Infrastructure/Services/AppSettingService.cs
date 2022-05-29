using QRSharingApp.ClientApi.Interfaces;
using QRSharingApp.Infrastructure.Services.Interfaces;
using System.Linq;

namespace QRSharingApp.Infrastructure.Services
{
    public class AppSettingService : IAppSettingService
    {
        private readonly ISettingApi _settingApi;

        private readonly string _localizationKey = "Localization";
        private readonly string _backgroundImagePathKey = "BackgroundImagePath";
        private readonly string _sortingDisplayFilesKey = "SortingDisplayFiles";
        private readonly string _wifiLogin = "WifiLogin";
        private readonly string _wifiPassword = "WifiPassword";
        private readonly string _wifiAuthenticationType = "wifiAuthenticationType";
        private readonly string _wifiIsHidden = "wifiIsHidden";
        private readonly string _autoSwitchSeconds = "AutoSwitchSeconds";
        private readonly string _webBackgroundImagePath = "WebBackgroundImagePath";
        private readonly string _downloadViaForm = "DownloadViaForm";
        private readonly string _requiredFieldsForDownload = "RequiredFieldsForDownload";

        public int[] RequiredFieldsForDownload
        {
            get
            {
                var value = _settingApi.GetSetting(_requiredFieldsForDownload);
                if (string.IsNullOrEmpty(value))
                    return new int[0];

                return value.Split(",").Select(_ => int.Parse(_)).ToArray();
            }
            set { _settingApi.SetSetting(_requiredFieldsForDownload, string.Join(",", value)); }
        }

        public bool DownloadViaForm
        {
            get
            {
                var value = _settingApi.GetSetting(_downloadViaForm);
                if (string.IsNullOrEmpty(value))
                    return false;

                return value == "1";
            }
            set { _settingApi.SetSetting(_downloadViaForm, value ? "1" : "0"); }
        }

        public string WebBackgroundImagePath
        {
            get { return _settingApi.GetSetting(_webBackgroundImagePath); }
            set { _settingApi.SetSetting(_webBackgroundImagePath, value); }
        }

        public string CultureName
        {
            get { return _settingApi.GetSetting(_localizationKey) ?? string.Empty; }
            set { _settingApi.SetSetting(_localizationKey, value); }
        }

        public string BackgroundImagePath
        {
            get { return _settingApi.GetSetting(_backgroundImagePathKey) ?? string.Empty; }
            set { _settingApi.SetSetting(_backgroundImagePathKey, value); }
        }

        public string WifiLogin
        {
            get { return _settingApi.GetSetting(_wifiLogin) ?? string.Empty; }
            set { _settingApi.SetSetting(_wifiLogin, value); }
        }

        public string WifiPassword
        {
            get { return _settingApi.GetSetting(_wifiPassword) ?? string.Empty; }
            set { _settingApi.SetSetting(_wifiPassword, value); }
        }

        public int WifiAuthenticationType
        {
            get
            {
                var value = _settingApi.GetSetting(_wifiAuthenticationType);
                if (string.IsNullOrEmpty(value))
                    return default;

                return int.Parse(value);
            }
            set { _settingApi.SetSetting(_wifiAuthenticationType, value.ToString()); }
        }

        public bool WifiIsHidden
        {
            get
            {
                var value = _settingApi.GetSetting(_wifiIsHidden);
                if (string.IsNullOrEmpty(value))
                    return default;

                return bool.Parse(value);
            }
            set { _settingApi.SetSetting(_wifiIsHidden, value.ToString()); }
        }

        public bool SortingDisplayFiles 
        {
            get
            {
                var value = _settingApi.GetSetting(_sortingDisplayFilesKey);
                if (string.IsNullOrEmpty(value))
                    return true;

                return bool.Parse(value);
            }
            set { _settingApi.SetSetting(_sortingDisplayFilesKey, value.ToString()); }
        }

        public int AutoSwitchSeconds
        {
            get
            {
                var value = _settingApi.GetSetting(_autoSwitchSeconds);
                if (string.IsNullOrEmpty(value))
                    return 0;

                return int.Parse(value);
            }
            set { _settingApi.SetSetting(_autoSwitchSeconds, value.ToString()); }
        }

        public AppSettingService(ISettingApi settingApi)
        {
            _settingApi = settingApi;
        }
    }
}
