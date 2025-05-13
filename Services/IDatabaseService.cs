using kolokwium_1.Models;

namespace kolokwium_1.Services;

public interface IDatabaseService
{
    public void AddOrderToDatabase(int order_id, int customer_id, string license_number, List<Product> products);
    public Delivery GetOrderById(int order_id);
}

