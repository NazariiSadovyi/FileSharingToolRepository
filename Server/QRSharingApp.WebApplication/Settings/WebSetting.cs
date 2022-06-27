using QRSharingApp.DataAccess.Repositories.Interfaces;
using System.Linq;

namespace QRSharingApp.WebApplication.Settings
{
    public class WebSetting : Common.Settings.WebSetting
    {
        private readonly ISettingRepository _settingRepository;

        public WebSetting(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public override string WebCultureCode
        {
            get => _settingRepository.GetStringSetting(WebCultureCodeKey) ?? null;
            set => _settingRepository.SetSetting(WebCultureCodeKey, value);
        }

        public override string NetworkId
        {
            get => _settingRepository.GetStringSetting(NetworkIdKey) ?? null;
            set => _settingRepository.SetSetting(NetworkIdKey, value);
        }

        public override string DefaultCountryOnDownload
        {
            get => _settingRepository.GetStringSetting(DefaultCountryOnDownloadKey) ?? "US";
            set => _settingRepository.SetSetting(DefaultCountryOnDownloadKey, value);
        }

        public override bool ShowWifiQrCodeInWeb
        {
            get => _settingRepository.GetBoolSetting(ShowWifiQrCodeInWebKey, false);
            set => _settingRepository.SetSetting(ShowWifiQrCodeInWebKey, value);
        }

        public override bool ShowGalleryUrlQrCodeInWeb
        {
            get => _settingRepository.GetBoolSetting(ShowGalleryUrlQrCodeInWebKey, false);
            set => _settingRepository.SetSetting(ShowGalleryUrlQrCodeInWebKey, value);
        }

        public override bool ShowAgreedCheckboxOnDownload
        {
            get => _settingRepository.GetBoolSetting(ShowAgreedCheckboxOnDownloadKey, false);
            set => _settingRepository.SetSetting(ShowAgreedCheckboxOnDownloadKey, value);
        }

        public override int[] RequiredFieldsForDownload
        {
            get
            {
                var _value = _settingRepository.GetStringSetting(RequiredFieldsForDownloadKey);
                if (string.IsNullOrEmpty(_value))
                {
                    return new int[0];
                }

                var trimmedValue = _value.Replace(" ", "");
                return trimmedValue.Split(",").Select(int.Parse).ToArray();
            }
            set => _settingRepository.SetSetting(RequiredFieldsForDownloadKey, string.Join(",", value));
        }

        public override bool DownloadViaForm
        {
            get => _settingRepository.GetBoolSetting(DownloadViaFormKey, false);
            set => _settingRepository.SetSetting(DownloadViaFormKey, value);
        }

        public override string WebBackgroundImagePath
        {
            get => _settingRepository.GetStringSetting(WebBackgroundImagePathKey);
            set => _settingRepository.SetSetting(WebBackgroundImagePathKey, value);
        }
    }
}