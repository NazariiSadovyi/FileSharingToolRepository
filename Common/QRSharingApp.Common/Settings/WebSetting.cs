using QRSharingApp.Common.Settings.Interfaces;

namespace QRSharingApp.Common.Settings
{
    public abstract class WebSetting : IWebSetting
    {
        protected readonly string WebBackgroundImagePathKey = "WebBackgroundImagePath";
        protected readonly string DownloadViaFormKey = "DownloadViaForm";
        protected readonly string RequiredFieldsForDownloadKey = "RequiredFieldsForDownload";
        protected readonly string ShowWifiQrCodeInWebKey = "ShowWifiQrCodeInWeb";
        protected readonly string DefaultCountryOnDownloadKey = "DefaultCountryOnDownload";
        protected readonly string ShowAgreedCheckboxOnDownloadKey = "ShowAgreedCheckboxOnDownload";
        protected readonly string ShowGalleryUrlQrCodeInWebKey = "ShowGalleryUrlQrCodeInWeb";

        public abstract string DefaultCountryOnDownload { get; set; }
        public abstract bool ShowWifiQrCodeInWeb { get; set; }
        public abstract bool ShowGalleryUrlQrCodeInWeb { get; set; }
        public abstract bool ShowAgreedCheckboxOnDownload { get; set; }
        public abstract int[] RequiredFieldsForDownload { get; set; }
        public abstract bool DownloadViaForm { get; set; }
        public abstract string WebBackgroundImagePath { get; set; }
    }
}
