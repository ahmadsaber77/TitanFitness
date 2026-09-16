using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.ValueObjects;

namespace TitanFitness.Infrastructure.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MembershipNumber)
            .IsRequired()
            .HasMaxLength(10)
            .HasConversion(
                membershipNumber => membershipNumber.Value,
                value => MembershipNumber.Create(value).Value);

        builder.HasIndex(x => x.MembershipNumber)
            .IsUnique();

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.JoinedDate)
            .IsRequired();

        builder.Property(x => x.HomeBranchId)
            .IsRequired();


        builder.Property(x => x.Email)
       .HasConversion(
           email => email == null ? null : email.Value,
           value => value == null ? null : Email.Create(value).Value)
       .HasMaxLength(100);

        builder.Property(x => x.Phone)
            .HasConversion(
                phone => phone == null ? null : phone.Value,
                value => value == null ? null : Phone.Create(value).Value)
            .HasMaxLength(20);

        builder.Property(x => x.Address)
            .HasMaxLength(200);

        builder.Property(x => x.Photo)
            .HasColumnType("varbinary(max)");

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => x.HomeBranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

