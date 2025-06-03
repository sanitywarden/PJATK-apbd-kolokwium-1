using kolokwium_1.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
    
    public async Task<int> AddClientRentalInformation(RentalRequest request)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        SqlTransaction transaction = conn.BeginTransaction();
        try
        {
            int clientId;
    
            // sprawdzamy klienta
            await using var checkClientExistsCommand = new SqlCommand();
            checkClientExistsCommand.Connection = conn;
            checkClientExistsCommand.CommandText = "SELECT ID FROM [clients] WHERE FirstName = @FirstName AND LastName = @LastName And Address = @Address;";
            checkClientExistsCommand.Transaction = transaction;
            checkClientExistsCommand.Parameters.AddWithValue("@FirstName", request.Client.FirstName);
            checkClientExistsCommand.Parameters.AddWithValue("@LastName", request.Client.LastName);
            checkClientExistsCommand.Parameters.AddWithValue("@Address", request.Client.Address);
            var checkResult = await checkClientExistsCommand.ExecuteScalarAsync();

            // klient istnieje
            if(checkResult != null) clientId = Convert.ToInt32(checkResult);
            
            // kleint nie istnieje wiec musimy go dodac do bazy danych
            else
            {
                await using var addClientCommand = new SqlCommand();
                addClientCommand.Connection = conn;
                addClientCommand.CommandText = "INSERT INTO [clients] (FirstName, LastName, Address) VALUES (@FirstName, @LastName, @Address); SELECT SCOPE_IDENTITY();";
                addClientCommand.Transaction = transaction;
                addClientCommand.Parameters.AddWithValue("@FirstName", request.Client.FirstName);
                addClientCommand.Parameters.AddWithValue("@LastName", request.Client.LastName);
                addClientCommand.Parameters.AddWithValue("@Address", request.Client.Address);

                clientId = Convert.ToInt32(await addClientCommand.ExecuteScalarAsync());
            }

            // wyliczamy cene za dzien
            int pricePerDay;
            await using var carPriceCommand = new SqlCommand();
            carPriceCommand.Connection = conn;
            carPriceCommand.CommandText = "SELECT PricePerDay FROM cars WHERE ID = @CarId;";
            carPriceCommand.Transaction = transaction;
            carPriceCommand.Parameters.AddWithValue("@CarId", request.CarId);
            var priceResult = await carPriceCommand.ExecuteScalarAsync();
            pricePerDay = Convert.ToInt32(priceResult);

            int totalDays = (request.DateTo - request.DateFrom).Days;
            int totalPrice = totalDays * pricePerDay;
            
            // dodajemy wypozyczenie
            await using var addCarCommand = new SqlCommand();
            addCarCommand.Connection = conn;
            addCarCommand.CommandText = "INSERT INTO car_rentals (ClientID, CarID, DateFrom, DateTo, TotalPrice, Discount) VALUES (@ClientID, @CarID, @DateFrom, @DateTo, @TotalPrice, 0)";
            addCarCommand.Transaction = transaction;
            addCarCommand.Parameters.AddWithValue("@ClientID", clientId);
            addCarCommand.Parameters.AddWithValue("@CarID", request.CarId);
            addCarCommand.Parameters.AddWithValue("@DateFrom", request.DateFrom);
            addCarCommand.Parameters.AddWithValue("@DateTo", request.DateTo);
            addCarCommand.Parameters.AddWithValue("@TotalPrice", totalPrice);
            await addCarCommand.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
            return 201;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await transaction.RollbackAsync();
            return 500;
        }
    }
    
    public async Task<ClientData> GetClientInfoAsync(int id)
    {
        await using var conn = new SqlConnection(_connectionString);
        
        await using var commandClient = new SqlCommand();
        commandClient.Connection = conn;
        commandClient.CommandText = "SELECT ID, FirstName, LastName, Address FROM [clients] WHERE ID = @Id;";
        commandClient.Parameters.AddWithValue("@Id", id);
        
        conn.Open();
        
        var readerClient = await commandClient.ExecuteReaderAsync();
        if (readerClient.Read())
        {
            var client = new ClientData()
            {
                Id        = readerClient.GetInt32(0),
                FirstName = readerClient.GetString(1),
                LastName  = readerClient.GetString(2),
                Address   = readerClient.GetString(3),
                Rentals   = new List<Rental>()
            };
            
            await readerClient.CloseAsync();

            await using var commandRentals = new SqlCommand();
            commandRentals.Connection = conn;
            commandRentals.CommandText =
                @"SELECT ca.VIN AS Vin, co.Name AS Color, m.Name AS Model, 
                cr.DateFrom, cr.DateTo, cr.TotalPrice
                FROM car_rentals cr
                JOIN cars ca ON cr.CarID = ca.ID
                JOIN models m ON ca.ModelID = m.ID
                JOIN colors co ON ca.ColorID = co.ID
                WHERE cr.ClientID = @Id";
            
            commandRentals.Parameters.AddWithValue("@Id", id);
            using (var rentalReader = commandRentals.ExecuteReader())
            {
                while (rentalReader.Read())
                {
                    var rental = new Rental
                    {
                        Vin        = rentalReader.GetString(0),
                        Color      = rentalReader.GetString(1),
                        Model      = rentalReader.GetString(2),
                        DateFrom   = rentalReader.GetDateTime(3),
                        DateTo     = rentalReader.GetDateTime(4),
                        TotalPrice = rentalReader.GetInt32(5)
                    };
                    
                    client.Rentals.Add(rental);
                }
            }
            
            return client;
        }

        return null;
    }
}

