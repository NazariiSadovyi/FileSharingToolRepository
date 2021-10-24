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
            containerRegistry.Register<IAppSettingService, AppSettingService>();
            containerRegistry.Register<IFileExplorerService, FileExplorerService>();

            DataAccessDependencies.Register(containerRegistry);
        }
    }
}
