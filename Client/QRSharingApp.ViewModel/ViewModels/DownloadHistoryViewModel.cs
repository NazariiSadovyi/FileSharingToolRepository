using QRSharingApp.Infrastructure.Models;
using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.ViewModels.Base;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class DownloadHistoryViewModel : BaseNavigationViewModel
    {
        #region Dependencies
        [Dependency]
        public IDataExportService DataExportService;
        [Dependency]
        public IApplicationTaskUtility ApplicationTaskUtility;
        [Dependency]
        public IDownloadHistoryService DownloadHistoryService;
        #endregion

        #region Properties
        public ObservableCollection<DownloadHistoryModel> HistoryItems { get; set; }
        #endregion

        #region Commands
        public ICommand ExportDownloadHistoryCmd => ReactiveCommand.CreateFromTask(
            async () => {
                await DataExportService.Export(HistoryItems);
            },
            HistoryItems.WhenAnyValue(_ => _.Count).Select(_ => _ > 0)
        );

        public ICommand ClearDownloadHistoryCmd => ReactiveCommand.CreateFromTask(
            async () =>
            {
                await DownloadHistoryService.ClearAsync();
                HistoryItems.Clear();
            },
            HistoryItems.WhenAnyValue(_ => _.Count).Select(_ => _ > 0)
        );

        public ICommand RefreshDownloadHistoryCmd => ReactiveCommand.CreateFromTask(
            async () => {
                await RefreshDownloadHistoryData();
            }
        );
        #endregion

        public DownloadHistoryViewModel()
        {
            HistoryItems = new ObservableCollection<DownloadHistoryModel>();
        }

        public override async Task OnLoadAsync()
        {
            var result = await DownloadHistoryService.GetAll();
            Application.Current.Dispatcher.Invoke(() =>
            {
                HistoryItems.Clear();
                foreach (var item in result)
                {
                    HistoryItems.Add(item);
                }
            });
        }

        private async Task RefreshDownloadHistoryData()
        {
            var result = await ApplicationTaskUtility.ExecuteFetchDataAsync(() =>
            {
                return DownloadHistoryService.GetAll();
            }, CultureLocalization.Localization.GetResource("FetchingDownloadHistoryData"));

            Application.Current.Dispatcher.Invoke(() =>
            {
                HistoryItems.Clear();
                foreach (var item in result)
                {
                    HistoryItems.Add(item);
                }
            });
        }
    }
}
