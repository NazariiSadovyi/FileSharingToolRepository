using FST.Common.Services.Interfaces;
using FST.DataAccess.Repositories.Interfaces;
using FST.Infrastructure.Models;
using FST.Infrastructure.Services.Interfaces;
using FST.ViewModel.Helpers;
using FST.ViewModel.Interfaces;
using FST.ViewModel.Services;
using FST.ViewModel.ViewModels.Interfaces;
using Microsoft.WindowsAPICodePack.Shell;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace FST.ViewModel.ViewModels
{
    public class GridFilePreviewViewModel : BindableBase, IGridFilePreviewViewModel, INavigationAware
    {
        #region Private fields
        private ObservableCollection<FilePreviewViewModel> _files;
        private ISharedAppDataViewModel _sharedAppDataViewModel;
        private string _backgroundImagePath;
        private int _currentPage;
        #endregion

        #region Dependencies
        [Dependency]
        public IWebServerService WebServerService;
        [Dependency]
        public IAppSettingService AppSettingService;
        [Dependency]
        public ILocalFilesService LocalFilesService;
        [Dependency]
        public ILocalFileRepository LocalFileRepository;
        [Dependency]
        public ILocalFileCacheService LocalFileCacheService;
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
        #endregion

        #region Properties
        public string BackgroundImagePath
        {
            get { return _backgroundImagePath; }
            set { SetProperty(ref _backgroundImagePath, value); }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set { SetProperty(ref _currentPage, value); }
        }

        public ISharedAppDataViewModel SharedAppDataViewModel
        {
            get { return _sharedAppDataViewModel; }
            set { SetProperty(ref _sharedAppDataViewModel, value); }
        }

        public ObservableCollection<FilePreviewViewModel> Files
        {
            get { return _files; }
            set { SetProperty(ref _files, value); }
        }
        #endregion

        public GridFilePreviewViewModel(ISharedAppDataViewModel sharedAppDataViewModel,
            ILocalFilesService localFilesService,
            IWebServerService webServerService)
        {
            SharedAppDataViewModel = sharedAppDataViewModel;
            Files = new ObservableCollection<FilePreviewViewModel>();
            localFilesService.LocalFiles.CollectionChanged += LocalFiles_CollectionChanged;
            webServerService.NetworkChanged += WebServerService_NetworkChanged;
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
            if (navigationContext.Parameters.TryGetValue<bool>("FromPreview", out _))
            {
                return;
            }

            BackgroundImagePath = AppSettingService.BackgroundImagePath;
        }
        #endregion

        public async Task LoadDataAsync()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Files = new ObservableCollection<FilePreviewViewModel>(LocalFilesService
                    .LocalFiles
                    .OrderBy(_ => _.CreationDate)
                    .Select(_ => new FilePreviewViewModel(_)));
            });

            IEnumerable<FilePreviewViewModel> files = Files;
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

        private async Task FetchThumbnailImage(FilePreviewViewModel file)
        {
            file.IsLoading = true;
            await Task.Run(() => 
            {
                var shellFile = ShellFile.FromFilePath(file.FullLocalPath);
                Application.Current.Dispatcher.Invoke(() => 
                    file.Image = shellFile.Thumbnail.ExtraLargeBitmapSource);
            });
            file.IsLoading = false;
        }

        private void LocalFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newLocalFile = e.NewItems[0] as LocalFile;
                    var filePreviewViewModel = new FilePreviewViewModel(newLocalFile);
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

        private async Task<string> GetOrCreateFileId(FilePreviewViewModel viewModel)
        {
            var localFile = await LocalFileRepository.GetByFullPath(viewModel.FullLocalPath);
            if (localFile == null)
            {
                localFile = await LocalFileRepository.Add(viewModel.FullLocalPath);
            }

            return localFile.Id;
        }

        private async Task FetchQRCodeImage(FilePreviewViewModel file)
        {
            file.Id = await GetOrCreateFileId(file);
            file.SharedLink = WebServerService.GetFilePath(file.Id);
            if (string.IsNullOrEmpty(file.SharedLink))
            {
                return;
            }
            
            var localQRImageName = $"{HashStringHelper.GetHashString(file.Id)}.jpeg";
            var qrCodeImagePath = await LocalFileCacheService.SaveFile(fileStream =>
            {
                return Task.Run(() =>
                {
                    QRCodeGeneratorService.SaveToStream(file.SharedLink, fileStream);
                });
            }, localQRImageName, false);
            
            file.QRImage = await LocalFileCacheService.GetBitmapImage(localQRImageName).ConfigureAwait(false);
        }

        private void WebServerService_NetworkChanged(object sender, bool e)
        {
            Task.Run(async () => 
            {
                await LocalFileRepository.RemoveAll();
                foreach (var file in Files)
                {
                    file.QRImage = null;
                }

                if (e)
                {
                    foreach (var file in Files)
                    {
                        await FetchQRCodeImage(file);
                    }
                }
            });
        }
    }
}
