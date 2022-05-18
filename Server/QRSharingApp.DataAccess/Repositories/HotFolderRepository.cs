using QRSharingApp.DataAccess.Entities;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRSharingApp.DataAccess.Repositories
{
    public class HotFolderRepository : BaseRepository, IHotFolderRepository
    {
        public HotFolderRepository(ApplicationDBContext context)
            : base(context) { }

        public async Task<HotFolder> Add(string folderPath)
        {
            return await ThreadSafeTaskExecute(async () => 
            {
                Context.HotFolder.Add(new HotFolder()
                {
                    FolderPath = folderPath
                });
                await Context.SaveChangesAsync();

                return await Context.HotFolder.FirstAsync(_ => _.FolderPath == folderPath);
            });
        }

        public async Task<IEnumerable<HotFolder>> GetAll()
        {
            return await ThreadSafeTaskExecute(async () =>
            {
                return await Context.HotFolder.AsQueryable().ToListAsync();
            });
        }

        public async Task<HotFolder> GetById(int folderId)
        {
            return await ThreadSafeTaskExecute(async () =>
            {
                return await Context.HotFolder.FirstOrDefaultAsync(_ => _.Id == folderId);
            });
        }

        public async Task<HotFolder> GetByPath(string folderPath)
        {
            return await ThreadSafeTaskExecute(async () =>
            {
                return await Context.HotFolder.FirstOrDefaultAsync(_ => _.FolderPath == folderPath);
            });
        }

        public async Task Remove(int folderId)
        {
            await ThreadSafeTaskExecute(async () =>
            {
                var folder = await Context.HotFolder.FirstOrDefaultAsync(_ => _.Id == folderId);
                if (folder == null)
                {
                    return;
                }

                Context.HotFolder.Remove(folder);
                await Context.SaveChangesAsync();
            });
        }
    }
}
