using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.ViewModels.Base;
using Prism.Commands;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class DesignViewModel : BaseNavigationViewModel
    {
        #region Private fields
        private WindowState _previousWindowState;
        #endregion

        #region Dependencies
        [Dependency]
        public IAppSettingService AppSettingService;
        [Dependency]
        public IFileExplorerService FileExplorerService;
        [Dependency]
        public IMainWindowViewModel MainWindowViewModel;
        #endregion

        #region Properties
        public string BackgroundImagePath { get; set; }

        public string WebBackgroundImagePath { get; set; }

        public bool SortingDisplayFiles { get; set; }

        public bool DownloadViaForm { get; set; }

        public int AutoSwitchSeconds { get; set; }
        #endregion

        #region Commands
        public ICommand ChangeLanguageCmd
        {
            get => new DelegateCommand<string>(
            cultureName => {
                CultureLocalization.Localization.Language = new System.Globalization.CultureInfo(cultureName);
                AppSettingService.CultureName = cultureName;
            });
        }

        public ICommand SelectGridPreviewBackgroundImageCmd
        {
            get => new DelegateCommand(
            () => {
                var filePath = FileExplorerService.SelectFile("Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png");
                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }

                BackgroundImagePath = filePath;
            });
        }

        public ICommand SelectWebBackgroundImageCmd
        {
            get => new DelegateCommand(
            () => {
                var filePath = FileExplorerService.SelectFile("Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png");
                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }

                WebBackgroundImagePath = filePath;
            });
        }

        public ICommand ChangeWindowModeCmd
        {
            get => new DelegateCommand<string>(
            mode => {
                switch (mode)
                {
                    case "full":
                        _previousWindowState = MainWindowViewModel.WindowState;
                        MainWindowViewModel.WindowState = WindowState.Maximized;
                        MainWindowViewModel.WindowStyle = WindowStyle.None;
                        MainWindowViewModel.ResizeMode = ResizeMode.NoResize;
                        break;
                    case "normal":
                        MainWindowViewModel.WindowState = _previousWindowState;
                        MainWindowViewModel.WindowStyle = WindowStyle.SingleBorderWindow;
                        MainWindowViewModel.ResizeMode = ResizeMode.CanResize;
                        break;
                    default:
                        break;
                }

                Application.Current.MainWindow.Hide();
                Application.Current.MainWindow.Show();
                Application.Current.MainWindow.BringIntoView();
            });
        }
        #endregion

        public DesignViewModel(IAppSettingService appSettingService)
        {
            WebBackgroundImagePath = appSettingService.WebBackgroundImagePath;
            BackgroundImagePath = appSettingService.BackgroundImagePath;
            SortingDisplayFiles = appSettingService.SortingDisplayFiles;
            DownloadViaForm = appSettingService.DownloadViaForm;
            AutoSwitchSeconds = appSettingService.AutoSwitchSeconds;

            PropertyChanged += (e, args) => 
            {
                switch (args.PropertyName)
                {
                    case nameof(SortingDisplayFiles):
                        AppSettingService.SortingDisplayFiles = SortingDisplayFiles;
                        break;
                    case nameof(BackgroundImagePath):
                        AppSettingService.BackgroundImagePath = BackgroundImagePath;
                        break;
                    case nameof(AutoSwitchSeconds):
                        AppSettingService.AutoSwitchSeconds = AutoSwitchSeconds;
                        break;
                    case nameof(DownloadViaForm):
                        AppSettingService.DownloadViaForm = DownloadViaForm;
                        break;
                    case nameof(WebBackgroundImagePath):
                        AppSettingService.WebBackgroundImagePath = WebBackgroundImagePath;
                        break;
                    default:
                        break;
                }
            };
        }
    }
}
