using Data.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = DatabaseHelper.GetConnectionString();
        
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseSqlite(connectionString)
                .UseLazyLoadingProxies(); // Enable lazy loading
        }
    }
    
    // public DbSet<CustomerEntity> Customers { get; set; } = null!;
    // public DbSet<ProductEntity> Products { get; set; } = null!;
    // public DbSet<StatusTypeEntity> StatusTypes { get; set; } = null!;
    // public DbSet<UserEntity> Users { get; set; } = null!;
    // public DbSet<ProjectEntity> Projects { get; set; } = null!;
}