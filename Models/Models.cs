using System.Runtime.InteropServices.JavaScript;

namespace kolokwium_1.Models;

public class Customer
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public DateTime dateOfBirth { get; set; }
}

public class Driver
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string licenseNumber { get; set; }
}

public class Delivery {
    public DateTime date { get; set; }
    public Customer customer { get; set; }
    public Driver driver { get; set; }
    public List<Product> products { get; set; }
}

public class Product
{
    public string name { get; set; }
    public int amount { get; set; }
}

public class DeliveryRequest
{
    public int deliveryId { get; set; }
    public int customerId { get; set; }
    public string licenseNumber { get; set; }
    public List<Product> products { get; set; }
}