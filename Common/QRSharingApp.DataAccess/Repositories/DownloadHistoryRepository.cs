using QRSharingApp.DataAccess.Entities;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRSharingApp.DataAccess.Repositories
{
    public class DownloadHistoryRepository : BaseRepository, IDownloadHistoryRepository
    {
        public DownloadHistoryRepository(ApplicationDBContext context)
            : base(context) { }

        public async Task<IEnumerable<DownloadHistory>> GetAll()
        {
            return await ThreadSafeTaskExecute(async () =>
            {
                await Context.DownloadHistory.LoadAsync();
                return await Context.DownloadHistory.AsQueryable().ToListAsync();
            });
        }

        public async Task Add(DownloadHistory downloadHistory)
        {
            await ThreadSafeTaskExecute(async () =>
            {
                Context.DownloadHistory.Add(downloadHistory);
                await Context.SaveChangesAsync();
            });
        }

        public async Task ClearAsync()
        {
            await Context.Database.ExecuteSqlRawAsync($"delete from [{nameof(ApplicationDBContext.DownloadHistory)}]");
        }
    }
}
