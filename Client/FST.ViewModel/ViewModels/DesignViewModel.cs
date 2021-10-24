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
        private ICommand _selectGridPreviewBackgroundImageCmd;
        private ICommand _selectEmailSendBackgroundImageCmd;
        private ICommand _updatePreviewGridCmd;
        private ICommand _changeWindowModeCmd;
        private ICommand _changeLanguageCmd;

        private WindowState _previousWindowState;
        private string _emailSendBackgroundImagePath;
        private string _backgroundImagePath;
        private bool _sortingDisplayFiles;
        private int _itemsInPreviewGrid;

        private IMainWindowViewModel _mainWindowViewModel;
        #endregion

        #region Dependencies
        [Dependency]
        public IAppSettingService AppSettingService;
        [Dependency]
        public IFileExplorerService FileExplorerService;
        #endregion

        #region Properties
        public int ItemsInPreviewGrid
        {
            get { return _itemsInPreviewGrid; }
            set { SetProperty(ref _itemsInPreviewGrid, value); }
        }

        public string BackgroundImagePath
        {
            get { return _backgroundImagePath; }
            set { SetProperty(ref _backgroundImagePath, value); }
        }

        public string EmailSendBackgroundImagePath
        {
            get { return _emailSendBackgroundImagePath; }
            set { SetProperty(ref _emailSendBackgroundImagePath, value); }
        }

        public bool SortingDisplayFiles
        {
            get { return _sortingDisplayFiles; }
            set { SetProperty(ref _sortingDisplayFiles, value); }
        }
        #endregion

        #region Commands
        public ICommand ChangeLanguageCmd
        {
            get
            {
                return _changeLanguageCmd ??
                  (_changeLanguageCmd = new DelegateCommand<string>(
                      cultureName => {
                          CultureLocalization.Localization.Language = new System.Globalization.CultureInfo(cultureName);
                          AppSettingService.CultureName = cultureName;
                      }
                  ));
            }
        }

        public ICommand SelectGridPreviewBackgroundImageCmd
        {
            get
            {
                return _selectGridPreviewBackgroundImageCmd ??
                  (_selectGridPreviewBackgroundImageCmd = new DelegateCommand(
                      () => {
                          var filePath = FileExplorerService.SelectFile("Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png");
                          if (string.IsNullOrEmpty(filePath))
                          {
                              return;
                          }

                          BackgroundImagePath = filePath;
                      }
                  ));
            }
        }

        public ICommand SelectEmailSendBackgroundImageCmd
        {
            get
            {
                return _selectEmailSendBackgroundImageCmd ??
                  (_selectEmailSendBackgroundImageCmd = new DelegateCommand(
                      () => {
                          var filePath = FileExplorerService.SelectFile("Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png");
                          if (string.IsNullOrEmpty(filePath))
                          {
                              return;
                          }

                          EmailSendBackgroundImagePath = filePath;
                      }
                  ));
            }
        }

        public ICommand UpdatePreviewGridCmd
        {
            get
            {
                return _updatePreviewGridCmd ??
                  (_updatePreviewGridCmd = new DelegateCommand<string>(
                      _itemsInGrid => {
                          var itemsInGrid = int.Parse(_itemsInGrid);
                          if (itemsInGrid != ItemsInPreviewGrid)
                          {
                              ItemsInPreviewGrid = itemsInGrid;
                              AppSettingService.ItemsInGrid = itemsInGrid;
                          }
                      }
                  ));
            }
        }

        public ICommand ChangeWindowModeCmd
        {
            get
            {
                return _changeWindowModeCmd ??
                  (_changeWindowModeCmd = new DelegateCommand<string>(
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
                      }
                  ));
            }
        }
        #endregion

        public DesignViewModel(IAppSettingService appSettingService)
        {
            ItemsInPreviewGrid = appSettingService.ItemsInGrid;
            BackgroundImagePath = appSettingService.BackgroundImagePath;
            SortingDisplayFiles = appSettingService.SortingDisplayFiles;
            EmailSendBackgroundImagePath = appSettingService.EmailSendBackgroundImagePath;

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
                    case nameof(EmailSendBackgroundImagePath):
                        AppSettingService.EmailSendBackgroundImagePath = EmailSendBackgroundImagePath;
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
