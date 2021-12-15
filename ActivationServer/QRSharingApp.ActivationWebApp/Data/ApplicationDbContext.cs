using QRSharingApp.ActivationWebApp.Data.Entities;
using QRSharingApp.ActivationWebApp.Data.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace QRSharingApp.ActivationWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ActivationKey> ActivationKey { get; set; }
        public DbSet<ProgramUser> ProgramUser { get; set; }
        public DbSet<ProgramTool> ProgramTool { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ActivationKeyConfiguration());
            builder.ApplyConfiguration(new ProgramUserConfiguration());
            builder.ApplyConfiguration(new ProgramToolConfiguration());
        }
    }
}
