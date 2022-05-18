using Prism.Ioc;
using QRSharingApp.ClientApi.Implementations;
using QRSharingApp.ClientApi.Interfaces;

namespace QRSharingApp.ClientApi
{
    public static class ClientApiDependencies
    {
        public static void Register(IContainerRegistry containerRegistry, string adress)
        {
            containerRegistry.RegisterSingleton<IClientProvider>(_ => new ClientProvider(adress));

            containerRegistry.RegisterSingleton<ILocalFileApi, LocalFileApi>();
            containerRegistry.RegisterSingleton<ISettingApi, SettingApi>();
            containerRegistry.RegisterSingleton<IDownloadHistoryApi, DownloadHistoryApi>();
            containerRegistry.RegisterSingleton<IHotFolderApi, HotFolderApi>();
        }
    }
}
