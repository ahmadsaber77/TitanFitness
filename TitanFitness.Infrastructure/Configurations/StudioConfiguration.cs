using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Infrastructure.Configurations;

public class StudioConfiguration : IEntityTypeConfiguration<Studio>
{
    public void Configure(EntityTypeBuilder<Studio> builder)
    {
        builder.ToTable("Studios");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.BranchId)
            .IsRequired();

        builder.Property(x => x.Capacity)
            .IsRequired();

        builder.HasOne<Branch>()
       .WithMany(x => x.Studios)
       .HasForeignKey(x => x.BranchId)
       .OnDelete(DeleteBehavior.Cascade);
    }
}