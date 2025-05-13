using kolokwium_1.Models;
using Microsoft.Data.SqlClient;

namespace kolokwium_1.Services;

public class Database : IDatabaseService
{
    public async void AddOrderToDatabase(int order_id, int customer_id, string license_number, List<Product> products)
    {
        using var connection = new SqlConnection();
        connection.Open();
        var transaction = connection.BeginTransaction();

        try
        {
            using var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "";

            await command.ExecuteNonQueryAsync();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
        }
    }
    
    public Delivery GetOrderById(int order_id)
    {
        // Temporary
        
        var product = new Product
        {
            amount = 3,
            name = "something"
        };

        var list = new List<Product> { product, product };

        var customer = new Customer
        {
            firstName = "Emily",
            lastName = "Clark",
            dateOfBirth = DateTime.Now
        };

        var driver = new Driver
        {
            firstName = "Emily",
            lastName = "Clark",
            licenseNumber = "ABCD"
        };

        var delivery = new Delivery
        {
            date = DateTime.Now,
            customer = customer, 
            driver = driver,     
            products = list     
        };
        
        return delivery;
    }
}

