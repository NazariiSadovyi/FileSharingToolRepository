using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.Models;
using QRSharingApp.ViewModel.Services;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity;
using Localization = QRSharingApp.CultureLocalization.Localization;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        private IDictionary<PageType, BaseNavigationViewModel> _pageDictionary;

        #region Dependencies
        [Dependency]
        public IWebServerService WebServerService { get; set; }
        [Dependency]
        public ILocalFilesService LocalFilesService { get; set; }
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }
        [Dependency]
        public IApplicationTaskUtility ApplicationTaskUtility { get; set; }
        [Dependency]
        public IActivationService ActivationService { get; set; }
        [Dependency]
        public GridFilePreviewViewModel GridFilePreviewViewModel { get; set; }
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

        public object CurrentPreviewViewModel { get; set; }
        public BaseNavigationViewModel CurrentPage { get; set; }
        #endregion

        #region Commands
        public ICommand SwitchUserControl => ReactiveCommand.Create<string>(
            intPageType =>
            {
                var pageType = (PageType)int.Parse(intPageType);
                CurrentPage = _pageDictionary[pageType];
            }
        );

        public ICommand OpenGridPreviewCmd => ReactiveCommand.Create(() =>
            {
                SharedAppDataViewModel.IsPreviewVisible = true;
                CurrentPreviewViewModel = GridFilePreviewViewModel;
                GridFilePreviewViewModel.StartAutoSwitchTimer();
            }
        );

        public ICommand OpenFilePreviewCmd => ReactiveCommand.CreateFromTask<ThumbnailViewModel>(
            async thumbnailViewModel =>
            {
                await OpenFilePreviewAsync(thumbnailViewModel);
            }
        );

        public ICommand CloseFilePreviewCmd => ReactiveCommand.Create(
            () =>
            {
                CurrentPreviewViewModel = GridFilePreviewViewModel;
                SharedAppDataViewModel.IsFilePreviewOpened = false;
            }
        );

        public ICommand KeyDownCmd => ReactiveCommand.Create(
            () =>
            {
                if (Keyboard.IsKeyDown(Key.Escape))
                {
                    switch (CurrentPreviewViewModel)
                    {
                        case GridFilePreviewViewModel _ when SharedAppDataViewModel.IsPreviewVisible:
                            CurrentPreviewViewModel = null;
                            SharedAppDataViewModel.IsPreviewVisible = false;
                            GridFilePreviewViewModel.StopAutoSwitchTimer();
                            break;
                        case FilePreviewBaseViewModel _:
                            CurrentPreviewViewModel = GridFilePreviewViewModel;
                            break;
                        default:
                            break;
                    }
                }
            }
        );

        public ICommand ShutdownApplicationCmd => ReactiveCommand.Create(() =>
            {
                Application.Current.Shutdown();
            }
        );

        public ICommand CloseInformationMessageCmd => ReactiveCommand.Create(() =>
            {
                ShowInformationMessage = false;
                InformationMessage = string.Empty;
            }
        );

        public ICommand OpenWebPreviewCmd => ReactiveCommand.Create(() =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = WebServerService.WebLocalhostUrl,
                    UseShellExecute = true
                });
            }
        );
        #endregion

        public MainWindowViewModel(IApplicationTaskUtility applicationTaskUtility,
            ISharedAppDataViewModel sharedAppDataViewModel)
        {
            _pageDictionary = new Dictionary<PageType, BaseNavigationViewModel>();
            IsTaskControlShown = true;
            FetchDataMessage = Localization.GetResource("AplicationIsStartingProcess");

            SharedAppDataViewModel = sharedAppDataViewModel;
            applicationTaskUtility.FetchDataSubject
                .Subscribe((fetchDataInfo) =>
            {
                IsTaskControlShown = fetchDataInfo.ShowControl;
                FetchDataMessage = fetchDataInfo.Message;
            });
            applicationTaskUtility.InformationMessageSubject
                .Subscribe((infoMessage) =>
            {
                InformationMessage = infoMessage.Message;
                InformationKind = infoMessage.Kind;
                ShowInformationMessage = true;
            });
        }

        public async Task OnLoadAsync()
        {
            await LoadPagesAsync();
            await CheckActivationAsync();
            await InitCurrentFilesAsync();
        }

        public async Task OpenFilePreviewAsync(ThumbnailViewModel thumbnailViewModel)
        {
            var filePreviewBaseViewModel = thumbnailViewModel.ToPreviewViewModel();
            UnityContainer.BuildUp(filePreviewBaseViewModel);
            CurrentPreviewViewModel = filePreviewBaseViewModel;
            SharedAppDataViewModel.IsFilePreviewOpened = true;
            await filePreviewBaseViewModel.OnLoadAsync();
        }

        private Task LoadPagesAsync()
        {
            return ApplicationTaskUtility.ExecuteFetchDataAsync(
                () =>
                {
                    return Task.Run(async () =>
                    {
                        _pageDictionary = new Dictionary<PageType, BaseNavigationViewModel>()
                        {
                            { PageType.HotFolder, UnityContainer.Resolve<HotFoldersViewModel>() },
                            { PageType.WifiSetting, UnityContainer.Resolve<WifiConfigurationViewModel>() },
                            { PageType.Design, UnityContainer.Resolve<DesignViewModel>() },
                            { PageType.DownloadHistory, UnityContainer.Resolve<DownloadHistoryViewModel>() },
                            { PageType.Activation, UnityContainer.Resolve<ActivationViewModel>() }
                        };

                        await Task.WhenAll(_pageDictionary.Values.Select(_ => _.OnLoadAsync()).ToArray());
                        CurrentPage = _pageDictionary[PageType.HotFolder];
                    });
                },
                Localization.GetResource("AplicationIsStartingProcess"),
                false,
                false);
        }

        private Task InitCurrentFilesAsync()
        {
            return ApplicationTaskUtility.ExecuteFetchDataAsync(() =>
                {
                    return Task.Run(async () =>
                    {
                        await LocalFilesService.InitCurrentFiles();
                        await GridFilePreviewViewModel.OnLoadAsync();
                    });
                },
                Localization.GetResource("AddingCurrentHotFoldersAndFilesFetchMessage"),
                false
            );
        }

        private Task CheckActivationAsync()
        {
            return Task.Run(async () =>
            {
                var activationStatus = await ApplicationTaskUtility.ExecuteFetchDataAsync(
                    () => ActivationService.IsActivatedAsync(),
                    Localization.GetResource("CheckingActivationFetchMessage"),
                    false,
                    false);

                SharedAppDataViewModel.ActivationStatus = activationStatus;
                switch (activationStatus)
                {
                    case ActivationStatus.NotActivated:
                        ApplicationTaskUtility.ShowInformationMessage(Localization.GetResource("ToolIsNotActivatedWarningMessage"), InformationKind.Warning);
                        RunTaskToCloseToolAfter5minutes();
                        break;
                    case ActivationStatus.Expired:
                        ApplicationTaskUtility.ShowInformationMessage(Localization.GetResource("ToolKeyIsExpiredWarningMessage"), InformationKind.Warning);
                        RunTaskToCloseToolAfter5minutes();
                        break;
                    default:
                        break;
                }
            });
        }

        private void RunTaskToCloseToolAfter5minutes()
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(5));
                if (SharedAppDataViewModel.ActivationStatus != ActivationStatus.Activated)
                {
                    Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
                }
            });
        }
    }
}