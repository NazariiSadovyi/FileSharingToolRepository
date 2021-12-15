using QRSharingApp.ActivationWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QRSharingApp.ActivationWebApp.Data.EntityConfigurations
{
    public class ActivationKeyConfiguration : IEntityTypeConfiguration<ActivationKey>
    {
        public void Configure(EntityTypeBuilder<ActivationKey> builder)
        {
            builder.HasKey(_ => _.Id);
            builder
                .HasOne(_ => _.ProgramUser)
                .WithOne(_ => _.ActivationKey)
                .HasForeignKey<ProgramUser>(_ => _.ActivationKeyId);
            builder
                .HasOne(_ => _.ProgramTool)
                .WithMany(_ => _.ActivationKeys)
                .HasForeignKey(_ => _.ProgramToolId);
        }
    }
}
