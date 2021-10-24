using FST.DataAccess.Repositories.Interfaces;
using FST.Infrastructure.Enums;
using FST.Infrastructure.EventArgs;
using FST.Infrastructure.Models;
using FST.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace FST.Infrastructure.Services
{
    public class HotFolderService : IHotFolderService
    {
        private readonly IHotFolderRepository _hotFolderRepository;

        public event EventHandler<HotFolderEventArgs> HotFolderUpdateEvent;

        public HotFolderService(IHotFolderRepository hotFolderRepository)
        {
            _hotFolderRepository = hotFolderRepository;
        }

        public async Task<bool> AddNew(string folderPath)
        {
            var folderPathAlreadyExist = (await _hotFolderRepository.GetByPath(folderPath)) != null;
            if (folderPathAlreadyExist)
            {
                return false;
            }

            var newFolder = await _hotFolderRepository.Add(folderPath);

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
            var dbHotFolders = await _hotFolderRepository.GetAll();
            return dbHotFolders.Select(_ => new HotFolder() 
            {
                Id = _.Id,
                FolderPath = _.FolderPath,
                IsAvailable = CanRead(_.FolderPath)
            });
        }

        public async Task Remove(int folderId)
        {
            var folderToRemove = await _hotFolderRepository.GetById(folderId);

            await _hotFolderRepository.Remove(folderId);

            HotFolderUpdateEvent?.Invoke(this, new HotFolderEventArgs() 
            {
                Folder = new HotFolder()
                {
                    Id = folderToRemove.Id,
                    FolderPath = folderToRemove.FolderPath,
                    IsAvailable = CanRead(folderToRemove.FolderPath)
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