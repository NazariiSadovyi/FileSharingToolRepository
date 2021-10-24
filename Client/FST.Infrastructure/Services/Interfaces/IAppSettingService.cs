namespace FST.Infrastructure.Services.Interfaces
{
    public interface IAppSettingService
    {
        string CultureName { get; set; }
        int ItemsInGrid { get; set; }
        string BackgroundImagePath { get; set; }
        string EmailSendBackgroundImagePath { get; set; }
        bool SortingDisplayFiles { get; set; }
    }
}
