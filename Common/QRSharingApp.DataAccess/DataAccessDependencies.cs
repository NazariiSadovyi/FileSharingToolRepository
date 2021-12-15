using QRSharingApp.DataAccess.Repositories;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using Prism.Ioc;

namespace QRSharingApp.DataAccess
{
    public static class DataAccessDependencies
    {
        public static void Register(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDownloadHistoryRepository, DownloadHistoryRepository>();
            containerRegistry.RegisterSingleton<ILocalFileRepository, LocalFileRepository>();
            containerRegistry.RegisterSingleton<IHotFolderRepository, HotFolderRepository>();
            containerRegistry.RegisterSingleton<ISettingRepository, SettingRepository>();
            containerRegistry.RegisterSingleton<ApplicationDBContext>();
        }
    }
}
