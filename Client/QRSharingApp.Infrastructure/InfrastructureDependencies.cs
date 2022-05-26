using QRSharingApp.Common.Services;
using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.Infrastructure.Services;
using QRSharingApp.Infrastructure.Services.Interfaces;
using Unity;

namespace QRSharingApp.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static void Register(IUnityContainer containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILocalFileCacheService, LocalFileCacheService>();
            containerRegistry.RegisterSingleton<IHotFolderService, HotFolderService>();
            containerRegistry.RegisterSingleton<IWebServerService, WebServerService>();
            containerRegistry.RegisterSingleton<IFileThumbnailService, FileThumbnailService>();
            containerRegistry.RegisterSingleton<IQRCodeGeneratorService, QRCodeGeneratorService>();
            containerRegistry.RegisterSingleton<IDownloadHistoryService, DownloadHistoryService>();
            containerRegistry.RegisterType<IAppSettingService, AppSettingService>();
            containerRegistry.RegisterType<IFileExplorerService, FileExplorerService>();
            containerRegistry.RegisterType<IExcelExportService, ExcelExportService>();
            containerRegistry.RegisterType<IWifiService, WifiService>();
        }
    }
}
