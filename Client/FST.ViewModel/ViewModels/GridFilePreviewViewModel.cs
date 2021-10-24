using FST.ViewModel.Helpers;
using FST.ViewModel.Interfaces;
using FST.ViewModel.Models;
using FST.ViewModel.Services;
using FST.ViewModel.ViewModels.Interfaces;
using FST.Infrastructure.Models;
using FST.Infrastructure.Services.Interfaces;
using Microsoft.WindowsAPICodePack.Shell;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Unity;


namespace FST.ViewModel.ViewModels
{
    public class GridFilePreviewViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<FilePreviewViewModel> _files;
        private ISharedAppDataViewModel _sharedAppDataViewModel;
        private string _backgroundImagePath;
        private int _currentPage;
        private int _columns;
        private int _rows;
        private int _itemsPerPage;
        private ObservableCollection<int> _pages;

        private IList<FilePreviewViewModel> AllFiles = new List<FilePreviewViewModel>();

        private ICommand _onLoad;
        private ICommand _closePreviewCmd;
        private ICommand _openFullPreviewCmd;
        private ICommand _changeFilePageCmd;

        [Dependency]
        public IRegionManager _regionManager;
        [Dependency]
        public IAppSettingService _appSettingService;
        [Dependency]
        public ILocalFilesService _localFilesService;
        [Dependency]
        public IApplicationTaskUtility _applicationTaskUtility;
        [Dependency]
        public ILocalFileCacheService _localFileCacheService;

        public ICommand ClosePreviewCmd
        {
            get
            {
                return _closePreviewCmd ??
                  (_closePreviewCmd = new DelegateCommand(
                      () => {
                          SharedAppDataViewModel.IsPreviewVisible = false;
                      }
                  ));
            }
        }

        public ICommand OnLoadCmd
        {
            get
            {
                return _onLoad ??
                  (_onLoad = new DelegateCommand(
                      async () => {
                          if (FilesOnPage != null)
                          {
                              return;
                          }

                          UpdateGridStructure();

                          _localFilesService.LocalFiles.CollectionChanged += LocalFiles_CollectionChanged;

                          Application.Current.Dispatcher.Invoke(() =>
                          {
                              AllFiles = new ObservableCollection<FilePreviewViewModel>(_localFilesService
                                  .LocalFiles
                                  .OrderBy(_ => _.CreationDate)
                                  .Select(_ => new FilePreviewViewModel(_)));

                              InitPages();

                              if (_appSettingService.SortingDisplayFiles)
                              {
                                  FilesOnPage = new ObservableCollection<FilePreviewViewModel>(AllFiles.Reverse().Take(ItemsPerPage));
                              }
                              else
                              {
                                  FilesOnPage = new ObservableCollection<FilePreviewViewModel>(AllFiles.Take(ItemsPerPage));
                              }
                          });

                          //var importedFiles = FileImportBackgroundService.ProccessigItems.Where(_ => _.Status == ImportStatus.Imported);
                          //foreach (var importedFile in importedFiles)
                          //{
                          //    var file = AllFiles.FirstOrDefault((_ => _.Name == importedFile.Name && _.LocalPath == importedFile.LocalPath));
                          //    if (file == null)
                          //    {
                          //        continue;
                          //    }

                          //    file.Id = importedFile.Id;
                          //    file.WebPath = importedFile.WebPath;
                          //}

                          IEnumerable<FilePreviewViewModel> files = AllFiles;
                          if (_appSettingService.SortingDisplayFiles)
                          {
                              files = AllFiles.Reverse();
                          }

                          foreach (var file in files)
                          {
                              file.IsLoading = true;
                              await FetchThumbnailImage(file);
                              await FetchQRCodeImage(file);
                              file.IsLoading = false;
                          }
                      }
                  ));
            }
        }

        //private void FileImportService_FileImportedEvent(object sender, ImportFileViewModel e)
        //{
        //    var previewFile = AllFiles.FirstOrDefault(_ => _.LocalPath == e.LocalPath && _.Name == e.Name);
        //    if (previewFile == null)
        //    {
        //        return;
        //    }

        //    previewFile.Id = e.Id;
        //    previewFile.WebPath = e.WebPath;

        //    Task.Run(async () => await FetchQRCodeImage(previewFile));
        //}

        public ICommand OpenFullPreviewCmd
        {
            get
            {
                return _openFullPreviewCmd ??
                  (_openFullPreviewCmd = new DelegateCommand<FilePreviewViewModel>(
                      file => {
                          if (file == null)
                          {
                              return;
                          }

                          var param = new NavigationParameters()
                          {
                              { nameof(FilePreviewViewModel), file }
                          };

                          if (file.IsVideo)
                          {
                              _regionManager.RequestNavigate("PreviewContentRegion", "VideoPreviewView", param);
                          }
                          else if (file.IsPhoto)
                          {
                              _regionManager.RequestNavigate("PreviewContentRegion", "PhotoPreviewView", param);
                          }
                      }
                  ));
            }
        }

        public ICommand ChangeFilePageCmd
        {
            get
            {
                return _changeFilePageCmd ??
                  (_changeFilePageCmd = new DelegateCommand<int?>(
                      pageNumber => {
                          CurrentPage = pageNumber.Value;
                          UpdateCurrentPageItems();
                      }
                  ));
            }
        }

