using kolokwium_1.Models;
using Microsoft.Data.SqlClient;

namespace kolokwium_1.Services;

public class DatabaseService : IDatabaseService
{
    string _connectionString;
    
    public DatabaseService()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        
        _connectionString = configuration.GetConnectionString("Default");
        Console.WriteLine(_connectionString);
    }
    
    public async void AddSomethingToDatabaseAsync() {
        await using var conn = new SqlConnection(_connectionString);
        var sqlTransaction = conn.BeginTransaction();
        
        try
        {
            await using var command = new SqlCommand();
            command.Connection = conn;

            // Dodaj cos do bazy danych
            command.CommandText = @"
                INSERT INTO [Table] (Column1, Column2) 
                VALUES (@Column1, @Column2);
                SELECT SCOPE_IDENTITY();";
            // SELECT JEST OPCJONALNY

            command.Parameters.AddWithValue("@Column1", 1);
            command.Parameters.AddWithValue("@Column2", 2);

            await conn.OpenAsync();

            // OPCJONALNE
            // Zwraca wartosc SCOPE_IDENTITY() ktora zselektowalismy
            // moze byc nim np numeryczne id
            var result = await command.ExecuteScalarAsync();
            var id = Convert.ToInt32(result);
            
            sqlTransaction.Commit();
        }
        catch (Exception ex)
        {
            sqlTransaction.Rollback();
            Console.WriteLine("caught exception" + ex.Message);
        }
    }
    
    public async void UpdateSomethingInDatabaseAsync() {
        await using var conn = new SqlConnection(_connectionString);
        var sqlTransaction = conn.BeginTransaction();

        try
        {
            await using var command = new SqlCommand();
            command.Connection = conn;

            // Zaaktualizuj cos w bazie danych
            command.CommandText = @"
                UPDATE [Table] SET 
                    Column1 = @Column1,
                    Column2 = @Column2 
                WHERE Id = @Id;";

            command.Parameters.AddWithValue("@Column1", 1);
            command.Parameters.AddWithValue("@Column2", 2);
            command.Parameters.AddWithValue("@Id", 3);

            await conn.OpenAsync();

            int rowsAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine("rows affected" + rowsAffected);
            
            sqlTransaction.Commit();
        }            
        catch (Exception ex)
        {
            sqlTransaction.Rollback();
            Console.WriteLine("caught exception" + ex.Message);
        }
    }
    
    public async Task<List<Trip>> GetSomethingFromDatabaseAsync()
    {
        var list = new List<Trip>();
        
        await using var conn = new SqlConnection(_connectionString);
        await using var command = new SqlCommand();
        command.Connection = conn;
        
        // Zaaktualizuj cos w bazie danych
        command.CommandText = @"
            SELECT * FROM [Client_Trip] 
            WHERE IdClient = @Id;";
        
        command.Parameters.AddWithValue("@Id", 1);
        
        conn.Open();
        
        var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Trip()
            {   
                IdClient = reader.GetInt32(0),
                IdTrip = reader.GetInt32(1),
                RegisteredAt = reader.GetInt32(2),
                PaymentDate = reader.GetInt32(3)
            });
        }
        
        Console.WriteLine("size: " + list.Count);
        return list;
    }

    public async void DeleteSomethingFromDatabaseAsync()
    {
        await using var conn = new SqlConnection(_connectionString);
        var sqlTransaction = conn.BeginTransaction();

        try
        {
            await using var command = new SqlCommand();
            command.Connection = conn;

            // Usun cos w bazie danych
            command.CommandText = @"
                DELETE FROM [Table] 
                WHERE Id = @Id;";

            command.Parameters.AddWithValue("@Id", 1);

            await conn.OpenAsync();

            int rowsAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine("rows affected" + rowsAffected);
            
            sqlTransaction.Commit();
        }
        catch (Exception ex)
        {
            sqlTransaction.Rollback();
            Console.WriteLine("caught exception" + ex.Message);
        }
    }
}

