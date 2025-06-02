using kolokwium_1.Models;
using kolokwium_1.Services;

namespace kolokwium_1.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SomeController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    
    public SomeController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var data = await _databaseService.GetSomethingFromDatabaseAsync();
        
        Console.WriteLine("got data" + data);
        
        return Ok(data);
    }
    
    [HttpPost]
    public IActionResult Create([FromBody] Something request)
    {
        // Can test with postman or with:
        // -d "{}" is json
        // curl -X POST http://localhost:5115/api/some -H "Content-Type: application/json" -d "{ \"id\": 1, \"date\": \"2024-06-01T00:00:00\" }"
        
        return Created("", request);
    }
}