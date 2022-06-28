using NLog;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace QRSharingApp.Activation
{
    public partial class ActivationProvider
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private static readonly LicenseKeyProvider _licenseKeyProvider = new LicenseKeyProvider();
        private static readonly RestClient _client;

        //Версия программы
        private const string actualVersion = "1.0.0";
        //Адрес к скрипту
        private const string url = "http://icu.by/scripts/QrLan/Activation_QR_Lan.php";

        public string MachineId => HardwareHelper.GetUniqueWinId();

        static ActivationProvider()
        {
            _client = new RestClient()
            {
                BaseUrl = new Uri(url),
                Timeout = 5000
            };
        }

        //Активация
        public async Task<bool> CheckAndSaveLicense(string key)
        {
            var isKeyInUse = await IsKeyInUse(key);
            if (isKeyInUse)
            {
                return false;
            }

            var request = new RestRequest(Method.POST);
            request.AddParameter("unity_hash", "728079e5fd7ae643749657989a325281");
            request.AddParameter("key", key);
            request.AddParameter("state", "activation");

            var response = await _client.ExecuteAsync(request);
            string responseString = response.Content;

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }
            else
            {
                if (responseString.Equals("correct"))
                {
                    LicenceUpdate(key);
                    _licenseKeyProvider.Key = key;

                    return true;
                }

                return false;
            }
        }

        // Проверка активации
        public async Task<bool> IsActivatedCheck()
        {
            var savedKey = GetSavedLicenseKey();
            if (string.IsNullOrEmpty(savedKey))
            {
                return false;
            }

            var request = new RestRequest(Method.POST);
            request.AddParameter("unity_hash", "728079e5fd7ae643749657989a325281");
            request.AddParameter("key", savedKey);
            request.AddParameter("state", "check");

            var restResponse = await _client.ExecuteAsync(request);
            var responseString = restResponse.Content;

            var uniqueWinId = HardwareHelper.GetUniqueWinId();
            if (uniqueWinId == responseString)
            {
                return true;
            }

            return false;
        }

        //Вносим данные в таблицу
        public void LicenceUpdate(string key)
        {
            var request = new RestRequest(Method.POST);
            request.AddParameter("key", key);
            request.AddParameter("unity_hash", "728079e5fd7ae643749657989a325281");
            request.AddParameter("state", "update");

            try
            {
                request.AddParameter("mac", HardwareHelper.GetCpu());

            }
            catch (Exception)
            {
                request.AddParameter("mac", "disabled");
            }

            try
            {
                request.AddParameter("motherBoardId", HardwareHelper.GetMotherBoard_ID());
            }
            catch (Exception)
            {
                request.AddParameter("motherBoardId", "disabled");
            }

            try
            {
                request.AddParameter("hddSerial", actualVersion);
            }
            catch (Exception)
            {
                request.AddParameter("hddSerial", "disabled");
            }

            try
            {
                request.AddParameter("winId", HardwareHelper.GetUniqueWinId());
            }
            catch (Exception)
            {
                request.AddParameter("winId", "disabled");
            }

            try
            {
                request.AddParameter("date", DateTime.Now.ToString("dd/MM/yyyy HH:mm \"GMT\"zzz"));
            }
            catch (Exception)
            {
                request.AddParameter("date", "disabled");
            }

            try
            {
                request.AddParameter("ip", HardwareHelper.GetExternalIPAddress());
            }
            catch (Exception)
            {
                request.AddParameter("ip", "disabled");
            }

            var response = _client.Execute(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            if (!response.IsSuccessful)
            {
                throw new Exception($"LicenceUpdate is not success. Actual status code: {response.StatusCode}");
            }
        }

        //Обновляем время запуска программы
        public void TimeUpdate(string key)
        {
            var request = new RestRequest(Method.POST);
            request.AddParameter("key", key);
            request.AddParameter("unity_hash", "728079e5fd7ae643749657989a325281");
            request.AddParameter("state", "updateDate");

            try
            {
                request.AddParameter("date", DateTime.Now.ToString("dd/MM/yyyy HH:mm \"GMT\"zzz"));
            }
            catch (Exception)
            {
                request.AddParameter("date", "disabled");
            }

            try
            {
                request.AddParameter("ip", HardwareHelper.GetExternalIPAddress());
            }
            catch (Exception)
            {
                request.AddParameter("ip", "disabled");
            }

            var response = _client.Execute(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            if (!response.IsSuccessful)
            {
                throw new Exception($"TimeUpdate is not success. Actual status code: {response.StatusCode}");
            }
        }

        //Проверка перед закрытием программы
        public void CheckBeforeClosing()
        {
            var request = new RestRequest(Method.POST);

            var response = _client.Execute(request);
            string responseString = response.Content;

            if (!response.IsSuccessful)
            {
                _logger.Error("No connection: " + response.StatusCode);
                return;
            }

            var key = _licenseKeyProvider.Key;

            IsActivatedCheckNew(key);
            TimeUpdate(key);

            var versionFromDb = GetVersion(key);

            //Если версия новее обновляем ее в БД
            if (versionFromDb != actualVersion)
            {
                LicenceUpdate(key);
            }
        }

        public string GetSavedLicenseKey()
        {
            return _licenseKeyProvider.Key;
        }

        //Удаление ключа
        public async Task<string> RemoveKey()
        {
            var newKey = await DeactivateLicense(_licenseKeyProvider.Key);
            _licenseKeyProvider.Key = string.Empty;

            return newKey;
        }

        //Получаем версию программы
        public async Task<string> DeactivateLicense(string key)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12
                | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var newKey = BullShitLogic(key);

            var request = new RestRequest(Method.POST);
            request.AddParameter("unity_hash", "728079e5fd7ae643749657989a325281");
            request.AddParameter("key", key);
            request.AddParameter("keyNew", newKey);
            request.AddParameter("state", "transfer");

            var response = await _client.ExecuteAsync(request);
            var responseString = response.Content;

            if (responseString.Equals("OK"))
            {
                return newKey;
            }

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            throw new Exception($"UpdateCurrentLicense is not success. Actual status code: {response.StatusCode}, and response string: {responseString}.");
        }

        //Проверка ключа в базе
        private void IsActivatedCheckNew(string key)
        {
            var request = new RestRequest(Method.POST);
            request.AddParameter("unity_hash", "728079e5fd7ae643749657989a325281");
            request.AddParameter("key", key);
            request.AddParameter("state", "check");

            _client.Execute(request);
        }

        //Получаем версию программы
        private string GetVersion(string key)
        {
            string version;

            var request = new RestRequest(Method.POST);
            request.AddParameter("key", key);
            request.AddParameter("unity_hash", "728079e5fd7ae643749657989a325281");
            request.AddParameter("state", "getVersion");

            var response = _client.Execute(request);

            if (!response.IsSuccessful)
            {
                version = "error";
            }
            else
            {
                version = response.Content.ToString();
            }

            return version;
        }

        private string BullShitLogic(string currentKey)
        {
            int intResult;
            var result = currentKey.Substring(currentKey.Length - 3);
            try
            {
                intResult = Convert.ToInt32(result) + 1;
            }
            catch
            {
                intResult = 001;
            }

            var valueTo3Format = string.Format("{0:D3}", intResult);
            var newKey = currentKey.Remove(currentKey.Length - 3) + valueTo3Format;

            return newKey;
        }

        private async Task<bool> IsKeyInUse(string key)
        {
            var request = new RestRequest(Method.POST);
            request.AddParameter("unity_hash", "728079e5fd7ae643749657989a325281");
            request.AddParameter("key", key);
            request.AddParameter("state", "check");

            var restResponse = await _client.ExecuteAsync(request);
            var responseString = restResponse.Content;

            var uniqueWinId = HardwareHelper.GetUniqueWinId();
            if (responseString == uniqueWinId)
            {
                return false;
            }

            if (responseString == "0" || responseString == "")
            {
                return false;
            }

            return true;
        }
    }
}
