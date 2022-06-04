using FFmpeg.AutoGen;
using HandyControl.Data;
using HandyControl.Tools;
using NLog;
using QRSharingApp.Client.Views;
using QRSharingApp.ClientApi;
using QRSharingApp.Infrastructure;
using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.Shared;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.Services;
using QRSharingApp.ViewModel.ViewModels;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Unosquare.FFME;
using Localization = QRSharingApp.CultureLocalization.Localization;

namespace QRSharingApp.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly static IUnityContainer Container = new UnityContainer();

        protected override void OnStartup(StartupEventArgs e)
        {
            _logger.Info("Application initialization started");
            try
            {
                SetupExceptionHandling();
                RegisterTypes();
                InitFFMPEG();

                var appSettingService = Container.Resolve<IAppSettingService>();
                UpdateSkin(appSettingService.SkinType == "white" ? SkinType.Default : SkinType.Dark);
                
                CreateShell();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Application initialization exception");
            }

            _logger.Info("Application initialization ended");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _logger.Info("Application shouted down");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            _logger.Info("All resources were released");
        }

        public void UpdateSkin(SkinType skin)
        {
            var skins0 = Resources.MergedDictionaries[0];
            skins0.MergedDictionaries.Clear();
            skins0.MergedDictionaries.Add(ResourceHelper.GetSkin(skin));

            var skins1 = Resources.MergedDictionaries[1];
            skins1.MergedDictionaries.Clear();
            skins1.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });

            Current.MainWindow?.OnApplyTemplate();
        }

        private void CreateShell()
        {
            var appSettingService = Container.Resolve<IAppSettingService>();
            Localization.Language = new System.Globalization.CultureInfo(appSettingService.CultureName ?? string.Empty);

            var window = Container.Resolve<MainWindowView>();
            var mainWindowViewModel = Container.Resolve<IMainWindowViewModel>();
            window.DataContext = mainWindowViewModel;
            window.Show();
            Task.Run(async () => await mainWindowViewModel.OnLoadAsync());
        }

        private void RegisterTypes()
        {
            Container.RegisterSingleton<IApplicationTaskUtility, ApplicationTaskUtility>();
            Container.RegisterSingleton<ISharedAppDataViewModel, SharedAppDataViewModel>();
            Container.RegisterSingleton<IActivationService, ActivationService>();
            Container.RegisterSingleton<ILocalFilesService, LocalFilesService>();
            Container.RegisterSingleton<IDataExportService, DataExportService>();

            RegisterViewModels();

            InfrastructureDependencies.Register(Container);
            ClientApiDependencies.Register(Container, SharedConstants.LocalhostPath);
        }

        private void RegisterViewModels()
        {
            Container.RegisterSingleton<MainWindowViewModel>();
            Container.RegisterSingleton<IMainWindowViewModel, MainWindowViewModel>();
            Container.RegisterSingleton<GridFilePreviewViewModel>();
            Container.RegisterSingleton<IGridFilePreviewViewModel, GridFilePreviewViewModel>();
            Container.RegisterSingleton<WifiConfigurationViewModel>();
        }

        private void InitFFMPEG()
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Library.FFmpegDirectory = Path.Combine(assemblyFolder, "FFMPEGBinaries");
            Library.EnableWpfMultiThreadedVideo = false;
            Library.FFmpegLoadModeFlags = FFmpegLoadMode.FullFeatures;
            Library.LoadFFmpeg();
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