using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.Models;
using QRSharingApp.ViewModel.Services;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        #region Dependencies
        [Dependency]
        public IRegionManager RegionManager { get; set; }
        [Dependency]
        public IWebServerService WebServerService { get; set; }
        [Dependency]
        public ILocalFilesService LocalFilesService { get; set; }
        [Dependency]
        public IGridFilePreviewViewModel GridFilePreviewViewModel { get; set; }
        #endregion

        #region Properties
        public string FetchDataMessage { get; set; }
        public string SpaceUsageMessage { get; set; }
        public int SpaceUsagePercentage { get; set; }
        public WindowStyle WindowStyle { get; set; } = WindowStyle.SingleBorderWindow;
        public ResizeMode ResizeMode { get; set; } = ResizeMode.CanResize;
        public WindowState WindowState { get; set; } = WindowState.Normal;
        public bool IsTaskControlShown { get; set; }

        public ISharedAppDataViewModel SharedAppDataViewModel { get; set; }

        public string InformationMessage { get; set; }
        public InformationKind InformationKind { get; set; }
        public bool ShowInformationMessage { get; set; }
        #endregion

        #region Commands
        public ICommand SwitchUserControl
        {
            get => new DelegateCommand<string>(userControlName =>
            {
                NavigateToPage(userControlName);
            });
        }

        public ICommand StartPreviewCmd
        {
            get => new DelegateCommand(() =>
            {
                RegionManager.RequestNavigate("PreviewContentRegion", "GridFilePreviewView");
                SharedAppDataViewModel.IsPreviewVisible = true;
                GridFilePreviewViewModel.StartAutoSwitchTimer();
            });
        }

        public ICommand ClosePreviewCmd
        {
            get => new DelegateCommand(() =>
            {
                if (Keyboard.IsKeyDown(Key.Escape))
                {
                    var isFileFullPreviewView = RegionManager.Regions["PreviewContentRegion"]
                              .ActiveViews
                              .Select(_ => _ as UserControl)
                              .FirstOrDefault(_ => !(_.DataContext is GridFilePreviewViewModel));
                    if (isFileFullPreviewView == null)
                    {
                        SharedAppDataViewModel.IsPreviewVisible = false;
                        GridFilePreviewViewModel.StopAutoSwitchTimer();
                    }
                }
            });
        }

        public ICommand ShutdownApplicationCmd
        {
            get => new DelegateCommand(() =>
            {
                Application.Current.Shutdown();
            });
        }

        public ICommand CloseInformationMessageCmd
        {
            get => new DelegateCommand(() =>
            {
                ShowInformationMessage = false;
                InformationMessage = string.Empty;
            });
        }

        public ICommand OpenWebPreviewCmd
        {
            get => new DelegateCommand(() =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = WebServerService.WebLocalhostUrl,
                    UseShellExecute = true
                });
            });
        }
        #endregion

        public MainWindowViewModel(IApplicationTaskUtility applicationTaskUtility,
            ISharedAppDataViewModel sharedAppDataViewModel)
        {
            IsTaskControlShown = true;
            FetchDataMessage = CultureLocalization.Localization.GetResource("AplicationIsStartingProcess");

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