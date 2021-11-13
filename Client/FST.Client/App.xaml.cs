using FST.Client.Views;
using FST.Infrastructure;
using FST.Infrastructure.Services.Interfaces;
using FST.ViewModel.Interfaces;
using FST.ViewModel.Services;
using FST.ViewModel.ViewModels;
using FST.ViewModel.ViewModels.Interfaces;
using NLog;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Localization = FST.CultureLocalization.Localization;

namespace FST.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            _logger.Info("Application started");
            SetupExceptionHandling();

            base.OnStartup(e);

            Task.Run(async () =>
            {
                await CheckActivation();
                await InitCurrentFiles();
                await InitPreviewFiles();
            });

            var localFileCacheService = Container.Resolve<ILocalFileCacheService>();
            localFileCacheService.ClearTemporary();
            _logger.Info("Temp folder was cleaned up");
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName.Replace(".Client.Views.", ".ViewModel.ViewModels.");
                var viewAssemblyName = "FST.ViewModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                var viewModelName = $"{viewName}Model, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }

        protected override Window CreateShell()
        {
            var appSettingService = Container.Resolve<IAppSettingService>();
            Localization.Language = new System.Globalization.CultureInfo(appSettingService.CultureName ?? string.Empty);

            return Container.Resolve<MainWindowView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IApplicationTaskUtility, ApplicationTaskUtility>();
            containerRegistry.RegisterSingleton<ISharedAppDataViewModel, SharedAppDataViewModel>();
            containerRegistry.RegisterSingleton<ILocalFilesService, LocalFilesService>();

            RegisterForNavigation(containerRegistry);
            RegisterViewModels(containerRegistry);

            InfrastructureDependencies.Register(containerRegistry);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<MainModule>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _logger.Info("Application shouted down");

            var localFileCacheService = Container.Resolve<ILocalFileCacheService>();
            localFileCacheService.ClearTemporary();
            _logger.Info("Temp folder was cleaned up");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            _logger.Info("All resources were released");

            base.OnExit(e);
        }
        private void RegisterForNavigation(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DesignView>();
            containerRegistry.RegisterForNavigation<HotFoldersView>();
            containerRegistry.RegisterForNavigation<ActivationView>();
            containerRegistry.RegisterForNavigation<GridFilePreviewView>();
            containerRegistry.RegisterForNavigation<WifiConfigurationView>();
        }

        private void RegisterViewModels(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<GridFilePreviewViewModel>();
            containerRegistry.RegisterSingleton<IGridFilePreviewViewModel, GridFilePreviewViewModel>();
            containerRegistry.RegisterSingleton<WifiConfigurationViewModel>();
        }

        private async Task InitCurrentFiles()
        {
            var applicationUtility = Container.Resolve<IApplicationTaskUtility>();
            var localFilesService = Container.Resolve<ILocalFilesService>();
            await applicationUtility.ExecuteFetchDataAsync(() =>
                {
                    return localFilesService.InitCurrentFiles();
                },
                Localization.GetResource("AddingCurrentHotFoldersAndFilesFetchMessage"),
                false
            );
        }

        private async Task InitPreviewFiles()
        {
            var gridFilePreviewViewModel = Container.Resolve<IGridFilePreviewViewModel>();
            await gridFilePreviewViewModel.LoadDataAsync();

            var wifiConfigurationViewModel = Container.Resolve<WifiConfigurationViewModel>();
            wifiConfigurationViewModel.UpdateQRCodeCmd.Execute(null);
        }

        private async Task CheckActivation()
        {
            //var applicationUtility = Container.Resolve<IApplicationTaskUtility>();
            //var sharedAppData = Container.Resolve<ISharedAppDataViewModel>();
            //var activationService = Container.Resolve<IActivationService>();

            //bool? isActivated = false;
            //await applicationUtility.ExecuteFetchDataAsync(
                //() =>
                //{
                    //return Task.Run(async () =>
                    //{
                        //isActivated = await activationService.IsActivated();
                    //});
                //},
                //Localization.GetResource("CheckingActivationFetchMessage")
            //);

            //if (!isActivated.Value)
            //{
                //applicationUtility.ShowInformationMessage(Localization.GetResource("ToolIsNotActivatedWarningMessage"), InformationKind.Warning);
            //}
            //sharedAppData.IsActivated = isActivated.Value;
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                _logger.Error(exception, message);
            }
        }
    }
}