using FST.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.IO;
using System.Reflection;

namespace FST.DataAccess
{
    public class ApplicationDBContext : DbContext
    {
        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        public string DbPath { get; private set; }

        public ApplicationDBContext()
        {
            var library = Assembly.GetExecutingAssembly().Location;
            var libraryPath = Path.GetDirectoryName(library);
#if (!DEBUG)
            libraryPath = BuildReleaseDbPath(libraryPath);
#else
            libraryPath = BuildDebugDbPath(libraryPath);
#endif
            DbPath = Path.Combine(libraryPath, "FileSharingTool.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _logger.Info($"SQLite Data Source='{DbPath}'");
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        public DbSet<LocalFile> LocalFile { get; set; }
        public DbSet<HotFolder> HotFolder { get; set; }
        public DbSet<Setting> Setting { get; set; }

        private string BuildReleaseDbPath(string libraryPath)
        {
            if (libraryPath.EndsWith("Client"))
            {
                return ReplaceLastOccurrence(libraryPath, "Client", "Server");
            }
            return libraryPath;
        }

        private string BuildDebugDbPath(string libraryPath)
        {
            if (libraryPath.EndsWith(@"Server\FST.WebApplication\bin\x64\Debug\net5.0"))
            {
                return ReplaceLastOccurrence(
                    libraryPath,
                    @"Server\FST.WebApplication\bin\x64\Debug\net5.0",
                    @"Client\FST.Client\bin\x64\Debug\net5.0-windows");
            }
            return libraryPath;
        }

        public static string ReplaceLastOccurrence(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find);

            if (place == -1)
                return source;

            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }
    }
}
