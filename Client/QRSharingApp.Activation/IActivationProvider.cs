using System.Threading.Tasks;

namespace QRSharingApp.Activation
{
    public interface IActivationProvider
    {
        string MachineId { get; }

        Task<bool?> CheckAndSaveLicense(string key);
        Task<bool?> IsActivatedCheck();
        string GetSavedLicenseKey();
        Task DeactivateLicense();
    }
}