using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<SearchHistory> SearchHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // User configuration
        builder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
        });

        // SearchHistory configuration
        builder.Entity<SearchHistory>(entity =>
        {
            entity.ToTable("searchhistories");
            entity.HasKey(sh => sh.Id);
            entity.Property(sh => sh.City).IsRequired().HasMaxLength(100);
            entity.Property(sh => sh.Country).HasMaxLength(100);
            entity.Property(sh => sh.WeatherCondition).HasMaxLength(50);
            entity.Property(sh => sh.Description).HasMaxLength(255);
            entity.Property(sh => sh.Period).HasMaxLength(50);

            entity.HasOne(sh => sh.User)
                .WithMany(u => u.SearchHistories)
                .HasForeignKey(sh => sh.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(sh => sh.SearchDate);
            entity.HasIndex(sh => sh.City);
            entity.HasIndex(sh => sh.UserId);
        });
    }
}
