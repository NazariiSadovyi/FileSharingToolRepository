namespace QRSharingApp.Common.Services.Interfaces
{
    public interface ISharedSettingService
    {
        bool DownloadViaForm { get; set; }
        string WebBackgroundImagePath { get; set; }
    }
}
