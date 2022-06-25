using QRSharingApp.Common.Models;
using System;
using System.Collections.Generic;

namespace QRSharingApp.Common.Services.Interfaces
{
    public interface IWebServerService
    {
        event EventHandler NetworkChanged;
        string WebLocalhostUrl { get; }

        string GetWebUrl(string networkId);
        string GetFilePath(string fileId, string networkId);
        IEnumerable<string> GetFilesPathes(string[] fileIds, string networkId);
        List<NetworkInformationModel> GetAvailableNetworks();
    }
}
