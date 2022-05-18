using Newtonsoft.Json;
using QRSharingApp.ClientApi.Interfaces;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QRSharingApp.ClientApi.Implementations
{
    public class ClientProvider : IClientProvider
    {
        private HttpClient _httpClient { get; set; }

        public ClientProvider(string baseAdress)
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseAdress)
            };
        }

        public async Task<T> GetAsync<T>(string requestUri) where T : class
        {
            var response = await _httpClient.GetAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                throw new NotImplementedException();
            }

            var content = await response.Content.ReadAsStringAsync();
            if (typeof(T) == typeof(string))
            {
                return content as T;
            }

            return JsonConvert.DeserializeObject<T>(content);
        }

        public T Get<T>(string requestUri) where T : class
        {
            var response = _httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new NotImplementedException();
            }

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (typeof(T) == typeof(string))
            {
                return content as T;
            }

            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task PostAsync<T>(string requestUri, T data) where T : class
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUri, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new NotImplementedException();
            }
        }

        public void Post<T>(string requestUri, T data) where T : class
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(requestUri, content).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<TResponse> PostWithResponseAsync<TRequest, TResponse>(string requestUri, TRequest data)
            where TRequest : class
            where TResponse : class
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUri, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new NotImplementedException();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TResponse>(json);

            return result;
        }

        public async Task DeleteAsync(string requestUri)
        {
            var response = await _httpClient.DeleteAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<TResponse> DeleteWithReponseAsync<TResponse>(string requestUri) where TResponse : class
        {
            var response = await _httpClient.DeleteAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                throw new NotImplementedException();
            }

            var content = await response.Content.ReadAsStringAsync();
            if (typeof(TResponse) == typeof(string))
            {
                return content as TResponse;
            }

            return JsonConvert.DeserializeObject<TResponse>(content);
        }
    }
}
