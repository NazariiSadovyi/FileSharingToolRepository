using QRSharingApp.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.IO;
using System.Reflection;

namespace QRSharingApp.DataAccess
{
    public class ApplicationDBContext : DbContext
    {
        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        public string DbPath { get; private set; }

        public ApplicationDBContext()
        {
            var library = Assembly.GetExecutingAssembly().Location;
            var libraryPath = Path.GetDirectoryName(library);
            DbPath = Path.Combine(libraryPath, "FileSharingTool.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //_logger.Info($"SQLite Data Source='{DbPath}'");
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        public DbSet<DownloadHistory> DownloadHistory { get; set; }
        public DbSet<LocalFile> LocalFile { get; set; }
        public DbSet<HotFolder> HotFolder { get; set; }
        public DbSet<Setting> Setting { get; set; }
    }
}
