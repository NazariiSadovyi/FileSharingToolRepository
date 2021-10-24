using FST.Infrastructure.EventArgs;
using FST.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FST.Infrastructure.Services.Interfaces
{
    public interface IHotFolderService
    {
        event EventHandler<HotFolderEventArgs> HotFolderUpdateEvent;

        Task<IEnumerable<HotFolder>> GetAll();
        Task<bool> AddNew(string folderPath);
        Task Remove(int folderId);
    }
}
