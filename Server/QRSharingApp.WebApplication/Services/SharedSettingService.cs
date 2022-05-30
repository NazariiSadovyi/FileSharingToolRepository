using QRSharingApp.DataAccess.Repositories.Interfaces;
using System.Linq;

namespace QRSharingApp.WebApplication.Services
{
    public interface ISharedSettingService
    {
        bool DownloadViaForm { get; set; }
        string WebBackgroundImagePath { get; set; }
        int[] RequiredFieldsForDownload { get; set; }
    }

    public class SharedSettingService : ISharedSettingService
    {
        private readonly ISettingRepository _settingRepository;

        private readonly string _webBackgroundImagePath = "WebBackgroundImagePath";
        private readonly string _downloadViaForm = "DownloadViaForm";
        private readonly string _requiredFieldsForDownload = "RequiredFieldsForDownload";
        private readonly string _showWifiQrCodeInWeb = "ShowWifiQrCodeInWeb";

        public bool ShowWifiQrCodeInWeb
        {
            get
            {
                var value = _settingRepository.GetStringSetting(_showWifiQrCodeInWeb);
                if (string.IsNullOrEmpty(value))
                    return default;

                return bool.Parse(value);
            }
            set { _settingRepository.SetSetting(_showWifiQrCodeInWeb, value.ToString()); }
        }

        public int[] RequiredFieldsForDownload
        {
            get
            {
                var value = _settingRepository.GetStringSetting(_requiredFieldsForDownload);
                if (string.IsNullOrEmpty(value))
                    return new int[0];

                return value.Split(",").Select(_ => int.Parse(_)).ToArray();
            }
            set { _settingRepository.SetSetting(_requiredFieldsForDownload, string.Join(",", value)); }
        }

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
