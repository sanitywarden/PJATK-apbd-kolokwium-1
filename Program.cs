using kolokwium_1.Services;

namespace kolokwium_1;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Dependency injection
        builder.Services.AddScoped<IDatabaseService, DatabaseService>();
        
        // Register controller services
        builder.Services.AddControllers();
        
        var app = builder.Build();

        // Use routing and map controllers
        app.MapControllers();

        app.Run();
    }
}