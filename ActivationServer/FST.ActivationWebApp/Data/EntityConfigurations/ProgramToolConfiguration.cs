using FST.ActivationWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FST.ActivationWebApp.Data.EntityConfigurations
{
    public class ProgramToolConfiguration : IEntityTypeConfiguration<ProgramTool>
    {
        public void Configure(EntityTypeBuilder<ProgramTool> builder)
        {
            builder.HasKey(_ => _.Id);
            builder
                .HasMany(_ => _.ActivationKeys)
                .WithOne(_ => _.ProgramTool)
                .HasForeignKey(_ => _.ProgramToolId);
        }
    }
}
