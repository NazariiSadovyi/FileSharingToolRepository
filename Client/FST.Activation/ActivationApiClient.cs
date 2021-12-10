using DeviceId;
using FST.Activation.Responses;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FST.Activation
{
    internal class ActivationApiClient
    {
        private const string _programToolId = "24518803-3abb-408d-9f3e-74789017240d";
        private const string _serviceAdress = "https://localhost:5001/";

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

        public async Task<ActivationStatusResponse> ResetAsync(string key)
        {
            return await GetAsync(key, "reset");
        }
        
        private async Task<ActivationStatusResponse> GetAsync(string key, string method)
        {
            var requestUri = Path.Combine(GetBaseActionAdress(key), method);
            requestUri = requestUri.Replace(@"\", @"/");
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
                .AddMacAddress()
                .AddProcessorId()
                .AddMotherboardSerialNumber()
                .ToString();
        }
    }

}