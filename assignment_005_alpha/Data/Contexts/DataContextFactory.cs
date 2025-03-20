using Data.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data.Contexts;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var connectionString = DatabaseHelper.GetConnectionString();
        
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        
        optionsBuilder.UseSqlite(connectionString)
            .UseLazyLoadingProxies();
        
        return new DataContext(optionsBuilder.Options);
    }
}
