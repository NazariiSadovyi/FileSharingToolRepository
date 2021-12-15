using QRSharingApp.Infrastructure.EventArgs;
using QRSharingApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRSharingApp.Infrastructure.Services.Interfaces
{
    public interface IHotFolderService
    {
        event EventHandler<HotFolderEventArgs> HotFolderUpdateEvent;

        Task<IEnumerable<HotFolder>> GetAll();
        Task<bool> AddNew(string folderPath);
        Task Remove(int folderId);
    }
}
