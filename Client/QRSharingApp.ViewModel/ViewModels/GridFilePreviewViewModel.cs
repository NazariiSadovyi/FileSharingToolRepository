using DynamicData;
using DynamicData.Binding;
using Microsoft.WindowsAPICodePack.Shell;
using QRSharingApp.ClientApi.Interfaces;
using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.Common.Settings.Interfaces;
using QRSharingApp.Infrastructure.Settings.Interfaces;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.Services;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Unity;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class GridFilePreviewViewModel : BaseNavigationViewModel, IGridFilePreviewViewModel
    {
        #region Private fields
        private readonly Timer _autoSwitchTimer;
        #endregion

        #region Dependencies
        [Dependency]
        public IAppSetting AppSetting;
        [Dependency]
        public IWebServerService WebServerService;
        [Dependency]
        public ILocalFilesService LocalFilesService;
        [Dependency]
        public ILocalFileApi LocalFileApi;
        [Dependency]
        public IApplicationTaskUtility ApplicationTaskUtility;
        [Dependency]
        public IQRCodeGeneratorService QRCodeGeneratorService;
        [Dependency]
        public IUnityContainer UnityContainer;
        [Dependency]
        public IWebSetting WebSetting;
        #endregion

        #region Commands
        public ICommand ClosePreviewCmd => ReactiveCommand.Create(() => 
            {
                StopAutoSwitchTimer();
                SharedAppDataViewModel.IsPreviewVisible = false;
            }
        );
        #endregion

        #region Properties
        public string BackgroundImagePath { get; set; }
        public PageRequestViewModel PageRequestViewModel { get; set; }
        public ISharedAppDataViewModel SharedAppDataViewModel { get; set; }
        public ReadOnlyObservableCollection<ThumbnailViewModel> CurrentPageFiles { get; set; }
        public ReadOnlyObservableCollection<ThumbnailViewModel> AllFiles { get; set; }
        public ObservableCollection<ObservableCollection<int>> GroupedPages { get; set; }
        public bool ShowNewestFilesInTheBeginning { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        #endregion

        public GridFilePreviewViewModel(
            ISharedAppDataViewModel sharedAppDataViewModel,
            ILocalFilesService localFilesService,
            IWebServerService webServerService,
            IAppSetting appSettingService)
        {
            SharedAppDataViewModel = sharedAppDataViewModel;
            ShowNewestFilesInTheBeginning = appSettingService.SortingDisplayFiles;

            sharedAppDataViewModel.NetworkChanged.Subscribe(NetworkChanged);
            _autoSwitchTimer = new Timer(new TimerCallback(AutoPageSwitch));
            
            PageRequestViewModel = new PageRequestViewModel(1, appSettingService.ItemsInGrid, localFilesService.LocalFiles);
            PageRequestViewModel
                .WhenPageOrSizeChanged()
                .Subscribe(_ => RaisePropertyChanged(nameof(PageRequestViewModel)));

            this.WhenAnyValue(_ => _.PageRequestViewModel.Size)
                .Subscribe(UpdateGridStructure);
        }

        private void NetworkChanged(string networkId)
        {
            Task.Run(async () =>
            {
                if (AllFiles == null)
                {
                    return;
                }

                var localFileList = AllFiles.ToList();
                foreach (var file in localFileList)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        file.QRImage = null;
                    });
                }

                if (!string.IsNullOrEmpty(networkId))
                {
                    foreach (var file in localFileList)
                    {
                        await FetchQRCodeImage(file, networkId);
                    }
                }
            });
        }

        public override Task OnLoadAsync()
        {
            BackgroundImagePath = AppSetting.BackgroundImagePath;

            LocalFilesService.LocalFiles
                .ToObservableChangeSet()
                .Filter(_ => _.IsPhoto || _.IsVideo)
                .Transform(ThumbnailViewModel.Compose)
                .Transform(LoadThumbnailData)
                .Bind(out ReadOnlyObservableCollection<ThumbnailViewModel> allFiles)
                .Sort(this.WhenAnyValue(_ => _.ShowNewestFilesInTheBeginning).Select(_ => GetSortFilesComparer()))
                .Page(this.WhenAnyValue(_ => _.PageRequestViewModel))
                .Bind(out ReadOnlyObservableCollection<ThumbnailViewModel> currentPageFiles)
                .Subscribe();

            CurrentPageFiles = currentPageFiles;
            AllFiles = allFiles;

            AllFiles
                .ToObservableChangeSet()
                .OnItemRemoved(OnFileRemoved)
                .Throttle(TimeSpan.FromSeconds(1))
                .OnItemAdded(OnFileAdded)
                .Subscribe();

            Observable.CombineLatest(
                this.WhenAnyValue(_ => _.LocalFilesService.LocalFiles.Count).DistinctUntilChanged(),
                this.WhenAnyValue(_ => _.PageRequestViewModel.Size).DistinctUntilChanged())
                .Subscribe(list =>
                {
                    var pagesCount = PageRequestViewModel.PageCount();
                    var chunkSize = 8;
                    var pages = Enumerable.Range(1, pagesCount);
                    var groupedPages = pages
                        .Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / chunkSize)
                        .Select(x => new ObservableCollection<int>(x.Select(v => v.Value)));
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GroupedPages = new ObservableCollection<ObservableCollection<int>>(groupedPages);
                    });
                });

            return Task.CompletedTask;
        }

        public void UpdateSorting(bool showNewestFilesInTheBeginning)
        {
            var currentPage = PageRequestViewModel.Page;
            var currentSize = PageRequestViewModel.Size;

            PageRequestViewModel.Page = 1;
            PageRequestViewModel.Size = AllFiles.Count + 1;

            ShowNewestFilesInTheBeginning = showNewestFilesInTheBeginning;

            PageRequestViewModel.Size = currentSize;
            PageRequestViewModel.Page = currentPage;
        }

        private ThumbnailViewModel LoadThumbnailData(ThumbnailViewModel filePreview)
        {
            var networkId = WebSetting.NetworkId;
            Task.Run(async () =>
            {
                filePreview.IsLoading = true;
                await FetchThumbnailImage(filePreview);
                await FetchQRCodeImage(filePreview, networkId);
                filePreview.IsLoading = false;
            });

            return filePreview;
        }

        public void StartAutoSwitchTimer()
        {
            var milliseconds = (int)TimeSpan.FromSeconds(AppSetting.AutoSwitchSeconds).TotalMilliseconds;
            if (milliseconds == 0)
                return;

            _autoSwitchTimer.Change(milliseconds, milliseconds);
        }

        public void StopAutoSwitchTimer()
        {
            _autoSwitchTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void AutoPageSwitch(object obj)
        {
            if (!SharedAppDataViewModel.IsPreviewVisible)
            {
                return;
            }

            PageRequestViewModel.NextPage();
        }

        private async Task FetchThumbnailImage(ThumbnailViewModel file, bool checkFileCanRead = true)
        {
            file.IsLoading = true;
            if (checkFileCanRead)
            {
                await WaitUntilFileIsReadable(file.FullLocalPath);
            }

            await Task.Run(() =>
            {
                var shellFile = ShellFile.FromFilePath(file.FullLocalPath);
                Application.Current.Dispatcher.Invoke(() =>
                    file.Thumbnail = shellFile.Thumbnail.LargeBitmapSource);
            });
            file.IsLoading = false;
        }

        private async Task<string> GetOrCreateFileId(ThumbnailViewModel viewModel)
        {
            var localFile = await LocalFileApi.GetFile(viewModel.FullLocalPath);
            if (localFile == null)
            {
                localFile = await LocalFileApi.CreateFile(viewModel.FullLocalPath);
            }

            return localFile.Id;
        }

        private async Task FetchQRCodeImage(ThumbnailViewModel file, string networkId)
        {
            file.Id = await GetOrCreateFileId(file);
            if (string.IsNullOrEmpty(networkId))
            {
                return;
            }

            file.SharedLink = WebServerService.GetFilePath(file.Id, networkId);
            if (string.IsNullOrEmpty(file.SharedLink))
            {
                return;
            }
            
            var bitMap = QRCodeGeneratorService.BitmapImage(file.SharedLink);
            Application.Current.Dispatcher.Invoke(() =>
            {
                file.QRImage = ToBitmapImage(bitMap);
            });
        }

        private BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private SortExpressionComparer<ThumbnailViewModel> GetSortFilesComparer()
        {
            if (ShowNewestFilesInTheBeginning)
            {
                return SortExpressionComparer<ThumbnailViewModel>.Descending(t => t.CreationDate);
            }

            return SortExpressionComparer<ThumbnailViewModel>.Ascending(t => t.CreationDate);
        }

        private void UpdateGridStructure(int size)
        {
            switch (size)
            {
                case 20: Rows = 4; Columns = 5; break;
                case 12: Rows = 3; Columns = 4; break;
                case 9: Rows = 3; Columns = 3; break;
                case 4: Rows = 2; Columns = 2; break;
                default: Rows = 1; Columns = 1; break;
            }
        }

        private async Task WaitUntilFileIsReadable(string fullLocalPath)
        {
            try
            {
                using (File.OpenRead(fullLocalPath))
                { }
            }
            catch (IOException)
            {
                await Task.Delay(200);
                await WaitUntilFileIsReadable(fullLocalPath);
            }
        }

        private void OnFileRemoved(ThumbnailViewModel file)
        {
            Task.Run(async () =>
            {
                await LocalFileApi.DeleteFile(file.FullLocalPath);
            });
        }

        private void OnFileAdded(ThumbnailViewModel file)
        {
            if (!SharedAppDataViewModel.IsFilePreviewOpened)
            {
                return;
            }

            if (PageRequestViewModel.Size != 1)
            {
                return;
            }

            Task.Run(async () =>
            {
                var mainWindowViewModel = UnityContainer.Resolve<IMainWindowViewModel>();
                await mainWindowViewModel.OpenFilePreviewAsync(file);
            });
        }
    }
}
