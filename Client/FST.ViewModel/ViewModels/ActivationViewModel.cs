using FST.ViewModel.Helpers;
using FST.ViewModel.Interfaces;
using FST.ViewModel.Models;
using FST.ViewModel.Services;
using FST.ViewModel.ViewModels.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
using Unity;
using Localization = FST.CultureLocalization.Localization;

namespace FST.ViewModel.ViewModels
{
    public class ActivationViewModel : BindableBase//, INavigationAware
    {
        //#region Private fields
        //private ICommand _activateCmd;
        //private ICommand _removeActivationCmd;
        //private string _newActivationKey;
        //private string _currentActivationKey;
        //#endregion

        //#region Dependency
        //[Dependency]
        //public IDialogService DialogService;
        //[Dependency]
        //public IActivationService ActivationService;
        //[Dependency]
        //public ISharedAppDataViewModel SharedAppData;
        //[Dependency]
        //public IApplicationTaskUtility ApplicationUtility;
        //#endregion

        //#region Commands
        //public ICommand ActivateCmd
        //{
        //    get
        //    {
        //        return _activateCmd ??
        //          (_activateCmd = new DelegateCommand(
        //              async () => {
        //                  await ApplicationUtility.ExecuteFetchDataAsync(async () => 
        //                  {
        //                      var result = await ActivationService.UpdateActivation(NewActivationKey);
        //                      if (!result.HasValue)
        //                      {
        //                          return;
        //                      }

        //                      if (result.Value)
        //                      {
        //                          CurrentActivationKey = NewActivationKey;
        //                          NewActivationKey = string.Empty;
        //                          SharedAppData.IsActivated = true;

        //                          ApplicationUtility.ShowInformationMessage(Localization.GetResource("ToolHasBeenActivated"), InformationKind.Success);
        //                      }
        //                      else
        //                      {
        //                          ApplicationUtility.ShowInformationMessage(Localization.GetResource("LicenseKeyIsNotValid"), InformationKind.Error);
        //                      }
        //                  },
        //                  Localization.GetResource("UpdatingActivationKeyFetchMessage"));
        //              },
        //              () => 
        //              {
        //                  return !string.IsNullOrEmpty(NewActivationKey) && NewActivationKey != CurrentActivationKey;
        //              }
        //          )
        //          .ObservesProperty(() => NewActivationKey)
        //          .ObservesProperty(() => CurrentActivationKey));
        //    }
        //}

        //public ICommand RemoveActivationCmd
        //{
        //    get
        //    {
        //        return _removeActivationCmd ??
        //          (_removeActivationCmd = new DelegateCommand(
        //              async () => {
        //                  await ApplicationUtility.ExecuteFetchDataAsync(async () =>
        //                  {
        //                      var newKey = await ActivationService.DeactivateLicense(CurrentActivationKey);
        //                      if (string.IsNullOrEmpty(newKey))
        //                      {
        //                          return;
        //                      }

        //                      System.Windows.Clipboard.SetText(newKey);
        //                      var result = await ActivationService.ResetCurrentActivation();
        //                      if (!result)
        //                      {
        //                          return;
        //                      }

        //                      SharedAppData.IsActivated = false;
        //                      CurrentActivationKey = string.Empty;

        //                      ApplicationUtility.ShowInformationMessage(string.Format(Localization.GetResource("LicenseKeyIsResetStatusFormat"), newKey), InformationKind.Success);
        //                  },
        //                  Localization.GetResource("RemovingActivationFetchMessage"));
        //              },
        //              () =>
        //              {
        //                  return !string.IsNullOrEmpty(CurrentActivationKey);
        //              }
        //          )
        //          .ObservesProperty(() => NewActivationKey)
        //          .ObservesProperty(() => CurrentActivationKey));
        //    }
        //}
        //#endregion

        //public string CurrentActivationKey
        //{
        //    get { return _currentActivationKey; }
        //    set { SetProperty(ref _currentActivationKey, value); }
        //}

        //public string NewActivationKey
        //{
        //    get { return _newActivationKey; }
        //    set { SetProperty(ref _newActivationKey, value); }
        //}

        //public ActivationViewModel(IActivationService _activationService)
        //{
        //    CurrentActivationKey = _activationService.Key;
        //}

        //#region Navigation
        //public bool IsNavigationTarget(NavigationContext navigationContext)
        //{
        //    return true;
        //}

        //public void OnNavigatedFrom(NavigationContext navigationContext)
        //{

        //}

        //public void OnNavigatedTo(NavigationContext navigationContext)
        //{

        //}
        //#endregion
    }
}
