using kolokwium_1.Models;
using Microsoft.AspNetCore.Mvc;

namespace kolokwium_1.Services;

public interface IDatabaseService
{
    public Task<int> AddClientRentalInformation(RentalRequest rentalRequest);
    public Task<ClientData> GetClientInfoAsync(int id);
    
}

