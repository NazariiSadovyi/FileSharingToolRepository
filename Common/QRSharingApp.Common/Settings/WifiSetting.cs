using QRSharingApp.Common.Settings.Interfaces;

namespace QRSharingApp.Common.Settings
{
    public abstract class WifiSetting : IWifiSetting
    {
        protected readonly string WifiLoginKey = "WifiLogin";
        protected readonly string WifiPasswordKey = "WifiPassword";
        protected readonly string WifiAuthenticationTypeKey = "WifiAuthenticationType";
        protected readonly string WifiIsHiddenKey = "WifiIsHidden";

        public abstract string WifiLogin { get; set; }
        public abstract string WifiPassword { get; set; }
        public abstract int WifiAuthenticationType { get; set; }
        public abstract bool WifiIsHidden { get; set; }
    }
}
