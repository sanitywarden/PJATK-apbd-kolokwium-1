using kolokwium_1.Models;
using kolokwium_1.Services;

namespace kolokwium_1.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DeliveriesController : ControllerBase
{
    private readonly IDatabaseService databaseService;
    
    // Constructor needs to be public for dependency injection
    public DeliveriesController(IDatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        try
        {
            var res = databaseService.GetOrderById(id);
            if (res == null)
                return NotFound();
            return Ok(res);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    // POST to create a resource
    // POST: api/deliveries
    [HttpPost]
    public IActionResult Create([FromBody] DeliveryRequest request)
    {
        databaseService.AddOrderToDatabase(request.deliveryId, request.customerId, request.licenseNumber, request.products);
        return Created();
    }
}