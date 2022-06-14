using QRSharingApp.ViewModel.Models;
using System.Threading.Tasks;

namespace QRSharingApp.ViewModel.Interfaces
{
    public interface IActivationService
    {
        string Key { get; }
        string MachineId { get; }

        Task<ActivationStatus> IsActivatedAsync();
        Task<bool?> UpdateActivationAsync(string key);
        Task<bool> DeactivateLicenseAsync();
    }
}
