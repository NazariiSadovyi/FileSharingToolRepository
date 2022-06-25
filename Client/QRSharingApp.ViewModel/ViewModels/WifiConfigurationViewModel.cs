using QRSharingApp.Common.Enums;
using QRSharingApp.Common.Models;
using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.Common.Settings.Interfaces;
using QRSharingApp.Infrastructure.Settings.Interfaces;
using QRSharingApp.ViewModel.Helpers;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        public IWifiSetting WifiSetting;
        [Dependency]
        public IAppSetting AppSetting;
        [Dependency]
        public IWebSetting WebSetting;
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

        public ICommand RefreshNetworkListCmd => ReactiveCommand.Create(() =>
            {
                RefreshNetworkInformationCollection();
            }
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

        public string NetworkId { get; set; }
        public ObservableCollection<NetworkInformationModel> NetworkInformations { get; set; }
        #endregion

        public WifiConfigurationViewModel(ISharedAppDataViewModel sharedAppDataViewModel)
        {
            SharedAppDataViewModel = sharedAppDataViewModel;
            NetworkInformations = new ObservableCollection<NetworkInformationModel>();
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
            SSID = WifiSetting.WifiLogin;
            CurrentSSID = SSID;
            Password = WifiSetting.WifiPassword;
            CurrentPassword = Password;
            WifiAuthenticationType = (WifiAuthenticationType)WifiSetting.WifiAuthenticationType;
            CurrentWifiAuthenticationType = WifiAuthenticationType;
            IsHidden = WifiSetting.WifiIsHidden;
            CurrentIsHidden = IsHidden;

            NetworkId = WebSetting.NetworkId;
            RefreshNetworkInformationCollection();

            var networkIdObservable = this.WhenAnyValue(_ => _.NetworkId);
            networkIdObservable.Subscribe(_ => WebSetting.NetworkId = _);

            var networkChangedObservable = Observable.FromEventPattern<EventHandler, EventArgs>(
                handler => WebServerService.NetworkChanged += handler,
                handler => WebServerService.NetworkChanged -= handler)
                .Throttle(TimeSpan.FromMilliseconds(300));
            networkChangedObservable.Subscribe(_ => RefreshNetworkInformationCollection());

            var networkObservable = Observable.Merge(networkIdObservable.IgnoreValue(), networkChangedObservable.IgnoreValue()).Throttle(TimeSpan.FromMilliseconds(300));
            networkObservable.Subscribe(_ => UpdateWebUrlQRImage());
            networkObservable.Subscribe(_ => SharedAppDataViewModel.NetworkChanged.OnNext(NetworkId));

            if (!string.IsNullOrEmpty(SSID))
            {
                SharedAppDataViewModel.WifiQRImage = GenerateWifiBitmapImage();
            }

            return Task.CompletedTask;
        }

        private void RefreshNetworkInformationCollection()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var newNetworkInformations = WebServerService.GetAvailableNetworks();

                var newNetworkInformationsIds = newNetworkInformations.Select(_ => _.Id);
                var itemsToRemove = NetworkInformations.Where(network => !newNetworkInformationsIds.Contains(network.Id)).ToList();

                var currentNetworkInformationsIds = NetworkInformations.Select(_ => _.Id);
                var itemsToAdd = newNetworkInformations.Where(network => !currentNetworkInformationsIds.Contains(network.Id)).ToList();

                foreach (var itemToRemove in itemsToRemove)
                {
                    NetworkInformations.Remove(itemToRemove);
                }

                foreach (var itemToAdd in itemsToAdd)
                {
                    NetworkInformations.Add(itemToAdd);
                }
            });
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

        private void UpdateWebUrlQRImage()
        {
            if (NetworkId == null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SharedAppDataViewModel.WebUrlQRImage = null;
                });
            }

            var webUrl = WebServerService.GetWebUrl(NetworkId);
            if (!string.IsNullOrEmpty(webUrl))
            {
                var image = GenerateWebUrlImage(webUrl);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SharedAppDataViewModel.WebUrlQRImage = image;
                });
            }
        }

        private BitmapImage GenerateWebUrlImage(string webUrl)
        {
            var bitmap = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                QRCodeGeneratorService.SaveToStream(webUrl, stream);
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
            WifiSetting.WifiLogin = CurrentSSID;
            WifiSetting.WifiPassword = CurrentPassword;
            WifiSetting.WifiAuthenticationType = (int)CurrentWifiAuthenticationType;
            WifiSetting.WifiIsHidden = CurrentIsHidden;
        }

        private void ClearWifiConfiguration()
        {
            WifiSetting.WifiLogin = string.Empty;
            WifiSetting.WifiPassword = string.Empty;
            WifiSetting.WifiAuthenticationType = 0;
            WifiSetting.WifiIsHidden = false;
        }
    }
}
