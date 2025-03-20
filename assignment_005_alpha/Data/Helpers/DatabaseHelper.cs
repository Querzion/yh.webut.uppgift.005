namespace Data.Helpers;

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