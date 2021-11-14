using FST.ViewModel.Interfaces;
using FST.Infrastructure.Services.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace FST.ViewModel.ViewModels
{
    public class DesignViewModel : BindableBase, INavigationAware
    {
        #region Private fields
        private IMainWindowViewModel _mainWindowViewModel;
        private WindowState _previousWindowState;
        private string _backgroundImagePath;
        private bool _sortingDisplayFiles;
        private int _autoSwitchSeconds;
        #endregion

        #region Dependencies
        [Dependency]
        public IAppSettingService AppSettingService;
        [Dependency]
        public IFileExplorerService FileExplorerService;
        #endregion

        #region Properties
        public string BackgroundImagePath
        {
            get { return _backgroundImagePath; }
            set { SetProperty(ref _backgroundImagePath, value); }
        }

        public bool SortingDisplayFiles
        {
            get { return _sortingDisplayFiles; }
            set { SetProperty(ref _sortingDisplayFiles, value); }
        }

        public int AutoSwitchSeconds
        {
            get { return _autoSwitchSeconds; }
            set { SetProperty(ref _autoSwitchSeconds, value); }
        }
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

        public ICommand ChangeWindowModeCmd
        {
            get => new DelegateCommand<string>(
            mode => {
                switch (mode)
                {
                    case "full":
                        _previousWindowState = _mainWindowViewModel.WindowState;
                        _mainWindowViewModel.WindowState = WindowState.Maximized;
                        _mainWindowViewModel.WindowStyle = WindowStyle.None;
                        _mainWindowViewModel.ResizeMode = ResizeMode.NoResize;
                        break;
                    case "normal":
                        _mainWindowViewModel.WindowState = _previousWindowState;
                        _mainWindowViewModel.WindowStyle = WindowStyle.SingleBorderWindow;
                        _mainWindowViewModel.ResizeMode = ResizeMode.CanResize;
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
            BackgroundImagePath = appSettingService.BackgroundImagePath;
            SortingDisplayFiles = appSettingService.SortingDisplayFiles;
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
                    default:
                        break;
                }
            };
        }

        #region Navigation
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _mainWindowViewModel = navigationContext.Parameters
                .GetValue<IMainWindowViewModel>(nameof(IMainWindowViewModel));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion
    }
}