        public string BackgroundImagePath
        {
            get { return _backgroundImagePath; }
            set { SetProperty(ref _backgroundImagePath, value); }
        }

        public int Rows
        {
            get { return _rows; }
            set { SetProperty(ref _rows, value); }
        }

        public int Columns
        {
            get { return _columns; }
            set { SetProperty(ref _columns, value); }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set { SetProperty(ref _currentPage, value); }
        }

        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set { SetProperty(ref _itemsPerPage, value); }
        }

        public ObservableCollection<int> Pages
        {
            get { return _pages; }
            set { SetProperty(ref _pages, value); }
        }

        public ObservableCollection<FilePreviewViewModel> FilesOnPage
        {
            get { return _files; }
            set { SetProperty(ref _files, value); }
        }

        public ISharedAppDataViewModel SharedAppDataViewModel
        {
            get { return _sharedAppDataViewModel; }
            set { SetProperty(ref _sharedAppDataViewModel, value); }
        }

        public GridFilePreviewViewModel(ISharedAppDataViewModel sharedAppDataViewModel)
        {
            SharedAppDataViewModel = sharedAppDataViewModel;
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
                    AllFiles.Add(filePreviewViewModel);
                    Task.Run(async () => await FetchThumbnailImage(filePreviewViewModel));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var locaFileToRemove = e.OldItems[0] as LocalFile;
                    var fileToRemove = AllFiles.FirstOrDefault(_ => _.Name == locaFileToRemove.Name && _.LocalPath == locaFileToRemove.Path);
                    if (fileToRemove == null)
                    {
                        return;
                    }
                    AllFiles.Remove(fileToRemove);
                    break;
                default:
                    break;
            }

            InitPages();
            UpdateCurrentPageItems();
        }

        private void InitPages()
        {
            Pages = new ObservableCollection<int>();
            for (int i = 0; i < (AllFiles.Count + ItemsPerPage - 1) / ItemsPerPage; i++)
            {
                Application.Current.Dispatcher.Invoke(() => Pages.Add(i));
            }
        }

        //public async Task<string> GetSharedLink(FilePreviewViewModel file, CloudApi client)
        //{
        //    var sharedLink = await client.GetSharedLink(file.WebPath);

        //    if (string.IsNullOrEmpty(sharedLink))
        //    {
        //        sharedLink = await client.CreateSharedLink(file.WebPath);
        //    }

        //    return sharedLink;
        //}

        private async Task FetchQRCodeImage(FilePreviewViewModel file)
        {
            if (string.IsNullOrEmpty(file.WebPath))
            {
                return;
            }

            //var client = await _cloudService.GetCurrentCloudeApi();
            //if (client == null)
            //{
            //    return;
            //}

            //var sharedLink = await GetSharedLink(file, client);
            //file.SharedLink = sharedLink;

            //var localQRImageName = $"{HashStringHelper.GetHashString(sharedLink)}.jpeg";
            //var localImage = await _localFileCacheService.GetBitmapImage(localQRImageName).ConfigureAwait(false);
            //if (localImage != null)
            //{
            //    file.QRImage = localImage;
            //    return;
            //}

            //var qrCodeImagePath = await _localFileCacheService.SaveFile(fileStream =>
            //{
            //    return Task.Run(() =>
            //    {
            //        var qrGenerator = new QRCodeGenerator();
            //        var qrCodeData = qrGenerator.CreateQrCode(sharedLink, QRCodeGenerator.ECCLevel.Q);
            //        var qrCode = new QRCode(qrCodeData);
            //        var qrCodeImage = qrCode.GetGraphic(5);
            //        qrCodeImage.Save(fileStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            //    });
            //}, localQRImageName, false);

            //file.QRImage = await _localFileCacheService.GetBitmapImage(localQRImageName).ConfigureAwait(false);
        }

        private void UpdateGridStructure()
        {
            if (ItemsPerPage == _appSettingService.ItemsInGrid)
            {
                return;
            }
            ItemsPerPage = _appSettingService.ItemsInGrid;
            Application.Current.Dispatcher.Invoke(() => 
            {
                switch (ItemsPerPage)
                {
                    case 20:
                        Rows = 4;
                        Columns = 5;
                        break;
                    case 16:
                        Rows = 4;
                        Columns = 4;
                        break;
                    case 9:
                        Rows = 3;
                        Columns = 3;
                        break;
                    case 4:
                        Rows = 2;
                        Columns = 2;
                        break;
                    default:
                        Rows = 1;
                        Columns = 1;
                        break;
                }
            });
        }

        private void UpdateCurrentPageItems()
        {
            if ((ItemsPerPage * CurrentPage) >= AllFiles.Count)
            {
                CurrentPage--;
            }
            FilesOnPage.Clear();

            IEnumerable<FilePreviewViewModel> files = AllFiles;
            if (_appSettingService.SortingDisplayFiles)
            {
                files = AllFiles.Reverse();
            }

            FilesOnPage.AddRange(files.Skip(CurrentPage * ItemsPerPage).Take(ItemsPerPage));
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

            BackgroundImagePath = _appSettingService.BackgroundImagePath;

            if (FilesOnPage != null)
            {
                CurrentPage = 0;
                UpdateGridStructure();
                InitPages();
                UpdateCurrentPageItems();
            }
        }
        #endregion

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
