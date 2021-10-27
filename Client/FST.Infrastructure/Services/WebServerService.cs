using FST.Infrastructure.Services.Interfaces;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace FST.Infrastructure.Services
{
    public class WebServerService : IWebServerService
    {
        public string GetFilePath(string fileId)
        {
            var localIp = GetLocalAdress();
            if (localIp == null)
            {
                return null;
            }

            return BuildFilePath(localIp, fileId);
        }

        public IEnumerable<string> GetFilesPathes(string[] fileIds)
        {
            var localIp = GetLocalAdress();
            if (localIp == null)
            {
                yield break;
            }

            foreach (var fileId in fileIds)
            {
                yield return BuildFilePath(localIp, fileId);
            }
        }

        private string BuildFilePath(string localIp, string fileId)
        {
            return $@"http://{localIp}:56/home/file?id={fileId}";
        }

        private string GetLocalAdress()
        {
            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 
                    && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }

            return null;
        }
    }
}
