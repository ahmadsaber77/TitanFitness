using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Infrastructure.Configurations;

public class GuestPassConfiguration : IEntityTypeConfiguration<GuestPass>
{
    public void Configure(EntityTypeBuilder<GuestPass> builder)
    {
        builder.ToTable("GuestPasses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MembershipId)
            .IsRequired();

        builder.Property(x => x.IssuedOn)
            .IsRequired();

        builder.Property(x => x.UsedOn);

        builder.Property(x => x.GuestName)
            .HasMaxLength(100);

        builder.HasOne<Membership>()
    .WithMany(x => x.GuestPasses)
    .HasForeignKey(x => x.MembershipId)
    .OnDelete(DeleteBehavior.Cascade);

    }
}
