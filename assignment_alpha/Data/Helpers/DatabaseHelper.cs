using Microsoft.Extensions.Configuration;

namespace Data.Helpers;

#region ChatGPT Version that isn't a hardcoded SQLite string

    // public static class DatabaseHelper
    // {
    //     private static IConfiguration? _configuration;
    //
    //     // This method should be called at startup to initialize the configuration
    //     public static void Initialize(IConfiguration configuration)
    //     {
    //         _configuration = configuration;
    //     }
    //
    //     public static string GetConnectionString(string name = "DefaultConnection")
    //     {
    //         if (_configuration == null)
    //         {
    //             throw new InvalidOperationException("DatabaseHelper is not initialized. Call Initialize() first.");
    //         }
    //
    //         return _configuration.GetConnectionString(name) 
    //                ?? throw new InvalidOperationException($"Connection string '{name}' is missing.");
    //     }
    // }

#endregion


public static class DatabaseHelper
{
    private static string SetConnectionString()
    {
        const string connectionString = "DataSource=../Data/Databases/sqlite_database.db";
        return connectionString;
    }
    
    public static string GetConnectionString()
    {
        var connectionString = SetConnectionString();
        return connectionString;
    }
}