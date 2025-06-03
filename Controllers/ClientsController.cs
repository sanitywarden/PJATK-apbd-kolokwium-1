using kolokwium_1.Models;
using kolokwium_1.Services;

namespace kolokwium_1.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    
    public ClientsController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        ClientData data = await _databaseService.GetClientInfoAsync(id);
        if (data == null)
            return NotFound();
        return Ok(data);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RentalRequest request)
    {
        var resultStatusCode = await _databaseService.AddClientRentalInformation(request);
        return Created("" + resultStatusCode, request);
    }
}