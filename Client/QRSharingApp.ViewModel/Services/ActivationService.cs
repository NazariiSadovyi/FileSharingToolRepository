using QRSharingApp.Activation;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.Models;
using NLog;
using System;
using System.Threading.Tasks;
using Localization = QRSharingApp.CultureLocalization.Localization;

namespace QRSharingApp.ViewModel.Services
{
    public class ActivationService : IActivationService
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IActivationProvider _activationProvider;
        public readonly IApplicationTaskUtility _applicationTaskUtility;

        public string Key { get => _activationProvider.GetSavedLicenseKey(); }

        public ActivationService(IApplicationTaskUtility applicationTaskUtility)
        {
            _activationProvider = new ActivationProvider();
            _applicationTaskUtility = applicationTaskUtility;
        }

        public async Task<ActivationStatus> IsActivatedAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Key))
                {
                    return ActivationStatus.NotActivated;
                }

                var result = await _activationProvider.IsActivatedCheck();
                switch (result)
                {
                    case true:
                        return ActivationStatus.Activated;
                    case false:
                        return ActivationStatus.NotActivated;
                    case null:
                        return ActivationStatus.Expired;
                    default:
                        throw new ArgumentOutOfRangeException(result.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Activation error: Can't check activation status");
                _applicationTaskUtility.ShowInformationMessage(Localization.GetResource("CantCheckActivationStatus"), InformationKind.Error);

                return ActivationStatus.Error;
            }
        }

        public async Task<bool?> UpdateActivationAsync(string key)
        {
            try
            {
                return await _activationProvider.CheckAndSaveLicense(key);
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Can't update activation");
                _applicationTaskUtility.ShowInformationMessage(Localization.GetResource("CantUpdateActivation"), InformationKind.Error);

                return null;
            }
        }

        public async Task<bool> DeactivateLicenseAsync()
        {
            try
            {
                await _activationProvider.DeactivateLicense();
                return true;
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Deactivate License");
                _applicationTaskUtility.ShowInformationMessage(Localization.GetResource("CantDeactivateLicense"), InformationKind.Error);

                return false;
            }
        }
    }
}