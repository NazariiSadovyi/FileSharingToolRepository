using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.DataAccess.Repositories.Interfaces;

namespace QRSharingApp.Common.Services
{
    public class SharedSettingService : ISharedSettingService
    {
        private readonly ISettingRepository _settingRepository;

        private readonly string _webBackgroundImagePath = "WebBackgroundImagePath";
        private readonly string _downloadViaForm = "DownloadViaForm";

        public bool DownloadViaForm
        {
            get { return _settingRepository.GetBoolSetting(_downloadViaForm, false); }
            set { _settingRepository.SetSetting(_downloadViaForm, value); }
        }

        public string WebBackgroundImagePath
        {
            get { return _settingRepository.GetStringSetting(_webBackgroundImagePath); }
            set { _settingRepository.SetSetting(_webBackgroundImagePath, value); }
        }

        public SharedSettingService(ISettingRepository settingRepository)
        {
           _settingRepository = settingRepository;
        }
    }
}
