using FST.ViewModel.Models;
using System.Threading.Tasks;

namespace FST.ViewModel.Interfaces
{
    public interface IActivationService
    {
        string Key { get; }
        Task<ActivationStatus> IsActivatedAsync();
        Task<bool?> UpdateActivationAsync(string key);
        Task<bool> DeactivateLicenseAsync();
    }
}
