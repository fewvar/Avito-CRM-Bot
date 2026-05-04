using Microsoft.Data.Sqlite;
using Dapper;
using AvitoBot.Models;

namespace AvitoBot.Data;

public class ProductRepository
{
 private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "weapple.db");
 private static readonly string ConnectionString = $"Data Source={DbPath}";

  public void AddProduct(AppleProduct product)
  {
      using var connection = new SqliteConnection(ConnectionString);
     
    const string sql = @"
            INSERT INTO Products (Model, Price, Storage, IsNew, State, CreatedAt)
            VALUES (@Model, @Price, @Storage, @IsNew, @State, @CreatedAt);";
    
    connection.Execute(sql, product);
  }
  
  public List<AppleProduct> GetAllProducts()
  {
      using var connection = new SqliteConnection(ConnectionString);
      const string sql = "SELECT * FROM Products ORDER BY CreatedAt DESC;";
      return connection.Query<AppleProduct>(sql).ToList();
  }
  
  public long GetTotalRevenue(int days)
  {
      using var connection = new SqliteConnection(ConnectionString);
      const string sql = "SELECT SUM(Price) FROM Products WHERE CreatedAt >= date('now', @Days)";
      return connection.ExecuteScalar<long>(sql, new { Days = $"-{days} days" });
  }
  
  public void ClearAll()
  {
      using var connection = new SqliteConnection(ConnectionString);
      const string sql = "DELETE FROM Products;";
      connection.Execute(sql);
  }

}