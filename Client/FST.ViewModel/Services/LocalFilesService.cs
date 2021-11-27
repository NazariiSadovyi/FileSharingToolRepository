using FST.ViewModel.Helpers;
using FST.ViewModel.ViewModels.Interfaces;
using FST.Infrastructure.Enums;
using FST.Infrastructure.EventArgs;
using FST.Infrastructure.Models;
using FST.Infrastructure.Services.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace FST.ViewModel.Services
{
    public interface ILocalFilesService
    {
        ObservableCollection<LocalFile> LocalFiles { get; }

        Task InitCurrentFiles();
    }

    public class LocalFilesService : ILocalFilesService
    {
        private readonly IDictionary<int, FileSystemWatcher> HotFolderWatchers;

        [Dependency]
        public IHotFolderService _hotFolderService;
        [Dependency]
        public ISharedAppDataViewModel _sharedAppData;

        public ObservableCollection<LocalFile> LocalFiles { get; set; }

        public LocalFilesService(IHotFolderService hotFolderService)
        {
            HotFolderWatchers = new Dictionary<int, FileSystemWatcher>();
            LocalFiles = new ObservableCollection<LocalFile>();

            hotFolderService.HotFolderUpdateEvent += HotFolderServiceHotFolderUpdateEvent;
        }

        public async Task InitCurrentFiles()
        {
            var hotFolders = (await _hotFolderService.GetAll()).Where(_ => _.IsAvailable);
            foreach (var hotFolder in hotFolders)
            {
                await NewHotFolderAddedAsync(hotFolder);
            }
        }

        private async Task AddFilesFromDirectory(HotFolder hotFolder)
        {
            var filesPathes = await Task.Run(() => Directory.GetFiles(hotFolder.FolderPath));
            var files = filesPathes.Select(_ => Path.GetFileName(_));
            foreach (var fileName in files)
            {
                var localFilePath = Path.Combine(hotFolder.FolderPath, fileName);
                if (!IsAllowedByActivation(localFilePath))
                {
                    continue;
                }

                var localFile = new LocalFile()
                {
                    Path = hotFolder.FolderPath,
                    Name = fileName,
                    IsPhoto = LocalFileHelper.IsPhoto(fileName),
                    IsVideo = LocalFileHelper.IsVideo(fileName),
                    CreationDate = File.GetCreationTime(localFilePath)
                };

                if (localFile.IsPhoto || localFile.IsVideo)
                {
                    Application.Current.Dispatcher.Invoke(() => LocalFiles.Add(localFile));
                }
            }
        }

        public async Task StopWatching()
        {
            foreach (var hotFolderWatcher in HotFolderWatchers)
            {
                hotFolderWatcher.Value.EnableRaisingEvents = false;
            }
            HotFolderWatchers.Clear();
        }

        private void HotFolderServiceHotFolderUpdateEvent(object sender, HotFolderEventArgs e)
        {
            switch (e.UpdateKind)
            {
                case HotFolderUpdateKind.Added:
                    NewHotFolderAdded(e.Folder);
                    break;
                case HotFolderUpdateKind.Removed:
                    var hotFolderWatcher = HotFolderWatchers[e.Folder.Id];
                    hotFolderWatcher.EnableRaisingEvents = false;
                    HotFolderWatchers.Remove(e.Folder.Id);
                    var filesToRemove = LocalFiles.Where(_ => _.Path == e.Folder.FolderPath).ToList();
                    foreach (var fileToRemove in filesToRemove)
                    {
                        LocalFiles.Remove(fileToRemove);
                    }
                    break;
                default:
                    break;
            }
        }

        private void NewHotFolderAdded(HotFolder hotFolder)
        {
            var watcher = new FileSystemWatcher
            {
                Path = hotFolder.FolderPath
            };
            watcher.Created += new FileSystemEventHandler(OnFileAdded);
            watcher.Deleted += new FileSystemEventHandler(OnFileDeleted);
            watcher.EnableRaisingEvents = true;
            HotFolderWatchers.Add(new KeyValuePair<int, FileSystemWatcher>(hotFolder.Id, watcher));

            Task.Run(async () =>
            {
                await AddFilesFromDirectory(hotFolder);
            });
        }

        private async Task NewHotFolderAddedAsync(HotFolder hotFolder)
        {
            var watcher = new FileSystemWatcher
            {
                Path = hotFolder.FolderPath
            };
            watcher.Created += new FileSystemEventHandler(OnFileAdded);
            watcher.Deleted += new FileSystemEventHandler(OnFileDeleted);
            watcher.EnableRaisingEvents = true;
            HotFolderWatchers.Add(new KeyValuePair<int, FileSystemWatcher>(hotFolder.Id, watcher));
            
            await AddFilesFromDirectory(hotFolder);
        }

        private void OnFileAdded(object sender, FileSystemEventArgs e)
        {
            if (!IsAllowedByActivation(e.FullPath))
            {
                return;
            }

            var localFile = new LocalFile()
            {
                Path = Path.GetDirectoryName(e.FullPath),
                Name = e.Name,
                IsPhoto = LocalFileHelper.IsPhoto(e.Name),
                IsVideo = LocalFileHelper.IsVideo(e.Name)
            };

            Application.Current.Dispatcher.Invoke(() => LocalFiles.Add(localFile));
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            var removedFile = LocalFiles.FirstOrDefault(_ => _.Name == e.Name 
                                && _.Path == Path.GetDirectoryName(e.FullPath));
            if (removedFile == null)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() => LocalFiles.Remove(removedFile));
        }

        private bool IsAllowedByActivation(string fullFilePath)
        {
            return true;

            if (_sharedAppData.IsActivated)
            {
                return true;
            }

            if (new FileInfo(fullFilePath).Length <= 200000) // 200KB
            {
                return true;
            }

            return false;
        }
    }
}