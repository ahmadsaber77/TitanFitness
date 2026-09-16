using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Infrastructure.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.DurationInMonths)
            .IsRequired();

        builder.Property(x => x.MaxFreezeDays)
            .IsRequired();

        builder.Property(x => x.MaxNumberOfFreezes)
            .IsRequired();

        builder.Property(x => x.GuestPassQuota)
            .IsRequired();

        builder.Property(x => x.AccessScope)
            .IsRequired();

        builder.Property(x => x.IsPublished)
            .IsRequired();
    }
        

}