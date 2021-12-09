using FST.Infrastructure.Models;
using FST.Infrastructure.Services.Interfaces;
using FST.ViewModel.Interfaces;
using FST.ViewModel.ViewModels.Base;
using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace FST.ViewModel.ViewModels
{
    public class DownloadHistoryViewModel : BaseNavigationViewModel
    {
        #region Private fields
        private ObservableCollection<DownloadHistoryModel> _historyItems;
        #endregion

        #region Dependencies
        [Dependency]
        public IDataExportService DataExportService;
        [Dependency]
        public IApplicationTaskUtility ApplicationTaskUtility;
        [Dependency]
        public IDownloadHistoryService DownloadHistoryService;
        #endregion

        #region Properties
        public ObservableCollection<DownloadHistoryModel> HistoryItems
        {
            get { return _historyItems; }
            set { SetProperty(ref _historyItems, value); }
        }
        #endregion

        #region Commands
        public ICommand ExportDownloadHistoryCmd
        {
            get => new DelegateCommand(
                async () => {
                    await DataExportService.Export(HistoryItems);
                },
                () => {
                    return HistoryItems.Any();
                }
            )
            .ObservesProperty(() => HistoryItems.Count);
        }

        public ICommand ClearDownloadHistoryCmd
        {
            get => new DelegateCommand(
                async () => {
                    await DownloadHistoryService.ClearAsync();
                    HistoryItems.Clear();
                },
                () => {
                    return HistoryItems.Any();
                }
            )
            .ObservesProperty(() => HistoryItems.Count);
        }

        public ICommand RefreshDownloadHistoryCmd
        {
            get => new DelegateCommand(
                () => {
                    RefreshDownloadHistoryData();
                }
            );
        }
        #endregion

        public DownloadHistoryViewModel()
        {
            HistoryItems = new ObservableCollection<DownloadHistoryModel>();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            RefreshDownloadHistoryData();
        }

        private void RefreshDownloadHistoryData()
        {
            Task.Run(async () =>
            {
                var result = await ApplicationTaskUtility.ExecuteFetchDataAsync(() =>
                {
                    return DownloadHistoryService.GetAll();
                }, CultureLocalization.Localization.GetResource("FetchingDownloadHistoryData"));

                Application.Current.Dispatcher.Invoke(() => 
                {
                    HistoryItems = new ObservableCollection<DownloadHistoryModel>(result);
                });
            });
        }
    }
}
