using QRSharingApp.Common.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace QRSharingApp.Common.Services
{
    public class WebServerService : IWebServerService
    {
        private readonly int _port = 5666;

        public event EventHandler<bool> NetworkChanged;
        public string WebLocalhostUrl => $"http://localhost:{_port}";

        public WebServerService()
        {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

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

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            var localAdress = GetLocalAdress();
            NetworkChanged?.Invoke(this, !string.IsNullOrEmpty(localAdress));
        }

        private string BuildFilePath(string localIp, string fileId)
        {
            return $@"http://{localIp}:{_port}/file/preview/{fileId}";
        }

        private string GetLocalAdress()
        {
            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && item.OperationalStatus == OperationalStatus.Up)
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

            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == NetworkInterfaceType.Ethernet && item.OperationalStatus == OperationalStatus.Up)
                {
                    var ipProperties = item.GetIPProperties();
                    foreach (var ip in ipProperties.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            if (ipProperties.GatewayAddresses.FirstOrDefault() != null)
                            {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
