using Microsoft.EntityFrameworkCore;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Branch> Branches { get; set; }
    public DbSet<Studio> Studios { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<ClassSession> ClassSessions { get; set; }
    public DbSet<CheckIn> CheckIns { get; set; }
    public DbSet<Freeze> Freezes { get; set; }
    public DbSet<GuestPass> GuestPasses { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }

}
