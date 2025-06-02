using kolokwium_1.Models;

namespace kolokwium_1.Services;

public interface IDatabaseService
{
    public void AddSomethingToDatabaseAsync();
    public void UpdateSomethingInDatabaseAsync();
    public Task<List<Trip>> GetSomethingFromDatabaseAsync();
    
    public void DeleteSomethingFromDatabaseAsync();
}

