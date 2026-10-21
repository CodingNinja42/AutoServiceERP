
using CustomerServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure.Data;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(
        DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.Email)
                .HasMaxLength(256)
                .IsRequired();

            entity.HasIndex(c => c.Email)
                .IsUnique();

            entity.Property(c => c.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(c => c.Address)
                .HasMaxLength(300);

            entity.Property(c => c.City)
                .HasMaxLength(100);

            entity.Property(c => c.State)
                .HasMaxLength(100);

            entity.Property(c => c.PostalCode)
                .HasMaxLength(20);
        });
    }
}
