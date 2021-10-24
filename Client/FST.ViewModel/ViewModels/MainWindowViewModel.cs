using FST.ViewModel.Interfaces;
using FST.ViewModel.Models;
using FST.ViewModel.Services;
using FST.ViewModel.ViewModels.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace FST.ViewModel.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        #region Private fields
        private readonly string _uploadingMessageTemplate = CultureLocalization.Localization.GetResource("MainViewLabelProgress"); 
        
        private ICommand _shutdownApplicationCmd;
        private ICommand _switchUserControl;
        private ICommand _startPreviewCmd;
        private ICommand _closePreviewCmd;

        private WindowStyle _windowStyle = WindowStyle.SingleBorderWindow;
        private WindowState _windowState = WindowState.Normal;
        private ResizeMode _resizeMode = ResizeMode.CanResize;
        private string _fetchDataMessage;
        private bool _isTaskControlShown;
        private string _informationMessage;
        private bool _showInformationMessage;
        private InformationKind _informationKind;
        private string _spaceUsageMessage;
        private int _spaceUsagePercentage;

        private ISharedAppDataViewModel _sharedAppDataViewModel;
        #endregion

        #region Dependencies
        [Dependency]
        public IRegionManager RegionManager { get; set; }
        [Dependency]
        public ILocalFilesService LocalFilesService { get; set; }
        #endregion

        #region Properties
        public string FetchDataMessage
        {
            get { return _fetchDataMessage; }
            set { SetProperty(ref _fetchDataMessage, value); }
        }
        public string SpaceUsageMessage
        {
            get { return _spaceUsageMessage; }
            set { SetProperty(ref _spaceUsageMessage, value); }
        }
        public int SpaceUsagePercentage
        {
            get { return _spaceUsagePercentage; }
            set { SetProperty(ref _spaceUsagePercentage, value); }
        }
        public WindowStyle WindowStyle
        {
            get { return _windowStyle; }
            set { SetProperty(ref _windowStyle, value); }
        }
        public ResizeMode ResizeMode
        {
            get { return _resizeMode; }
            set { SetProperty(ref _resizeMode, value); }
        }
        public WindowState WindowState
        {
            get { return _windowState; }
            set { SetProperty(ref _windowState, value); }
        }
        public bool IsTaskControlShown
        {
            get { return _isTaskControlShown; }
            set { SetProperty(ref _isTaskControlShown, value); }
        }

        public ISharedAppDataViewModel SharedAppDataViewModel
        {
            get { return _sharedAppDataViewModel; }
            set { SetProperty(ref _sharedAppDataViewModel, value); }
        }

        public string InformationMessage
        {
            get { return _informationMessage; }
            set { SetProperty(ref _informationMessage, value); }
        }

        public InformationKind InformationKind
        {
            get { return _informationKind; }
            set { SetProperty(ref _informationKind, value); }
        }

        public bool ShowInformationMessage
        {
            get { return _showInformationMessage; }
            set { SetProperty(ref _showInformationMessage, value); }
        }
        #endregion

        #region Commands
        public ICommand SwitchUserControl
        {
            get
            {
                return _switchUserControl ??
                  (_switchUserControl = new DelegateCommand<string>(
                      obj => {
                          var navigationParams = new NavigationParameters
                          {
                              { nameof(IMainWindowViewModel), this as IMainWindowViewModel }
                          };
                          NavigateToPage(obj, navigationParams);
                      }
                  ));
            }
        }

        public ICommand StartPreviewCmd
        {
            get
            {
                return _startPreviewCmd ??
                  (_startPreviewCmd = new DelegateCommand(
                      () => {
                          RegionManager.RequestNavigate("PreviewContentRegion", "GridFilePreviewView");
                          SharedAppDataViewModel.IsPreviewVisible = true;
                      }
                  ));
            }
        }

        public ICommand ClosePreviewCmd
        {
            get
            {
                return _closePreviewCmd ??
                  (_closePreviewCmd = new DelegateCommand(
                      () => {
                          if (Keyboard.IsKeyDown(Key.Escape))
                          {
                              var isFileFullPreviewView = RegionManager.Regions["PreviewContentRegion"]
                                        .ActiveViews
                                        .Select(_ => _ as UserControl)
                                        .FirstOrDefault(_ => !(_.DataContext is GridFilePreviewViewModel));
                              if (isFileFullPreviewView == null)
                              {
                                  SharedAppDataViewModel.IsPreviewVisible = false;
                              }
                          }
                      }
                  ));
            }
        }

        public ICommand ShutdownApplicationCmd
        {
            get
            {
                return _shutdownApplicationCmd ??
                  (_shutdownApplicationCmd = new DelegateCommand(
                      () => {
                          Application.Current.Shutdown();
                      }
                  ));
            }
        }

        public ICommand CloseInformationMessageCmd
        {
            get => new DelegateCommand(
                () => {
                    ShowInformationMessage = false;
                    InformationMessage = string.Empty;
                });
        }
        #endregion

        public MainWindowViewModel(IApplicationTaskUtility applicationTaskUtility,
            ISharedAppDataViewModel sharedAppDataViewModel,
            ILocalFilesService localFilesService)
        {
            IsTaskControlShown = true;
            FetchDataMessage = "Aplication is starting...";

            sharedAppDataViewModel.UploadingMessage = string.Format(_uploadingMessageTemplate, 0, 0);
            SharedAppDataViewModel = sharedAppDataViewModel;
            applicationTaskUtility.FetchDataCommand.RegisterCommand(
                new DelegateCommand<FetchDataInfo>((obj) =>
                {
                    IsTaskControlShown = obj.ShowControl;
                    FetchDataMessage = obj.Message;
                })
                {
                    IsActive = true
                }
            );
            applicationTaskUtility.InformationMessageCommand.RegisterCommand(
                new DelegateCommand<InformationMessageInfo>((obj) =>
                {
                    InformationMessage = obj.Message;
                    InformationKind = obj.Kind;
                    ShowInformationMessage = true;
                })
                {
                    IsActive = true
                }
            );
        }

        private void NavigateToPage(string userControlName, NavigationParameters valuePairs = default)
        {
            RegionManager.RequestNavigate("MainContentRegion", userControlName, valuePairs);
        }
    }
}