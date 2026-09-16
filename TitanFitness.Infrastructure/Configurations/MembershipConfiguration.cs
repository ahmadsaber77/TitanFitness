using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Infrastructure.Configurations;

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("Memberships");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MemberId)
            .IsRequired();

        builder.Property(x => x.PlanId)
            .IsRequired();

        builder.Property(x => x.PurchaseDate)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.OwnsOne(x => x.AgreedTerms, agreedTerms =>
        {
            agreedTerms.Property(x => x.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            agreedTerms.Property(x => x.DurationInMonths)
                .IsRequired();

            agreedTerms.Property(x => x.MaxFreezeDays)
                .IsRequired();

            agreedTerms.Property(x => x.MaxNumberOfFreezes)
                .IsRequired();

            agreedTerms.Property(x => x.GuestPassQuota)
                .IsRequired();

            agreedTerms.Property(x => x.AccessScope)
                .IsRequired();
        });


        builder.HasOne<Member>()
    .WithMany()
    .HasForeignKey(x => x.MemberId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Plan>()
            .WithMany()
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
