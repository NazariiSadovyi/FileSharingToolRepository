using FST.Common.Services;
using FST.Common.Services.Interfaces;
using FST.DataAccess;
using FST.Infrastructure.Services;
using FST.Infrastructure.Services.Interfaces;
using Prism.Ioc;

namespace FST.Infrastructure
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
            containerRegistry.Register<IAppSettingService, AppSettingService>();
            containerRegistry.Register<IFileExplorerService, FileExplorerService>();
            containerRegistry.Register<IWifiService, WifiService>();

            DataAccessDependencies.Register(containerRegistry);
        }
    }
}
