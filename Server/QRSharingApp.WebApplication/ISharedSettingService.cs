namespace QRSharingApp.WebApplication
{
    public interface ISharedSettingService
    {
        bool DownloadViaForm { get; set; }
        string WebBackgroundImagePath { get; set; }
    }
}
