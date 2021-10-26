using FST.Client.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FST.Client
{
    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("MainContentRegion", typeof(HotFoldersView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HotFoldersView>();
        }
    }
}
