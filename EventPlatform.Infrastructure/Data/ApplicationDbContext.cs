using EventPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPlatform.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity properties and relationships here if needed
        modelBuilder.Entity<User>()
            .HasKey(x => x.UserId);
    }
}