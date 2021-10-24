using FST.DataAccess.Entities;
using FST.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FST.DataAccess.Repositories
{
    public class LocalFileRepository : ILocalFileRepository
    {
        private static object _locker = new object();
        private readonly ApplicationDBContext _context;
        public LocalFileRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<LocalFile> Add(string localFilePath)
        {
            var localfile = new LocalFile()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Path.GetFileName(localFilePath),
                Path = Path.GetDirectoryName(localFilePath)
            };
            _context.LocalFile.Add(localfile);
            await _context.SaveChangesAsync();

            return localfile;
        }

        public async Task<IEnumerable<LocalFile>> GetAll()
        {
            Monitor.Enter(_locker);
            var localFiles = await _context.LocalFile.ToListAsync();
            Monitor.Exit(_locker);

            return localFiles;
        }

        public async Task<LocalFile> GetById(string fileId)
        {
            return await _context.LocalFile.FirstOrDefaultAsync(_ => _.Id == fileId);
        }

        public async Task<LocalFile> GetByFullPath(string fileFullPath)
        {
            var name = Path.GetFileName(fileFullPath);
            var path = Path.GetDirectoryName(fileFullPath);
            return await _context.LocalFile
                .FirstOrDefaultAsync(_ => _.Name == name && _.Path == path);
        }

        public async Task Remove(string fileId)
        {
            var locaFile = await _context.LocalFile.FirstOrDefaultAsync(_ => _.Id == fileId);
            if (locaFile == null)
            {
                return;
            }

            _context.LocalFile.Remove(locaFile);
            await _context.SaveChangesAsync();
        }
    }
}