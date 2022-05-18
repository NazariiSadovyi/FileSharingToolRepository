using System.Threading.Tasks;

namespace QRSharingApp.ClientApi.Interfaces
{
    public interface IClientProvider
    {
        Task DeleteAsync(string requestUri);
        Task<TResponse> DeleteWithReponseAsync<TResponse>(string requestUri) where TResponse : class;
        T Get<T>(string requestUri) where T : class;
        Task<T> GetAsync<T>(string requestUri) where T : class;
        void Post<T>(string requestUri, T data) where T : class;
        Task PostAsync<T>(string requestUri, T data) where T : class;
        Task<TResponse> PostWithResponseAsync<TRequest, TResponse>(string requestUri, TRequest data)
            where TRequest : class
            where TResponse : class;
    }
}
