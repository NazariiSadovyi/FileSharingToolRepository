using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.Models;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity;
using Localization = QRSharingApp.CultureLocalization.Localization;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class ActivationViewModel : BaseNavigationViewModel
    {
        #region Private fields
        private ISharedAppDataViewModel _sharedAppData;
        private string _newActivationKey;
        private string _currentActivationKey;
        #endregion

        #region Dependency
        [Dependency]
        public IActivationService ActivationService;
        [Dependency]
        public IApplicationTaskUtility ApplicationUtility;
        #endregion

        #region Commands
        public ICommand ActivateCmd
        {
            get => new DelegateCommand(
                async () =>
                {
                    await ApplicationUtility.ExecuteFetchDataAsync(async () =>
                    {
                        var result = await ActivationService.UpdateActivationAsync(NewActivationKey);
                        switch (result)
                        {
                            case true:
                                CurrentActivationKey = NewActivationKey;
                                NewActivationKey = string.Empty;
                                SharedAppData.ActivationStatus = ActivationStatus.Activated;
                                ApplicationUtility.ShowInformationMessage(Localization.GetResource("ToolHasBeenActivated"), InformationKind.Success);
                                break;
                            case false:
                                ApplicationUtility.ShowInformationMessage(Localization.GetResource("LicenseKeyIsNotValid"), InformationKind.Error);
                                break;
                            case null:
                                ApplicationUtility.ShowInformationMessage(Localization.GetResource("LicenseKeyIsExpired"), InformationKind.Error);
                                return;
                            default:
                                throw new ArgumentOutOfRangeException(result.ToString());
                        }
                    },
                    Localization.GetResource("UpdatingActivationKeyFetchMessage"));
                },
                () =>
                {
                    return !string.IsNullOrEmpty(NewActivationKey) && NewActivationKey != CurrentActivationKey;
                }
            )
            .ObservesProperty(() => NewActivationKey)
            .ObservesProperty(() => CurrentActivationKey);
        }

        public ICommand RemoveActivationCmd
        {
            get => new DelegateCommand(
                async () =>
                {
                    await ApplicationUtility.ExecuteFetchDataAsync(async () =>
                    {
                        var result = await ActivationService.DeactivateLicenseAsync();
                        if (!result)
                        {
                            return;
                        }
                        
                        SharedAppData.ActivationStatus = ActivationStatus.NotActivated;
                        CurrentActivationKey = string.Empty;

                        RunTaskToCloseToolAfter5minutes();

                        ApplicationUtility.ShowInformationMessage(string.Format(Localization.GetResource("LicenseKeyIsResetStatusFormat")), InformationKind.Success);
                    },
                    Localization.GetResource("RemovingActivationFetchMessage"));
                },
                () =>
                {
                    return !string.IsNullOrEmpty(CurrentActivationKey);
                }
            )
            .ObservesProperty(() => NewActivationKey)
            .ObservesProperty(() => CurrentActivationKey);
        }

        public ICommand RefreshActivationCmd
        {
            get => new DelegateCommand(
                async () =>
                {
                    var activationStatus = await ApplicationUtility.ExecuteFetchDataAsync(
                        () => ActivationService.IsActivatedAsync(),
                        Localization.GetResource("CheckingActivationFetchMessage"));

                    SharedAppData.ActivationStatus = activationStatus;
                    switch (activationStatus)
                    {
                        case ActivationStatus.NotActivated:
                            break;
                        case ActivationStatus.Expired:
                            ApplicationUtility.ShowInformationMessage(Localization.GetResource("ActivationViewThisActivationKeyIsExpired"), InformationKind.Warning);
                            break;
                        default:
                            break;
                    }
                },
                () =>
                {
                    return SharedAppData.ActivationStatus != ActivationStatus.Activated
                        || SharedAppData.ActivationStatus != ActivationStatus.NotActivated;
                }
            )
            .ObservesProperty(() => SharedAppData.ActivationStatus);
        }
        #endregion

        #region Properties
        public string CurrentActivationKey
        {
            get { return _currentActivationKey; }
            set { SetProperty(ref _currentActivationKey, value); }
        }

        public string NewActivationKey
        {
            get { return _newActivationKey; }
            set { SetProperty(ref _newActivationKey, value); }
        }

        public ISharedAppDataViewModel SharedAppData
        {
            get { return _sharedAppData; }
            set { SetProperty(ref _sharedAppData, value); }
        }
        #endregion

        public ActivationViewModel(IActivationService activationService,
            ISharedAppDataViewModel sharedAppData)
        {
            CurrentActivationKey = activationService.Key;
            SharedAppData = sharedAppData;
        }

        public void RunTaskToCloseToolAfter5minutes()
        {
            Task.Run(async () => 
            {
                await Task.Delay(TimeSpan.FromMinutes(5));
                if (SharedAppData.ActivationStatus != ActivationStatus.Activated)
                {
                    Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
                }
            });
        }
    }
}
