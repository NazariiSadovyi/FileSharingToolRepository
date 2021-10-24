using FST.DataAccess.Entities;
using FST.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FST.DataAccess.Repositories
{
    public class HotFolderRepository : IHotFolderRepository
    {
        private static object _locker = new object();
        private readonly ApplicationDBContext _context;
        public HotFolderRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<HotFolder> Add(string folderPath)
        {
            _context.HotFolder.Add(new HotFolder()
            {
                FolderPath = folderPath
            });
            await _context.SaveChangesAsync();

            return await _context.HotFolder.FirstAsync(_ => _.FolderPath == folderPath);
        }

        public async Task<IEnumerable<HotFolder>> GetAll()
        {
            Monitor.Enter(_locker);
            var hotFolders = await _context.HotFolder.AsQueryable().ToListAsync();
            Monitor.Exit(_locker);

            return hotFolders;
        }

        public async Task<HotFolder> GetById(int folderId)
        {
            return await _context.HotFolder.FirstOrDefaultAsync(_ => _.Id == folderId);
        }

        public async Task<HotFolder> GetByPath(string folderPath)
        {
            return await _context.HotFolder.FirstOrDefaultAsync(_ => _.FolderPath == folderPath);
        }

        public async Task Remove(int folderId)
        {
            var folder = await _context.HotFolder.FirstOrDefaultAsync(_ => _.Id == folderId);
            if (folder == null)
            {
                return;
            }

            _context.HotFolder.Remove(folder);
            await _context.SaveChangesAsync();
        }
    }
}
