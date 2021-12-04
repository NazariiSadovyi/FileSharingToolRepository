using FST.ViewModel.Interfaces;
using FST.ViewModel.Models;
using FST.ViewModel.ViewModels.Base;
using FST.ViewModel.ViewModels.Interfaces;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity;
using Localization = FST.CultureLocalization.Localization;

namespace FST.ViewModel.ViewModels
{
    public class ActivationViewModel : BaseNavigationViewModel
    {
        #region Private fields
        private string _newActivationKey;
        private string _currentActivationKey;
        #endregion

        #region Dependency
        [Dependency]
        public IActivationService ActivationService;
        [Dependency]
        public ISharedAppDataViewModel SharedAppData;
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
                        var result = await ActivationService.UpdateActivation(NewActivationKey);
                        if (!result.HasValue)
                        {
                            return;
                        }

                        if (result.Value)
                        {
                            CurrentActivationKey = NewActivationKey;
                            NewActivationKey = string.Empty;
                            SharedAppData.IsActivated = true;

                            ApplicationUtility.ShowInformationMessage(Localization.GetResource("ToolHasBeenActivated"), InformationKind.Success);
                        }
                        else
                        {
                            ApplicationUtility.ShowInformationMessage(Localization.GetResource("LicenseKeyIsNotValid"), InformationKind.Error);
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
                        await ActivationService.DeactivateLicense();
                        
                        SharedAppData.IsActivated = false;
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
        #endregion

        public ActivationViewModel(IActivationService _activationService)
        {
            CurrentActivationKey = _activationService.Key;
        }

        public void RunTaskToCloseToolAfter5minutes()
        {
            Task.Run(async () => 
            {
                await Task.Delay(TimeSpan.FromMinutes(5));
                if (!SharedAppData.IsActivated)
                {
                    Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
                }
            });
        }
    }
}
