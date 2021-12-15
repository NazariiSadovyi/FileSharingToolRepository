using QRSharingApp.ActivationWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QRSharingApp.ActivationWebApp.Data.EntityConfigurations
{
    public class ProgramUserConfiguration : IEntityTypeConfiguration<ProgramUser>
    {
        public void Configure(EntityTypeBuilder<ProgramUser> builder)
        {
            builder.HasKey(_ => _.Id);
            builder
                .HasOne(_ => _.ActivationKey)
                .WithOne(_ => _.ProgramUser)
                .HasForeignKey<ActivationKey>(_ => _.ProgramUserId);
        }
    }
}
