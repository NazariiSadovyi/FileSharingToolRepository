using Microsoft.Win32;
using NLog;
using System;
using System.Management;

namespace QRSharingApp.Activation
{
    public static class HardwareHelper
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public static string GetExternalIPAddress()
        {
            var externalip = new TimedWebClient { Timeout = 8000 }.DownloadString("http://icanhazip.com");
            return externalip;
        }

        //Номер сборки Windows
        public static string GetUniqueWinId()
        {
            try
            {
                RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                RegistryKey sqlsrvKey = localMachineX64View.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                string uniqueWinId = ((string)sqlsrvKey.GetValue("MachineGuid")).ToUpper();

                return uniqueWinId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetUniqueWinId");
                return null;
            }
        }

        public static string GetCpu()
        {
            using (ManagementObjectSearcher win32Proc = new ManagementObjectSearcher("select * from Win32_Processor"),
                win32CompSys = new ManagementObjectSearcher("select * from Win32_ComputerSystem"),
                win32Memory = new ManagementObjectSearcher("select * from Win32_PhysicalMemory"))
            {
                foreach (ManagementObject obj in win32Proc.Get())
                {
                    string procName = obj["Name"].ToString();
                    return procName;
                }
            }

            return "Can't get CPU info";
        }

        public static string GetMotherBoard_ID()
        {
            try
            {
                string MotherBoardID = string.Empty;
                SelectQuery query = new SelectQuery("Win32_BaseBoard");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

                ManagementObjectCollection.ManagementObjectEnumerator enumerator = searcher.Get().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ManagementObject info = (ManagementObject)enumerator.Current;
                    MotherBoardID = info["SerialNumber"].ToString().Trim();
                }
                return MotherBoardID;
            }
            catch (Exception)
            {
                return "007XYTEM25S";
            }
        }
    }
}
