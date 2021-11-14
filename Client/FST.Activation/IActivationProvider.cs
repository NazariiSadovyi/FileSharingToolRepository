using System.Threading.Tasks;

namespace FST.Activation
{
    public interface IActivationProvider
    {
        Task<bool> CheckAndSaveLicense(string key);
        Task<bool> IsActivatedCheck();
        Task LicenceUpdate(string key);
        string GetSavedLicenseKey();
        Task DeactivateLicense();
    }
}