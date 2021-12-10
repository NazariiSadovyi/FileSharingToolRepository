using FST.Activation;
using FST.ViewModel.Interfaces;
using FST.ViewModel.Models;
using NLog;
using System;
using System.Threading.Tasks;
using Localization = FST.CultureLocalization.Localization;

namespace FST.ViewModel.Services
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

        public async Task<bool?> IsActivated()
        {
            try
            {
                return await _activationProvider.IsActivatedCheck();
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Activation error: Can't check activation status");
                _applicationTaskUtility.ShowInformationMessage(Localization.GetResource("CantCheckActivationStatus"), InformationKind.Error);

                return null;
            }
        }

        public async Task<bool?> UpdateActivation(string key)
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

        public async Task DeactivateLicense()
        {
            try
            {
                await _activationProvider.DeactivateLicense();
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Deactivate License");
                _applicationTaskUtility.ShowInformationMessage(Localization.GetResource("CantDeactivateLicense"), InformationKind.Error);
            }
        }

        public async Task<bool> ResetCurrentActivation()
        {
            try
            {
                await _activationProvider.DeactivateLicense();
                return true;
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Reset Activation");
                _applicationTaskUtility.ShowInformationMessage(Localization.GetResource("CantResetLicense"), InformationKind.Error);

                return false;
            }
        }
    }
}