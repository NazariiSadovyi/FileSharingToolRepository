using QRSharingApp.DataAccess.Entities;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace QRSharingApp.DataAccess.Repositories
{
    public class LocalFileRepository : BaseRepository, ILocalFileRepository
    {
        public LocalFileRepository(ApplicationDBContext context)
            : base(context) { }

        public async Task<LocalFile> Add(string localFilePath)
        {
            return await ThreadSafeTaskExecute(async () => 
            {
                var localfile = new LocalFile()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Path.GetFileName(localFilePath),
                    Path = Path.GetDirectoryName(localFilePath),
                    AddedDate = DateTime.Now.ToString()
                };
                Context.LocalFile.Add(localfile);
                await Context.SaveChangesAsync();

                return localfile;
            });
        }

        public async Task<List<LocalFile>> Add(IEnumerable<string> localFilePathes)
        {
            return await ThreadSafeTaskExecute(async () => 
            {
                var localFiles = localFilePathes.Select(_ => new LocalFile
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Path.GetFileName(_),
                    Path = Path.GetDirectoryName(_)
                }).ToList();

                await Context.LocalFile.AddRangeAsync(localFiles);
                await Context.SaveChangesAsync();

                return localFiles;
            });
        }

        public async Task<IEnumerable<LocalFile>> GetAll()
        {
            return await ThreadSafeTaskExecute(async () =>
            {
                return await Context.LocalFile.ToListAsync();
            });
        }

        public async Task<LocalFile> GetById(string fileId)
        {
            return await ThreadSafeTaskExecute(async () =>
            {
                return await Context.LocalFile.FirstOrDefaultAsync(_ => _.Id == fileId);
            });
        }

        public async Task<LocalFile> GetByFullPath(string fileFullPath)
        {
            return await ThreadSafeTaskExecute(async () =>
            {
                var name = Path.GetFileName(fileFullPath);
                var path = Path.GetDirectoryName(fileFullPath);
                return await Context.LocalFile.FirstOrDefaultAsync(_ => _.Name == name && _.Path == path);
            });
        }

        public async Task Remove(string fileId)
        {
            await ThreadSafeTaskExecute(async () =>
            {
                var locaFile = await Context.LocalFile.FirstOrDefaultAsync(_ => _.Id == fileId);
                if (locaFile == null)
                {
                    return;
                }

                Context.LocalFile.Remove(locaFile);
                await Context.SaveChangesAsync();
            });
        }

        public async Task RemoveAll()
        {
            await ThreadSafeTaskExecute(async () =>
            {
                var locaFiles = await Context.LocalFile.ToListAsync();
                Context.LocalFile.RemoveRange(locaFiles);
                await Context.SaveChangesAsync();
            });
        }
    }
}