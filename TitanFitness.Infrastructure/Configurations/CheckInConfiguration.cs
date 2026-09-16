using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Infrastructure.Configurations;

public class CheckInConfiguration : IEntityTypeConfiguration<CheckIn>
{
    public void Configure(EntityTypeBuilder<CheckIn> builder)
    {
        builder.ToTable("CheckIns");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MemberId)
            .IsRequired();

        builder.Property(x => x.BranchId)
            .IsRequired();

        builder.Property(x => x.CheckInDateTime)
            .IsRequired();

        builder.Property(x => x.CheckOutDateTime);

        builder.Property(x => x.Result)
            .IsRequired();

        builder.Property(x => x.RefusalReason)
            .HasMaxLength(100);

        builder.HasOne<Member>()
    .WithMany()
    .HasForeignKey(x => x.MemberId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

    }


}
