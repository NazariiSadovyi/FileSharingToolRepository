namespace QRSharingApp.Infrastructure.Settings.Interfaces
{
    public interface IAppSetting
    {
        string CultureName { get; set; }
        string BackgroundImagePath { get; set; }
        bool SortingDisplayFiles { get; set; }
        int AutoSwitchSeconds { get; set; }
        int ItemsInGrid { get; set; }
        string SkinType { get; set; }
    }
}
