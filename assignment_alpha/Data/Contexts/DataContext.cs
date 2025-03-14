// using Data.Entities;
// using Data.Helpers;
// using Microsoft.EntityFrameworkCore;
//
// namespace Data.Contexts;

// public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
// {
//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         if (!optionsBuilder.IsConfigured)
//         {
//             optionsBuilder
//                 .UseSqlite(DatabaseHelper.GetSQLiteDatabaseConnectionString())
//                 .UseLazyLoadingProxies();
//         }
//     }
//
//     public DbSet<ClientEntity> Clients { get; set; } = null!;
// }