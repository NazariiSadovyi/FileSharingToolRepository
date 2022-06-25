using QRSharingApp.Common.Models;
using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace QRSharingApp.Common.Services
{
    public class WebServerService : IWebServerService
    {
        private NetworkInterfaceType[] _supportedNetworkTypes = new[] { NetworkInterfaceType.Wireless80211, NetworkInterfaceType.Ethernet };

        public string WebLocalhostUrl => SharedConstants.LocalhostPath;
        public event EventHandler NetworkChanged;

        public WebServerService()
        {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }

        public string GetWebUrl(string networkId)
        {
            var localIp = GetLocalAdress(networkId);
            if (localIp == null)
            {
                return null;
            }

            return $@"http://{localIp}:{SharedConstants.Port}";
        }

        public string GetFilePath(string fileId, string networkId)
        {
            var localIp = GetLocalAdress(networkId);
            if (localIp == null)
            {
                return null;
            }

            return BuildFilePath(localIp, fileId);
        }

        public IEnumerable<string> GetFilesPathes(string[] fileIds, string networkId)
        {
            var localIp = GetLocalAdress(networkId);
            if (localIp == null)
            {
                yield break;
            }

            foreach (var fileId in fileIds)
            {
                yield return BuildFilePath(localIp, fileId);
            }
        }

        public List<NetworkInformationModel> GetAvailableNetworks()
        {
            var networks = new List<NetworkInformationModel>();
            foreach (var item in NetworkInterface.GetAllNetworkInterfaces().Where(_ => _supportedNetworkTypes.Contains(_.NetworkInterfaceType)))
            {
                var network = new NetworkInformationModel();
                network.Id = item.Id;
                network.Name = item.Name;
                network.Description = item.Description;
                network.IsUp = item.OperationalStatus == OperationalStatus.Up;

                switch (item.NetworkInterfaceType)
                {
                    case NetworkInterfaceType.Wireless80211:
                        network.Adress = item.GetIPProperties()
                            .UnicastAddresses?
                            .FirstOrDefault(_ => _.Address.AddressFamily == AddressFamily.InterNetwork)?
                            .Address?
                            .ToString();
                        break;
                    case NetworkInterfaceType.Ethernet:
                        network.Adress = item.GetIPProperties()
                            .UnicastAddresses?
                            .FirstOrDefault(_ => _.Address.AddressFamily == AddressFamily.InterNetwork)?
                            .Address?
                            .ToString();
                        break;
                    default:
                        break;
                }

                networks.Add(network);
            }

            return networks;
        }

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            NetworkChanged?.Invoke(this, e);
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            NetworkChanged?.Invoke(this, e);
        }

        private string BuildFilePath(string localIp, string fileId)
        {
            return $@"http://{localIp}:{SharedConstants.Port}/file/preview/{fileId}";
        }

        private string GetLocalAdress(string networkId)
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()?
                .FirstOrDefault(_ => _.Id == networkId)?
                .GetIPProperties()?
                .UnicastAddresses?
                .FirstOrDefault(_ => _.Address.AddressFamily == AddressFamily.InterNetwork)?
                .Address?
                .ToString();
        }
    }
}
