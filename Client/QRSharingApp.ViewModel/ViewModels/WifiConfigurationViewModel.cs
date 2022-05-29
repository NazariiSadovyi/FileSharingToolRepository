using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.Infrastructure.Enums;
using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Unity;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class WifiConfigurationViewModel : BaseNavigationViewModel
    {
        #region Dependencies
        [Dependency]
        public IWifiService WifiService;
        [Dependency]
        public IAppSettingService AppSettingService;
        [Dependency]
        public IQRCodeGeneratorService QRCodeGeneratorService;
        [Dependency]
        public IWebServerService WebServerService;
        #endregion

        #region Commands
        public ICommand UpdateQRCodeCmd => ReactiveCommand.Create(() =>
            {
                SharedAppDataViewModel.WifiQRImage = GenerateWifiBitmapImage();

                CurrentSSID = SSID;
                CurrentWifiAuthenticationType = WifiAuthenticationType;
                CurrentPassword = Password;
                CurrentIsHidden = IsHidden;

                UpdateWifiConfiguration();
            },
            Observable.CombineLatest(
                this.WhenAnyValue(_ => _.SSID),
                this.WhenAnyValue(_ => _.Password),
                this.WhenAnyValue(_ => _.WifiAuthenticationType),
                this.WhenAnyValue(_ => _.IsHidden),
                this.WhenAnyValue(_ => _.CurrentSSID),
                this.WhenAnyValue(_ => _.CurrentPassword),
                this.WhenAnyValue(_ => _.CurrentWifiAuthenticationType),
                this.WhenAnyValue(_ => _.CurrentIsHidden),
                (ssid, password, wifiAuthenticationType, isHidden, currentSSID, currentPassword, currentWifiAuthenticationType, currentIsHidden) =>
                {
                    if (string.IsNullOrEmpty(ssid))
                    {
                        return false;
                    }

                    return ssid != currentSSID || password != currentPassword
                        || wifiAuthenticationType != currentWifiAuthenticationType | isHidden != currentIsHidden;
                })
        );

        public ICommand ClearQRCodeCmd => ReactiveCommand.Create(() =>
            {
                SharedAppDataViewModel.WifiQRImage = null;
                CurrentSSID = string.Empty;
                CurrentPassword = string.Empty;
                CurrentWifiAuthenticationType = WifiAuthenticationType.Nopass;
                SSID = string.Empty;
                Password = string.Empty;
                WifiAuthenticationType = WifiAuthenticationType.Nopass;
                IsHidden = false;

                ClearWifiConfiguration();
            },
            this.WhenAnyValue(x => x.CurrentSSID).Select(_ => !string.IsNullOrEmpty(_))
        );
        #endregion

        #region Properties
        public string CurrentSSID { get; set; }
        public string CurrentPassword { get; set; }
        public bool CurrentIsHidden { get; set; }
        public WifiAuthenticationType CurrentWifiAuthenticationType { get; set; }

        public string SSID { get; set; }
        public string Password { get; set; }
        public bool IsHidden { get; set; }
        public WifiAuthenticationType WifiAuthenticationType { get; set; }

        public ObservableCollection<WifiAuthenticationType> WifiAuthenticationTypes { get; set; }
        public ISharedAppDataViewModel SharedAppDataViewModel { get; set; }
        #endregion

        public WifiConfigurationViewModel(ISharedAppDataViewModel sharedAppDataViewModel)
        {
            SharedAppDataViewModel = sharedAppDataViewModel;
            WifiAuthenticationTypes = new ObservableCollection<WifiAuthenticationType>()
            {
                WifiAuthenticationType.Nopass,
                WifiAuthenticationType.WEP,
                WifiAuthenticationType.WPA,
                WifiAuthenticationType.WPA2,
            };
        }

        public override Task OnLoadAsync()
        {
            SSID = AppSettingService.WifiLogin;
            CurrentSSID = SSID;
            Password = AppSettingService.WifiPassword;
            CurrentPassword = Password;
            WifiAuthenticationType = (WifiAuthenticationType)AppSettingService.WifiAuthenticationType;
            CurrentWifiAuthenticationType = WifiAuthenticationType;
            IsHidden = AppSettingService.WifiIsHidden;
            CurrentIsHidden = IsHidden;

            if (!string.IsNullOrEmpty(SSID))
            {
                SharedAppDataViewModel.WifiQRImage = GenerateWifiBitmapImage();
            }

            if (!string.IsNullOrEmpty(WebServerService.WebUrl))
            {
                SharedAppDataViewModel.WebUrlQRImage = GenerateWebUrlImage();
            }
            WebServerService.NetworkChanged += WebServerService_NetworkChanged;

            return Task.CompletedTask;
        }

        private void WebServerService_NetworkChanged(object sender, bool isAvailable)
        {
            if (isAvailable)
            {
                SharedAppDataViewModel.WebUrlQRImage = GenerateWebUrlImage();
            }
            else
            {
                SharedAppDataViewModel.WebUrlQRImage = null;
            }
        }

        private BitmapImage GenerateWifiBitmapImage()
        {
            var wifiConnectionString = WifiService.GenerateConfigString(SSID, WifiAuthenticationType, Password, IsHidden);
            var bitmap = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                QRCodeGeneratorService.SaveToStream(wifiConnectionString, stream);
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            return bitmap;
        }

        private BitmapImage GenerateWebUrlImage()
        {
            var bitmap = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                QRCodeGeneratorService.SaveToStream(WebServerService.WebUrl, stream);
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            return bitmap;
        }

        private void UpdateWifiConfiguration()
        {
            AppSettingService.WifiLogin = CurrentSSID;
            AppSettingService.WifiPassword = CurrentPassword;
            AppSettingService.WifiAuthenticationType = (int)CurrentWifiAuthenticationType;
            AppSettingService.WifiIsHidden = CurrentIsHidden;
        }

        private void ClearWifiConfiguration()
        {
            AppSettingService.WifiLogin = string.Empty;
            AppSettingService.WifiPassword = string.Empty;
            AppSettingService.WifiAuthenticationType = 0;
            AppSettingService.WifiIsHidden = false;
        }
    }
}
