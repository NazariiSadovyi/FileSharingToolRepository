using FST.DataAccess.Repositories.Interfaces;
using FST.Infrastructure.Services.Interfaces;

namespace FST.Infrastructure.Services
{
    public class AppSettingService : IAppSettingService
    {
        private readonly ISettingRepository _settingRepository;

        private readonly string _localizationKey = "Localization";
        private readonly string _itemsInGridKey = "ItemsInGrid";
        private readonly string _backgroundImagePathKey = "BackgroundImagePath";
        private readonly string _sortingDisplayFilesKey = "SortingDisplayFiles";
        private readonly string _emailSendBackgroundImagePath = "EmailSendBackgroundImagePath";

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

        public string EmailSendBackgroundImagePath
        {
            get { return _settingRepository.GetStringSetting(_emailSendBackgroundImagePath) ?? string.Empty; }
            set { _settingRepository.SetSetting(_emailSendBackgroundImagePath, value); }
        }

        public int ItemsInGrid
        {
            get {
                var itemsInGrid = _settingRepository.GetIntSetting(_itemsInGridKey);
                return itemsInGrid == 0 ? 9 : itemsInGrid;
            }
            set { _settingRepository.SetSetting(_itemsInGridKey, value); }
        }

        public bool SortingDisplayFiles 
        {
            get { return _settingRepository.GetBoolSetting(_sortingDisplayFilesKey, true); }
            set { _settingRepository.SetSetting(_sortingDisplayFilesKey, value); }
        }

        public AppSettingService(ISettingRepository settingRepository)
        {
           _settingRepository = settingRepository;
        }
    }
}
