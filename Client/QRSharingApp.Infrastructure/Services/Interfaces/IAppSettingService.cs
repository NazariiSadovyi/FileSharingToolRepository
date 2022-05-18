namespace QRSharingApp.Infrastructure.Services.Interfaces
{
    public interface IAppSettingService
    {
        string CultureName { get; set; }
        string BackgroundImagePath { get; set; }
        bool SortingDisplayFiles { get; set; }
        string WifiLogin { get; set; }
        string WifiPassword { get; set; }
        bool WifiIsHidden { get; set; }
        int WifiAuthenticationType { get; set; }
        int AutoSwitchSeconds { get; set; }
        bool DownloadViaForm { get; set; }
        string WebBackgroundImagePath { get; set; }
    }
}
