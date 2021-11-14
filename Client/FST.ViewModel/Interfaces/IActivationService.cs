using System.Threading.Tasks;

namespace FST.ViewModel.Interfaces
{
    public interface IActivationService
    {
        string Key { get; }
        Task<bool?> IsActivated();
        Task<bool> ResetCurrentActivation();
        Task<bool?> UpdateActivation(string key);
        Task DeactivateLicense();
    }
}
