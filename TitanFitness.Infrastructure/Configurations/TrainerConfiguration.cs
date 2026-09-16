using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.ValueObjects;

namespace TitanFitness.Infrastructure.Configurations;

public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.ToTable("Trainers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TrainerNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.TrainerName)
            .IsRequired()
            .HasMaxLength(100);

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


        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.BranchId)
            .IsRequired();

        builder.HasOne<Branch>()
    .WithMany()
    .HasForeignKey(x => x.BranchId)
    .OnDelete(DeleteBehavior.Restrict);
    }

}
