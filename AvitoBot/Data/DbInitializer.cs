using Dapper;
using Microsoft.Data.Sqlite;
namespace AvitoBot.Data;

public class DbInitializer
{
    private const string ConnectionString = "Data Source = weapple.db";

    public static void Initialize()
    {
        using var connection = new SqliteConnection(ConnectionString);
        
        var sql = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Model TEXT NOT NULL,
                Price INTEGER,
                Storage INTEGER,
                IsNew INTEGER, 
                State TEXT,
                CreatedAt TEXT
            );";
        
        connection.Execute(sql);
    }
}