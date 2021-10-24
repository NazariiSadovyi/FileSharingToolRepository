using FST.DataAccess.Entities;
using System.Data.Entity;

namespace FST.DataAccess
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext() : base("DefaultConnection")
        {
            //Database.SetInitializer<ApplicationDBContext>(null);
        }

        public DbSet<LocalFile> LocalFile { get; set; }
        public DbSet<HotFolder> HotFolder { get; set; }
        public DbSet<Setting> Setting { get; set; }
    }
}
