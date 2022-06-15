using QRSharingApp.ClientApi.Interfaces;
using System.Linq;

namespace QRSharingApp.Infrastructure.Settings
{
    public class WebSetting : Common.Settings.WebSetting
    {
        private readonly ISettingApi _settingApi;

        public WebSetting(ISettingApi settingApi)
        {
            _settingApi = settingApi;
        }

        public override bool ShowAgreedCheckboxOnDownload
        {
            get
            {
                var value = _settingApi.GetSetting(ShowAgreedCheckboxOnDownloadKey);
                if (string.IsNullOrEmpty(value))
                    return false;

                return value == "1";
            }
            set { _settingApi.SetSetting(ShowAgreedCheckboxOnDownloadKey, value ? "1" : "0"); }
        }

        public override string DefaultCountryOnDownload
        {
            get
            {
                var skinType = _settingApi.GetSetting(DefaultCountryOnDownloadKey);
                if (string.IsNullOrEmpty(skinType))
                {
                    return "US";
                }

                return skinType;
            }
            set { _settingApi.SetSetting(DefaultCountryOnDownloadKey, value); }
        }

        public override int[] RequiredFieldsForDownload
        {
            get
            {
                var value = _settingApi.GetSetting(RequiredFieldsForDownloadKey);
                if (string.IsNullOrEmpty(value))
                    return new int[0];

                return value.Split(",").Select(_ => int.Parse(_)).ToArray();
            }
            set { _settingApi.SetSetting(RequiredFieldsForDownloadKey, string.Join(",", value)); }
        }

        public override bool DownloadViaForm
        {
            get
            {
                var value = _settingApi.GetSetting(DownloadViaFormKey);
                if (string.IsNullOrEmpty(value))
                    return false;

                return value == "1";
            }
            set { _settingApi.SetSetting(DownloadViaFormKey, value ? "1" : "0"); }
        }

        public override string WebBackgroundImagePath
        {
            get { return _settingApi.GetSetting(WebBackgroundImagePathKey); }
            set { _settingApi.SetSetting(WebBackgroundImagePathKey, value); }
        }

        public override string WebCultureCode
        {
            get { return _settingApi.GetSetting(WebCultureCodeKey); }
            set { _settingApi.SetSetting(WebCultureCodeKey, value); }
        }

        public override bool ShowWifiQrCodeInWeb
        {
            get
            {
                var value = _settingApi.GetSetting(ShowWifiQrCodeInWebKey);
                if (string.IsNullOrEmpty(value))
                    return false;

                return value == "1";
            }
            set { _settingApi.SetSetting(ShowWifiQrCodeInWebKey, value ? "1" : "0"); }
        }

        public override bool ShowGalleryUrlQrCodeInWeb
        {
            get
            {
                var value = _settingApi.GetSetting(ShowGalleryUrlQrCodeInWebKey);
                if (string.IsNullOrEmpty(value))
                    return false;

                return value == "1";
            }
            set { _settingApi.SetSetting(ShowGalleryUrlQrCodeInWebKey, value ? "1" : "0"); }
        }
    }
}
