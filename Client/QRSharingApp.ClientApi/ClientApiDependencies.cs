using QRSharingApp.ClientApi.Implementations;
using QRSharingApp.ClientApi.Interfaces;
using Unity;
using Unity.Injection;

namespace QRSharingApp.ClientApi
{
    public static class ClientApiDependencies
    {
        public static void Register(IUnityContainer containerRegistry, string adress)
        {
            containerRegistry.RegisterSingleton<IClientProvider, ClientProvider>(new InjectionConstructor(adress));

            containerRegistry.RegisterSingleton<ILocalFileApi, LocalFileApi>();
            containerRegistry.RegisterSingleton<ISettingApi, SettingApi>();
            containerRegistry.RegisterSingleton<IDownloadHistoryApi, DownloadHistoryApi>();
            containerRegistry.RegisterSingleton<IHotFolderApi, HotFolderApi>();
        }
    }
}
