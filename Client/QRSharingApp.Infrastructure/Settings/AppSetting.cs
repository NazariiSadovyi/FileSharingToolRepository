using QRSharingApp.ClientApi.Interfaces;
using QRSharingApp.Infrastructure.Settings.Interfaces;

namespace QRSharingApp.Infrastructure.Settings
{
    public class AppSetting : IAppSetting
    {
        private readonly ISettingApi _settingApi;

        private readonly string _localizationKey = "Localization";
        private readonly string _backgroundImagePathKey = "BackgroundImagePath";
        private readonly string _sortingDisplayFilesKey = "SortingDisplayFiles";
        private readonly string _autoSwitchSeconds = "AutoSwitchSeconds";
        private readonly string _itemsInGridKey = "ItemsInGrid";
        private readonly string _skinType = "SkinType";

        public AppSetting(ISettingApi settingApi)
        {
            _settingApi = settingApi;
        }

        public string SkinType
        {
            get
            {
                var skinType = _settingApi.GetSetting(_skinType);
                if (string.IsNullOrEmpty(skinType))
                {
                    return "white";
                }

                return skinType;
            }
            set { _settingApi.SetSetting(_skinType, value); }
        }

        public int ItemsInGrid
        {
            get
            {
                var itemsInGrid = _settingApi.GetSetting(_itemsInGridKey);
                if (string.IsNullOrEmpty(itemsInGrid))
                {
                    return 9;
                }
                return int.Parse(itemsInGrid);
            }
            set { _settingApi.SetSetting(_itemsInGridKey, value.ToString()); }
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
    }
}
