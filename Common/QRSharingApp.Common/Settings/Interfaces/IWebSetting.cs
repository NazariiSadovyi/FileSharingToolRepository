namespace QRSharingApp.Common.Settings.Interfaces
{
    public interface IWebSetting
    {
        bool DownloadViaForm { get; set; }
        string WebBackgroundImagePath { get; set; }
        int[] RequiredFieldsForDownload { get; set; }
        string DefaultCountryOnDownload { get; set; }
        bool ShowAgreedCheckboxOnDownload { get; set; }
        bool ShowWifiQrCodeInWeb { get; set; }
        bool ShowGalleryUrlQrCodeInWeb { get; set; }
    }
}
