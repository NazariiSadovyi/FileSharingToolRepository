using FST.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;

namespace FST.DataAccess
{
    public class ApplicationDBContext : DbContext
    {
        public string DbPath { get; private set; }

        public ApplicationDBContext()
        {
            var library = Assembly.GetExecutingAssembly().Location;
            var libraryPath = Path.GetDirectoryName(library);
#if (!DEBUG)
            libraryPath = BuildReleaseDbPath(libraryPath);
#endif
            DbPath = Path.Combine(libraryPath, "FileSharingTool.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        public DbSet<LocalFile> LocalFile { get; set; }
        public DbSet<HotFolder> HotFolder { get; set; }
        public DbSet<Setting> Setting { get; set; }

        private string BuildReleaseDbPath(string libraryPath)
        {
            var result = libraryPath;
            result = ReplaceLastOccurrence(result, "Client", "Common");
            result = ReplaceLastOccurrence(result, "Server", "Common");
            return result;
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
