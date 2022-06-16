using QRSharingApp.Infrastructure.Settings.Interfaces;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.Models;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity;
using Localization = QRSharingApp.CultureLocalization.Localization;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class ActivationViewModel : BaseNavigationViewModel
    {
        #region Dependency
        [Dependency]
        public IActivationService ActivationService;
        [Dependency]
        public IApplicationTaskUtility ApplicationUtility;
        [Dependency]
        public IAppSetting AppSetting;
        #endregion

        #region Commands
        public ICommand ActivateCmd => ReactiveCommand.CreateFromTask(
            async () =>
            {
                await ApplicationUtility.ExecuteFetchDataAsync(async () =>
                {
                    var result = await ActivationService.UpdateActivationAsync(NewActivationKey);
                    switch (result)
                    {
                        case true:
                            if (!string.IsNullOrEmpty(CurrentActivationKey))
                            {
                                LastActivationKey = CurrentActivationKey;
                                AppSetting.LastActivationKey = CurrentActivationKey;
                            }
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
            Observable.CombineLatest(this.WhenAnyValue(_ => _.NewActivationKey), this.WhenAnyValue(_ => _.CurrentActivationKey),
                (newActivationKey, currentActivationKey) => !string.IsNullOrEmpty(newActivationKey) && newActivationKey != currentActivationKey)
        );

        public ICommand RemoveActivationCmd => ReactiveCommand.CreateFromTask(
            async () =>
            {
                await ApplicationUtility.ExecuteFetchDataAsync(async () =>
                {
                    var result = await ActivationService.DeactivateLicenseAsync();
                    if (!result.Item1)
                    {
                        return;
                    }

                    SharedAppData.ActivationStatus = ActivationStatus.NotActivated;
                    LastActivationKey = result.Item2;
                    AppSetting.LastActivationKey = result.Item2;
                    CurrentActivationKey = string.Empty;

                    RunTaskToCloseToolAfter5minutes();

                    ApplicationUtility.ShowInformationMessage(string.Format(Localization.GetResource("LicenseKeyIsResetStatusFormat")), InformationKind.Success);
                },
                Localization.GetResource("RemovingActivationFetchMessage"));
            },
            Observable.CombineLatest(this.WhenAnyValue(_ => _.NewActivationKey), this.WhenAnyValue(_ => _.CurrentActivationKey),
                (newActivationKey, currentActivationKey) => !string.IsNullOrEmpty(currentActivationKey))
        );

        public ICommand RefreshActivationCmd => ReactiveCommand.CreateFromTask(
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
            SharedAppData.WhenAnyValue(_ => _.ActivationStatus).Select(s => s != ActivationStatus.Activated || s != ActivationStatus.NotActivated)
        );

        public ICommand SetTextToClipboardCmd => ReactiveCommand.Create<string>(
            (text) =>
            {
                Clipboard.SetText(text);
            }
        );
        #endregion

        #region Properties
        public string MachineId { get; set; }
        public string CurrentActivationKey { get; set; }
        public string LastActivationKey { get; set; }
        public string NewActivationKey { get; set; }
        public ISharedAppDataViewModel SharedAppData { get; set; }
        #endregion

        public ActivationViewModel(ISharedAppDataViewModel sharedAppData)
        {
            SharedAppData = sharedAppData;
        }

        public override Task OnLoadAsync()
        {
            CurrentActivationKey = ActivationService.Key;
            LastActivationKey = AppSetting.LastActivationKey;
            MachineId = ActivationService.MachineId;
            return Task.CompletedTask;
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
