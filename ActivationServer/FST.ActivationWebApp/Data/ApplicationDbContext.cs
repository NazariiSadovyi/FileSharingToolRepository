using FST.ActivationWebApp.Data.Entities;
using FST.ActivationWebApp.Data.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FST.ActivationWebApp.Data
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
