using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.Infrastructure.Models;
using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.ViewModel.Helpers;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.Services;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.FilePreviewVIewModels;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using Microsoft.WindowsAPICodePack.Shell;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Unity;
using QRSharingApp.ClientApi.Interfaces;

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
        public ICommand ClosePreviewCmd
        {
            get => new DelegateCommand(() => 
            {
                SharedAppDataViewModel.IsPreviewVisible = false;
            });
        }

        public ICommand ChangeFilePageCmd
        {
            get => new DelegateCommand<int?>(pageNumber =>
            {
                CurrentPage = pageNumber.Value;
            });
        }

        public ICommand SwitchFilePageCmd
        {
            get => new DelegateCommand<string>(action =>
            {
                switch (action)
                {
                    case "left":
                        if (CurrentPage - 1 < 0)
                        {
                            CurrentPage = Files.Count - 1;
                            return;
                        }

                        CurrentPage--;
                        break;
                    case "right":
                        if (CurrentPage + 1 >= Files.Count)
                        {
                            CurrentPage = 0;
                            return;
                        }

                        CurrentPage++;
                        break;
                    default:
                        break;
                }
            });
        }
        #endregion

        #region Properties
        public string BackgroundImagePath { get; set; }
        public int CurrentPage { get; set; }
        public ISharedAppDataViewModel SharedAppDataViewModel { get; set; }
        public ObservableCollection<FilePreviewBaseViewModel> Files { get; set; }
        #endregion

        public GridFilePreviewViewModel(ISharedAppDataViewModel sharedAppDataViewModel,
            ILocalFilesService localFilesService,
            IWebServerService webServerService)
        {
            SharedAppDataViewModel = sharedAppDataViewModel;
            Files = new ObservableCollection<FilePreviewBaseViewModel>();
            localFilesService.LocalFiles.CollectionChanged += LocalFiles_CollectionChanged;
            webServerService.NetworkChanged += WebServerService_NetworkChanged;
            _autoSwitchTimer = new Timer(new TimerCallback(AutoPageSwitch));
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.TryGetValue<bool>("FromPreview", out _))
            {
                return;
            }

            BackgroundImagePath = AppSettingService.BackgroundImagePath;
        }

        public async Task LoadDataAsync()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Files = new ObservableCollection<FilePreviewBaseViewModel>(LocalFilesService
                    .LocalFiles
                    .OrderBy(_ => _.CreationDate)
                    .Where(_ => _.IsPhoto || _.IsVideo)
                    .Select(_ => _.ToFilePreviewViewModel()));
            });

            IEnumerable<FilePreviewBaseViewModel> files = Files;
            if (AppSettingService.SortingDisplayFiles)
            {
                files = Files.Reverse();
            }

            foreach (var file in files)
            {
                file.IsLoading = true;
                await FetchThumbnailImage(file);
                await FetchQRCodeImage(file);
                file.IsLoading = false;
            }
        }

        public void StartAutoSwitchTimer()
        {
            var milliseconds = (int)TimeSpan.FromSeconds(AppSettingService.AutoSwitchSeconds).TotalMilliseconds;
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

            if (CurrentPage + 1 == Files.Count)
            {
                CurrentPage = 0;
                return;
            }

            CurrentPage++;
        }

        private async Task FetchThumbnailImage(FilePreviewBaseViewModel file)
        {
            var photoPreview = file as PhotoFilePreviewViewModel;
            if (photoPreview == null)
            {
                return;
            }

            photoPreview.IsLoading = true;
            await Task.Run(() => 
            {
                var shellFile = ShellFile.FromFilePath(photoPreview.FullLocalPath);
                Application.Current.Dispatcher.Invoke(() =>
                    photoPreview.Image = shellFile.Thumbnail.ExtraLargeBitmapSource);
            });
            photoPreview.IsLoading = false;
        }

        private void LocalFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newLocalFile = e.NewItems[0] as LocalFile;
                    var filePreviewViewModel = newLocalFile.ToFilePreviewViewModel();
                    Files.Add(filePreviewViewModel);
                    Task.Run(async () => await FetchThumbnailImage(filePreviewViewModel));
                    Task.Run(async () => await FetchQRCodeImage(filePreviewViewModel));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var locaFileToRemove = e.OldItems[0] as LocalFile;
                    var fileToRemove = Files.FirstOrDefault(_ => _.Name == locaFileToRemove.Name && _.LocalPath == locaFileToRemove.Path);
                    if (fileToRemove == null)
                    {
                        return;
                    }
                    Files.Remove(fileToRemove);
                    break;
                default:
                    break;
            }
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
                foreach (var file in Files)
                {
                    file.QRImage = null;
                }

                if (isValid)
                {
                    foreach (var file in Files)
                    {
                        await FetchQRCodeImage(file);
                    }
                }
            });
        }

        public BitmapImage ToBitmapImage(Bitmap bitmap)
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
    }
}
