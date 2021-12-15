using QRSharingApp.Infrastructure.Enums;

namespace QRSharingApp.Infrastructure.Services.Interfaces
{
    public interface IWifiService
    {
        string GenerateConfigString(string ssid, WifiAuthenticationType wifiAuthenticationType, string password = "", bool isHidden = false);
    }
}
