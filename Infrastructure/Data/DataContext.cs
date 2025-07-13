using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

// Явно указываем типы: IdentityUser, IdentityRole и ключ — string
public class DataContext(DbContextOptions<DataContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Rental> Rentals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Важно вызвать базовую реализацию IdentityDbContext
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.IdentityUserId)
            .IsUnique();

        modelBuilder.Entity<Branch>()
            .HasMany(b => b.Cars)
            .WithOne(c => c.Branch)
            .HasForeignKey(c => c.BranchId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Cars)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CarId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customers)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CustomerId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Branchs)
            .WithMany(b => b.Rentals)
            .HasForeignKey(r => r.BranchId);
    }
}