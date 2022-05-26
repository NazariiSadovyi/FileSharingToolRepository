using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.Infrastructure.Enums;
using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
        #endregion

        #region Commands
        public ICommand UpdateQRCodeCmd
        {
            get => new DelegateCommand(() =>
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
                SharedAppDataViewModel.WifiQRImage = bitmap;

                CurrentSSID = SSID;
                CurrentWifiAuthenticationType = WifiAuthenticationType;
                CurrentPassword = Password;
                CurrentIsHidden = IsHidden;
            },
            () => {
                if (string.IsNullOrEmpty(SSID))
                {
                    return true;
                }
                return SSID != CurrentSSID || Password != CurrentPassword
                    || WifiAuthenticationType != CurrentWifiAuthenticationType || IsHidden != CurrentIsHidden;
            })
            .ObservesProperty(() => SSID)
            .ObservesProperty(() => Password)
            .ObservesProperty(() => WifiAuthenticationType)
            .ObservesProperty(() => IsHidden)
            .ObservesProperty(() => CurrentSSID)
            .ObservesProperty(() => CurrentPassword)
            .ObservesProperty(() => CurrentWifiAuthenticationType)
            .ObservesProperty(() => CurrentIsHidden);
        }

        public ICommand ClearQRCodeCmd
        {
            get => new DelegateCommand(() =>
            {
                SharedAppDataViewModel.WifiQRImage = null;
                CurrentSSID = string.Empty;
                CurrentPassword = string.Empty;
                CurrentWifiAuthenticationType = WifiAuthenticationType.Nopass;
                SSID = string.Empty;
                Password = string.Empty;
                WifiAuthenticationType = WifiAuthenticationType.Nopass;
            },
            () =>
            {
                return !string.IsNullOrEmpty(CurrentSSID);
            })
            .ObservesProperty(() => CurrentSSID);
        }
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

        public ISharedAppDataViewModel SharedAppDataViewModel { get; set; }

        public ObservableCollection<WifiAuthenticationType> WifiAuthenticationTypes { get; set; }
        #endregion

        public WifiConfigurationViewModel(IAppSettingService appSettingService,
            ISharedAppDataViewModel sharedAppDataViewModel)
        {
            WifiAuthenticationTypes = new ObservableCollection<WifiAuthenticationType>()
            {
                WifiAuthenticationType.Nopass,
                WifiAuthenticationType.WEP,
                WifiAuthenticationType.WPA,
                WifiAuthenticationType.WPA2,
            };

            SharedAppDataViewModel = sharedAppDataViewModel;
            SSID = appSettingService.WifiLogin;
            CurrentSSID = SSID;
            Password = appSettingService.WifiPassword;
            CurrentPassword = Password;
            WifiAuthenticationType = (WifiAuthenticationType)appSettingService.WifiAuthenticationType;
            CurrentWifiAuthenticationType = WifiAuthenticationType;
            IsHidden = appSettingService.WifiIsHidden;
            CurrentIsHidden = IsHidden;

            PropertyChanged += WifiConfigurationViewModel_PropertyChanged;
        }

        private void WifiConfigurationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentSSID):
                    AppSettingService.WifiLogin = CurrentSSID;
                    break;
                case nameof(CurrentWifiAuthenticationType):
                    AppSettingService.WifiAuthenticationType = (int)CurrentWifiAuthenticationType;
                    break;
                case nameof(CurrentPassword):
                    AppSettingService.WifiPassword = CurrentPassword;
                    break;
                case nameof(CurrentIsHidden):
                    AppSettingService.WifiIsHidden = CurrentIsHidden;
                    break;
                default:
                    break;
            }
        }
    }
}
