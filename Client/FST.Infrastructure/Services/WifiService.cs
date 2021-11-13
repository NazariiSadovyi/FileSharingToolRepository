using FST.Infrastructure.Enums;
using FST.Infrastructure.Services.Interfaces;

namespace FST.Infrastructure.Services
{
    public class WifiService : IWifiService
    {
        public string GenerateConfigString(string ssid, WifiAuthenticationType wifiAuthenticationType, string password = "", bool isHidden = false)
        {
            /*
             * Order of fields does not matter.
             * Special characters "", ";", "," and ":" should be escaped with a backslash ("") as in MECARD encoding.
             * For example, if an SSID was literally "foo;bar\baz" (with double quotes part of the SSID name itself)
             * then it would be encoded like: WIFI:S:\"foo\;bar\\baz\";;
             */
            ssid = ssid.Replace(@"""", @"\""").Replace(";", @"\;").Replace(",", @"\,").Replace(":", @"\:");
            password = password.Replace(@"""", @"\""").Replace(";", @"\;").Replace(",", @"\,").Replace(":", @"\:");

            /*
             * Generate Wi-Fi Network config.
             */
            string wifiContext = "";
            wifiContext += $"WIFI:T:{wifiAuthenticationType};";
            wifiContext += $"S:{ssid};";
            wifiContext += wifiAuthenticationType == WifiAuthenticationType.Nopass ? $"P:{password};" : ";";
            wifiContext += isHidden ? "H:True;" : ";";

            return wifiContext;
        }
    }
}