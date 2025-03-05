using System.Runtime.InteropServices;

namespace Data.Helpers;

public class DatabaseHelper
{
    public static string GetSQLiteDatabaseConnectionString()
    {
        var databasePath = "SQLite_Database.db";
        try
        {
            databasePath = "../Data/Databases/SQLite_Database.db";
        }
        catch
        {
            // Determine the correct database path based on the OS
            databasePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? @"C:\Projects\DataBase\SQLite_Database.db"
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Projects", "Database", "SQLite_Database.db");
        }
        EnsureDatabaseExists(databasePath);
        return $"Data Source={databasePath}";
    }

    private static void EnsureDatabaseExists(string databasePath)
    {
        // Ensure the directory exists
        string? directoryPath = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Create the database file if it does not exist
        if (!File.Exists(databasePath))
        {
            File.Create(databasePath).Close(); // Close to release the file handle
        }
    }
}