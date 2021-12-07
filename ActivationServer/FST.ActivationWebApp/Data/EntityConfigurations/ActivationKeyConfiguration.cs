using FST.ActivationWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FST.ActivationWebApp.Data.EntityConfigurations
{
    public class ActivationKeyConfiguration : IEntityTypeConfiguration<ActivationKey>
    {
        public void Configure(EntityTypeBuilder<ActivationKey> builder)
        {
            builder.HasKey(_ => _.Id);
            builder
                .HasOne(_ => _.ProgramUser)
                .WithOne(_ => _.ActivationKey)
                .HasForeignKey<ProgramUser>();
        }
    }
}
