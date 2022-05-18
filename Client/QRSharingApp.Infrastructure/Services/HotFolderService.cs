using QRSharingApp.ClientApi.Interfaces;
using QRSharingApp.Infrastructure.Enums;
using QRSharingApp.Infrastructure.EventArgs;
using QRSharingApp.Infrastructure.Models;
using QRSharingApp.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QRSharingApp.Infrastructure.Services
{
    public class HotFolderService : IHotFolderService
    {
        private readonly IHotFolderApi _hotFolderApi;

        public event EventHandler<HotFolderEventArgs> HotFolderUpdateEvent;

        public HotFolderService(IHotFolderApi hotFolderApi)
        {
            _hotFolderApi = hotFolderApi;
        }

        public async Task<bool> AddNew(string folderPath)
        {
            var folderPathAlreadyExist = (await _hotFolderApi.GetByPath(folderPath)) != null;
            if (folderPathAlreadyExist)
            {
                return false;
            }

            var newFolder = await _hotFolderApi.Create(folderPath);

            HotFolderUpdateEvent?.Invoke(this, new HotFolderEventArgs()
            {
                Folder = new HotFolder()
                {
                    Id = newFolder.Id,
                    FolderPath = newFolder.FolderPath,
                    IsAvailable = CanRead(newFolder.FolderPath)
                },
                UpdateKind = HotFolderUpdateKind.Added
            });

            return true;
        }

        public async Task<IEnumerable<HotFolder>> GetAll()
        {
            var dbHotFolders = await _hotFolderApi.GetAll();
            return dbHotFolders.Select(_ => new HotFolder() 
            {
                Id = _.Id,
                FolderPath = _.FolderPath,
                IsAvailable = CanRead(_.FolderPath)
            });
        }

        public async Task Remove(int folderId)
        {
            var removedFolder = await _hotFolderApi.Delete(folderId);

            HotFolderUpdateEvent?.Invoke(this, new HotFolderEventArgs() 
            {
                Folder = new HotFolder()
                {
                    Id = removedFolder.Id,
                    FolderPath = removedFolder.FolderPath,
                    IsAvailable = CanRead(removedFolder.FolderPath)
                },
                UpdateKind = HotFolderUpdateKind.Removed
            });
        }

        public static bool CanRead(string path)
        {
            try
            {
                //TODO

                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                return false;
            }
            catch (DirectoryNotFoundException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}