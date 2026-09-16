using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Infrastructure.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {

        builder.ToTable("Branches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Address)
           .HasMaxLength(200);

        builder.Property(x => x.OpeningTime)
            .IsRequired();

        builder.Property(x => x.ClosingTime)
            .IsRequired();

       
    }
}
