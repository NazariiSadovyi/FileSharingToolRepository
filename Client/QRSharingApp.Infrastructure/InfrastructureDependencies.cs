using QRSharingApp.Common.Services;
using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.Infrastructure.Services;
using QRSharingApp.Infrastructure.Services.Interfaces;
using Prism.Ioc;

namespace QRSharingApp.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static void Register(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILocalFileCacheService, LocalFileCacheService>();
            containerRegistry.RegisterSingleton<IHotFolderService, HotFolderService>();
            containerRegistry.RegisterSingleton<IWebServerService, WebServerService>();
            containerRegistry.RegisterSingleton<IFileThumbnailService, FileThumbnailService>();
            containerRegistry.RegisterSingleton<IQRCodeGeneratorService, QRCodeGeneratorService>();
            containerRegistry.RegisterSingleton<IDownloadHistoryService, DownloadHistoryService>();
            containerRegistry.Register<IAppSettingService, AppSettingService>();
            containerRegistry.Register<IFileExplorerService, FileExplorerService>();
            containerRegistry.Register<IExcelExportService, ExcelExportService>();
            containerRegistry.Register<IWifiService, WifiService>();
        }
    }
}
