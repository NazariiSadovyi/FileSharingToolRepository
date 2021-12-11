using FST.Activation.Responses;
using System;
using System.Threading.Tasks;

namespace FST.Activation
{
    public class ActivationProvider : IActivationProvider
    {
        private readonly ActivationApiClient _activationApiClient = new();
        private readonly LicenseKeyProvider _licenseKeyProvider = new();

        public async Task<bool?> CheckAndSaveLicense(string key)
        {
            var response = await _activationApiClient.ActivateToolAsync(key);
            switch (response.State)
            {
                case ActivationKeyStateEnum.Correct:
                    await LicenceUpdate(key);
                    return true;
                case ActivationKeyStateEnum.Incorrect:
                    return false;
                case ActivationKeyStateEnum.Expired:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(response.State.ToString());
            }
        }

        public string GetSavedLicenseKey()
        {
            return _licenseKeyProvider.Key;
        }

        public async Task<bool?> IsActivatedCheck()
        {
            var currentKey = _licenseKeyProvider.Key;
            if (string.IsNullOrEmpty(currentKey))
            {
                return false;
            }

            var response = await _activationApiClient.CheckAsync(currentKey);
            switch (response.State)
            {
                case ActivationKeyStateEnum.Correct:
                    return true; 
                case ActivationKeyStateEnum.Incorrect:
                    return false;
                case ActivationKeyStateEnum.Expired:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(response.State.ToString());
            }
        }

        public async Task DeactivateLicense()
        {
            await _activationApiClient.ResetAsync(_licenseKeyProvider.Key);
            await Task.Run(() => _licenseKeyProvider.Key = string.Empty);
        }

        private async Task LicenceUpdate(string key)
        {
            await Task.Run(() => _licenseKeyProvider.Key = key);
        }
    }
}
