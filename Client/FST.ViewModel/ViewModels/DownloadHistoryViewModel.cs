using FST.CultureLocalization;
using FST.Infrastructure.Services.Interfaces;
using FST.ViewModel.Interfaces;
using FST.ViewModel.ViewModels.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using FST.Infrastructure.Models;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace FST.ViewModel.ViewModels
{
    public class DownloadHistoryViewModel : BindableBase, INavigationAware
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

        #region Navigation
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RefreshDownloadHistoryData();
        }
        #endregion

        private void RefreshDownloadHistoryData()
        {
            Task.Run(async () =>
            {
                var result = await ApplicationTaskUtility.ExecuteFetchDataAsync(() =>
                {
                    return DownloadHistoryService.GetAll();
                }, "Fetching download history data...");

                Application.Current.Dispatcher.Invoke(() => 
                {
                    HistoryItems = new ObservableCollection<DownloadHistoryModel>(result);
                });
            });
        }
    }
}
