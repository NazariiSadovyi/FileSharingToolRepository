namespace QRSharingApp.ActivationWebApp.ConfigurationOptions
{
    public class EmailConfigurationOption
    {
        public const string SectionName = "EmailConfiguration";

        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool EnableSSL { get; set; }
    }
}
