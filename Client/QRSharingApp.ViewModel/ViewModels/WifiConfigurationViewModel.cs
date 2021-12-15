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
        #region Private fields
        private ObservableCollection<WifiAuthenticationType> _wifiAuthenticationTypes;
        private ISharedAppDataViewModel _sharedAppDataViewModel;
        private WifiAuthenticationType _wifiAuthenticationType;
        private string _password;
        private string _ssid;
        private bool _isHidden;
        private WifiAuthenticationType _currentWifiAuthenticationType;
        private string _currentPassword;
        private string _currentssid;
        private bool _currentIsHidden;
        #endregion

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
        public string CurrentSSID
        {
            get { return _currentssid; }
            set { SetProperty(ref _currentssid, value); }
        }

        public string CurrentPassword
        {
            get { return _currentPassword; }
            set { SetProperty(ref _currentPassword, value); }
        }

        public bool CurrentIsHidden
        {
            get { return _currentIsHidden; }
            set { SetProperty(ref _currentIsHidden, value); }
        }

        public WifiAuthenticationType CurrentWifiAuthenticationType
        {
            get { return _currentWifiAuthenticationType; }
            set { SetProperty(ref _currentWifiAuthenticationType, value); }
        }

        public string SSID
        {
            get { return _ssid; }
            set { SetProperty(ref _ssid, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public bool IsHidden
        {
            get { return _isHidden; }
            set { SetProperty(ref _isHidden, value); }
        }

        public WifiAuthenticationType WifiAuthenticationType
        {
            get { return _wifiAuthenticationType; }
            set { SetProperty(ref _wifiAuthenticationType, value); }
        }

        public ISharedAppDataViewModel SharedAppDataViewModel
        {
            get { return _sharedAppDataViewModel; }
            set { SetProperty(ref _sharedAppDataViewModel, value); }
        }

        public ObservableCollection<WifiAuthenticationType> WifiAuthenticationTypes
        {
            get { return _wifiAuthenticationTypes; }
            set { SetProperty(ref _wifiAuthenticationTypes, value); }
        }
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
