using QRSharingApp.Common.Enums;

namespace QRSharingApp.Common.Services.Interfaces
{
    public interface IWifiService
    {
        string GenerateConfigString(string ssid, WifiAuthenticationType wifiAuthenticationType, string password = "", bool isHidden = false);
    }
}
