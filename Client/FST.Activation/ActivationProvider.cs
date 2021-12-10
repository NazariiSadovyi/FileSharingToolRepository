using FST.Activation.Responses;
using System;
using System.Threading.Tasks;

namespace FST.Activation
{
    public class ActivationProvider : IActivationProvider
    {
        private readonly ActivationApiClient _activationApiClient = new();
        private readonly LicenseKeyProvider _licenseKeyProvider = new();

        public async Task<bool> CheckAndSaveLicense(string key)
        {
            var response = await _activationApiClient.ActivateToolAsync(key);
            if (response.State == ActivationKeyStateEnum.Correct)
            {
                await LicenceUpdate(key);
                return true;
            }
            else if (response.State == ActivationKeyStateEnum.Incorrect)
            {
                return false;
            }
            
            throw new Exception("Sorry, but this key is expaired.");
        }

        public string GetSavedLicenseKey()
        {
            return _licenseKeyProvider.Key;
        }

        public async Task<bool> IsActivatedCheck()
        {
            var currentKey = _licenseKeyProvider.Key;
            if (string.IsNullOrEmpty(currentKey))
            {
                return false;
            }

            var response = await _activationApiClient.CheckAsync(currentKey);
            if (response.State == ActivationKeyStateEnum.Correct)
            {
                return true;
            }
            else if (response.State == ActivationKeyStateEnum.Incorrect)
            {
                return false;
            }

            throw new Exception("Sorry, but this key is expaired.");
        }

        public async Task DeactivateLicense()
        {
            var response = await _activationApiClient.ResetAsync(_licenseKeyProvider.Key);
            if (response.State == ActivationKeyStateEnum.Reset)
            {
                await Task.Run(() => _licenseKeyProvider.Key = string.Empty);
                return;
            }

            throw new Exception($"Activation key wasn't reset. Server result: {response?.State}");
        }

        private async Task LicenceUpdate(string key)
        {
            await Task.Run(() => _licenseKeyProvider.Key = key);
        }
    }
}
