using NLog;
using System;
using System.IO;
using System.Linq;

namespace QRSharingApp.Activation
{
    internal class LicenseKeyProvider
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _licenseFileName = "licenceSmControl.txt";

        public string Key
        {
            get
            {
                try
                {
                    if (!File.Exists(_licenseFileName))
                    {
                        return string.Empty;
                    }

                    return File.ReadLines(_licenseFileName).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    using (var writetext = new StreamWriter(_licenseFileName))
                    {
                        writetext.WriteLine(value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
        }
    }
}