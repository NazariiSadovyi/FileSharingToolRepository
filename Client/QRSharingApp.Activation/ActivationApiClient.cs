using DeviceId;
using QRSharingApp.Activation.Responses;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace QRSharingApp.Activation
{
    internal class ActivationApiClient
    {
        private const string _programToolId = "d64607db-429c-4ffb-ad09-44599fe79e9b";
        //private const string _serviceAdress = "https://localhost:5001/";
        private const string _serviceAdress = "http://spinner360-001-site1.btempurl.com/";

        private readonly HttpClient _client;

        public ActivationApiClient()
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(_serviceAdress)
            };
        }

        public async Task<ActivationStatusResponse> ActivateToolAsync(string key)
        {
            return await GetAsync(key, "activate");
        }

        public async Task<ActivationStatusResponse> CheckAsync(string key)
        {
            return await GetAsync(key, "check");
        }

        public async Task ResetAsync(string key)
        {
            var requestUri = GetMethodAdress(key, "reset");
            await _client.GetAsync(requestUri);
        }
        
        private async Task<ActivationStatusResponse> GetAsync(string key, string method)
        {
            var requestUri = GetMethodAdress(key, method);
            var response = await _client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var contentText = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ActivationStatusResponse>(contentText);
            }
            else
            {
                var contentText = await response.Content.ReadAsStringAsync();
                return new ActivationStatusResponse()
                {
                    State = ActivationKeyStateEnum.Incorrect,
                    Message = contentText
                };
            }
        }

        private string GetMethodAdress(string key, string method)
        {
            var requestUri = Path.Combine(GetBaseActionAdress(key), method);
            requestUri = requestUri.Replace(@"\", @"/");
            return requestUri;
        }

        private string GetBaseActionAdress(string key)
        {
            //Api adress: "api/activation/programTool/{programToolId}/key/{key}/machine/{machineId}/{method}"
            var baseApiControlleAdress = "api/activation";
            var programToolAdress = $"programTool/{_programToolId}";
            var keyAdress = $"key/{key}";
            var machineAdress = $"machine/{MachineId()}";
            var adress = Path.Combine(_serviceAdress, baseApiControlleAdress, programToolAdress, keyAdress, machineAdress);

            return adress;
        }

        private string MachineId()
        {
            return new DeviceIdBuilder()
                .AddMachineName()
                .AddSystemUUID()
                .AddProcessorId()
                .AddMotherboardSerialNumber()
                .ToString();
        }
    }

}