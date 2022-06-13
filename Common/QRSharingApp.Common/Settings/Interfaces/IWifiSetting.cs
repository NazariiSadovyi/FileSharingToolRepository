namespace QRSharingApp.Common.Settings.Interfaces
{
    public interface IWifiSetting
    {
        string WifiLogin { get; set; }
        string WifiPassword { get; set; }
        int WifiAuthenticationType { get; set; }
        bool WifiIsHidden { get; set; }
    }
}
