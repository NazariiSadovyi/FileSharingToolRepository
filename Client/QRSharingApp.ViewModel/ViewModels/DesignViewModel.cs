using DynamicData;
using DynamicData.Binding;
using QRSharingApp.Common;
using QRSharingApp.Common.Models;
using QRSharingApp.Common.Settings.Interfaces;
using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.Infrastructure.Settings.Interfaces;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.ViewModels.Base;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class DesignViewModel : BaseNavigationViewModel
    {
        #region Private fields
        private WindowState _previousWindowState;
        #endregion

        #region Dependencies
        [Dependency]
        public IWebSetting WebSetting;
        [Dependency]
        public IAppSetting AppSetting;
        [Dependency]
        public IFileExplorerService FileExplorerService;
        [Dependency]
        public IMainWindowViewModel MainWindowViewModel;
        [Dependency]
        public GridFilePreviewViewModel GridFilePreviewViewModel;
        #endregion

        #region Properties
        public string BackgroundImagePath { get; set; }
        public string WebBackgroundImagePath { get; set; }
        public bool SortingDisplayFiles { get; set; }
        public bool DownloadViaForm { get; set; }
        public bool ShowWifiQrCodeInWeb { get; set; }
        public bool ShowGalleryUrlQrCodeInWeb { get; set; }
        public bool ShowAgreedCheckboxOnDownload { get; set; }
        public int AutoSwitchSeconds { get; set; }
        public int ItemsInGrid { get; set; }

        public ObservableCollection<CountryModel> Countries { get; set; }
        public CountryModel SelectedCountry { get; set; }

        public ObservableCollection<ListBoxItemViewModel> FormRequiredFields { get; set; }
        public bool AllFieldsWereRequired { get; set; }
        #endregion

        #region Commands
        public ICommand ChangeItemsInGridCommand => ReactiveCommand.Create<string>(
            itemsInGrid => {
                var itemsInGridInt = int.Parse(itemsInGrid);
                ItemsInGrid = itemsInGridInt;
            }
        );

        public ICommand ChangeLanguageCmd => ReactiveCommand.Create<string>(
            cultureName => {
                var newCultureInfo = new System.Globalization.CultureInfo(cultureName);
                if (CultureLocalization.Localization.Language.Name == newCultureInfo.Name)
                {
                    return;
                }
                CultureLocalization.Localization.Language = new System.Globalization.CultureInfo(cultureName);
                AppSetting.CultureName = cultureName;
            }
        );

        public ICommand SelectGridPreviewBackgroundImageCmd => ReactiveCommand.Create(
            () => {
                var filePath = FileExplorerService.SelectFile("Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png");
                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }

                BackgroundImagePath = filePath;
            }
        );

        public ICommand SelectWebBackgroundImageCmd => ReactiveCommand.Create(
            () => {
                var filePath = FileExplorerService.SelectFile("Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png");
                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }

                WebBackgroundImagePath = filePath;
            }
        );

        public ICommand ChangeWindowModeCmd => ReactiveCommand.Create<string>(
            mode => {
                switch (mode)
                {
                    case "full":
                        if (MainWindowViewModel.WindowStyle == WindowStyle.None)
                        {
                            return;
                        }
                        _previousWindowState = MainWindowViewModel.WindowState;
                        MainWindowViewModel.WindowState = WindowState.Maximized;
                        MainWindowViewModel.WindowStyle = WindowStyle.None;
                        MainWindowViewModel.ResizeMode = ResizeMode.NoResize;
                        break;
                    case "normal":
                        if (MainWindowViewModel.WindowStyle == WindowStyle.SingleBorderWindow)
                        {
                            return;
                        }
                        MainWindowViewModel.WindowState = _previousWindowState;
                        MainWindowViewModel.WindowStyle = WindowStyle.SingleBorderWindow;
                        MainWindowViewModel.ResizeMode = ResizeMode.CanResize;
                        break;
                    default:
                        break;
                }

                Application.Current.MainWindow.Hide();
                Application.Current.MainWindow.Show();
                Application.Current.MainWindow.BringIntoView();
            }
        );
        #endregion

        public DesignViewModel()
        {
        }

        public override Task OnLoadAsync()
        {
            BackgroundImagePath = AppSetting.BackgroundImagePath;
            SortingDisplayFiles = AppSetting.SortingDisplayFiles;
            AutoSwitchSeconds = AppSetting.AutoSwitchSeconds;
            ItemsInGrid = AppSetting.ItemsInGrid;

            DownloadViaForm = WebSetting.DownloadViaForm;
            WebBackgroundImagePath = WebSetting.WebBackgroundImagePath;
            ShowWifiQrCodeInWeb = WebSetting.ShowWifiQrCodeInWeb;
            ShowGalleryUrlQrCodeInWeb = WebSetting.ShowGalleryUrlQrCodeInWeb;
            ShowAgreedCheckboxOnDownload = WebSetting.ShowAgreedCheckboxOnDownload;
            var selectedRequiredFields = WebSetting.RequiredFieldsForDownload;
            FormRequiredFields = new ObservableCollection<ListBoxItemViewModel>
            {
               new ListBoxItemViewModel(1, "Name", selectedRequiredFields.Contains(1)),
               new ListBoxItemViewModel(2, "Email", selectedRequiredFields.Contains(2)),
               new ListBoxItemViewModel(3, "Phone", selectedRequiredFields.Contains(3))
            };
            AllFieldsWereRequired = FormRequiredFields.All(_ => _.IsSelected);
            FormRequiredFields
                .ToObservableChangeSet()
                .WhenAnyPropertyChanged()
                .Subscribe(_ =>
                {
                    WebSetting.RequiredFieldsForDownload = FormRequiredFields
                        .Where(_ => _.IsSelected)
                        .Select(_ => _.Id)
                        .ToArray();
                });

            Countries = new ObservableCollection<CountryModel>(CountriesProvider.GetAll());
            SelectedCountry = Countries.FirstOrDefault(_ => _.Code == WebSetting.DefaultCountryOnDownload);

            PropertyChanged += (e, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(SortingDisplayFiles):
                        AppSetting.SortingDisplayFiles = SortingDisplayFiles;
                        GridFilePreviewViewModel.UpdateSorting(SortingDisplayFiles);
                        break;
                    case nameof(BackgroundImagePath):
                        AppSetting.BackgroundImagePath = BackgroundImagePath;
                        GridFilePreviewViewModel.BackgroundImagePath = BackgroundImagePath;
                        break;
                    case nameof(AutoSwitchSeconds):
                        AppSetting.AutoSwitchSeconds = AutoSwitchSeconds;
                        break;
                    case nameof(ItemsInGrid) when ItemsInGrid != 0:
                        AppSetting.ItemsInGrid = ItemsInGrid;
                        GridFilePreviewViewModel.PageRequestViewModel.Size = ItemsInGrid;
                        break;
                    case nameof(DownloadViaForm):
                        WebSetting.DownloadViaForm = DownloadViaForm;
                        break;
                    case nameof(WebBackgroundImagePath):
                        WebSetting.WebBackgroundImagePath = WebBackgroundImagePath;
                        break;
                    case nameof(ShowWifiQrCodeInWeb):
                        WebSetting.ShowWifiQrCodeInWeb = ShowWifiQrCodeInWeb;
                        break;
                    case nameof(SelectedCountry):
                        WebSetting.DefaultCountryOnDownload = SelectedCountry.Code;
                        break;
                    case nameof(ShowAgreedCheckboxOnDownload):
                        WebSetting.ShowAgreedCheckboxOnDownload = ShowAgreedCheckboxOnDownload;
                        break;
                    case nameof(ShowGalleryUrlQrCodeInWeb):
                        WebSetting.ShowGalleryUrlQrCodeInWeb = ShowGalleryUrlQrCodeInWeb;
                        break;
                    default:
                        break;
                }
            };

            return Task.CompletedTask;
        }
    }
}
