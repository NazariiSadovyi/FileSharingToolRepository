using DynamicData;
using DynamicData.Binding;
using Microsoft.WindowsAPICodePack.Shell;
using QRSharingApp.ClientApi.Interfaces;
using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.ViewModel.Helpers;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.Services;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels;
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
        public IWebServerService WebServerService;
        [Dependency]
        public IAppSettingService AppSettingService;
        [Dependency]
        public ILocalFilesService LocalFilesService;
        [Dependency]
        public ILocalFileApi LocalFileApi;
        [Dependency]
        public IApplicationTaskUtility ApplicationTaskUtility;
        [Dependency]
        public IQRCodeGeneratorService QRCodeGeneratorService;
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
        public ReadOnlyObservableCollection<FilePreviewBaseViewModel> CurrentPageFiles { get; set; }
        public ReadOnlyObservableCollection<FilePreviewBaseViewModel> AllFiles { get; set; }
        public ObservableCollection<ObservableCollection<int>> GroupedPages { get; set; }
        public bool ShowNewestFilesInTheBeginning { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        #endregion

        public GridFilePreviewViewModel(
            ISharedAppDataViewModel sharedAppDataViewModel,
            ILocalFilesService localFilesService,
            IWebServerService webServerService,
            IAppSettingService appSettingService)
        {
            SharedAppDataViewModel = sharedAppDataViewModel;
            ShowNewestFilesInTheBeginning = appSettingService.SortingDisplayFiles;

            webServerService.NetworkChanged += WebServerService_NetworkChanged;
            _autoSwitchTimer = new Timer(new TimerCallback(AutoPageSwitch));
            
            PageRequestViewModel = new PageRequestViewModel(1, appSettingService.ItemsInGrid, localFilesService.LocalFiles);
            PageRequestViewModel
                .WhenAnyValueChanged()
                .Subscribe(_ => RaisePropertyChanged(nameof(PageRequestViewModel)));

            this.WhenAnyValue(_ => _.PageRequestViewModel.Size)
                .Subscribe(UpdateGridStructure);
        }

        public override Task OnLoadAsync()
        {
            LocalFilesService.LocalFiles
                .ToObservableChangeSet()
                .Filter(_ => _.IsPhoto || _.IsVideo)
                .Transform(_ => _.ToFilePreviewViewModel())
                .Transform(LoadFilePreviewData)
                .OnItemRemoved(OnFileRemoved)
                .Bind(out ReadOnlyObservableCollection<FilePreviewBaseViewModel> allFiles)
                .Sort(this.WhenAnyValue(_ => _.ShowNewestFilesInTheBeginning).Select(_ => GetSortFilesComparer()))
                .Page(this.WhenAnyValue(_ => _.PageRequestViewModel))
                .Bind(out ReadOnlyObservableCollection<FilePreviewBaseViewModel> currentPageFiles)
                .Subscribe();

            CurrentPageFiles = currentPageFiles;
            AllFiles = allFiles;

            Observable.CombineLatest(
                this.WhenAnyValue(_ => _.LocalFilesService.LocalFiles.Count).DistinctUntilChanged(),
                this.WhenAnyValue(_ => _.PageRequestViewModel.Size).DistinctUntilChanged())
                .Subscribe(list =>
                {
                    var itemsCount = list[0];
                    var pageSize = list[1];
                    var pagesCount = itemsCount == itemsCount / pageSize * pageSize
                        ? itemsCount / pageSize
                        : itemsCount / pageSize + 1;
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

        private FilePreviewBaseViewModel LoadFilePreviewData(FilePreviewBaseViewModel filePreview)
        {
            Task.Run(async () =>
            {
                filePreview.IsLoading = true;
                await FetchThumbnailImage(filePreview);
                await FetchQRCodeImage(filePreview);
                filePreview.IsLoading = false;
            });

            return filePreview;
        }

        public void StartAutoSwitchTimer()
        {
            var milliseconds = (int)TimeSpan.FromSeconds(AppSettingService.AutoSwitchSeconds).TotalMilliseconds;
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

        private async Task FetchThumbnailImage(FilePreviewBaseViewModel file, bool checkFileCanRead = true)
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

        private async Task<string> GetOrCreateFileId(FilePreviewBaseViewModel viewModel)
        {
            var localFile = await LocalFileApi.GetFile(viewModel.FullLocalPath);
            if (localFile == null)
            {
                localFile = await LocalFileApi.CreateFile(viewModel.FullLocalPath);
            }

            return localFile.Id;
        }

        private async Task FetchQRCodeImage(FilePreviewBaseViewModel file)
        {
            file.Id = await GetOrCreateFileId(file);
            file.SharedLink = WebServerService.GetFilePath(file.Id);
            if (string.IsNullOrEmpty(file.SharedLink))
            {
                return;
            }
            
            var bitMap = QRCodeGeneratorService.BitmapImage(file.SharedLink);
            file.QRImage = ToBitmapImage(bitMap);
        }

        private void WebServerService_NetworkChanged(object sender, bool isValid)
        {
            Task.Run(async () =>
            {
                foreach (var file in AllFiles)
                {
                    file.QRImage = null;
                }

                if (isValid)
                {
                    foreach (var file in AllFiles)
                    {
                        await FetchQRCodeImage(file);
                    }
                }
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

        private SortExpressionComparer<FilePreviewBaseViewModel> GetSortFilesComparer()
        {
            if (ShowNewestFilesInTheBeginning)
            {
                return SortExpressionComparer<FilePreviewBaseViewModel>.Descending(t => t.CreationDate);
            }

            return SortExpressionComparer<FilePreviewBaseViewModel>.Ascending(t => t.CreationDate);
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

        private void OnFileRemoved(FilePreviewBaseViewModel file)
        {
            Task.Run(async () =>
            {
                await LocalFileApi.DeleteFile(file.FullLocalPath);
            });
        }
    }
}
