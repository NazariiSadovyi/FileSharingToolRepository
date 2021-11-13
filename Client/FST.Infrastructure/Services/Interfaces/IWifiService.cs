using FST.Infrastructure.Enums;

namespace FST.Infrastructure.Services.Interfaces
{
    public interface IWifiService
    {
        string GenerateConfigString(string ssid, WifiAuthenticationType wifiAuthenticationType, string password = "", bool isHidden = false);
    }
}
