// using Data.Helpers;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
//
// namespace Data.Contexts;

// public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
// {
//     public DataContext CreateDbContext(string[] args)
//     {
//         var connectionString = DatabaseHelper.GetSQLiteDatabaseConnectionString();
//
//         var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
//
//         optionsBuilder.UseSqlite(connectionString)
//             .UseLazyLoadingProxies();
//
//         return new DataContext(optionsBuilder.Options);
//     }
// }