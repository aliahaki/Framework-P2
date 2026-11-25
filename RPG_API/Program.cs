
using Game.Api.Services;

namespace Game.Features;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        Configure(app);

        app.Run();
    }

    // Services registreren
    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();

        // Week 2: GameService toevoegen
        builder.Services.AddScoped<GameService>();
    }

    // API-routes instellen
    private static void Configure(WebApplication app)
    {
        app.MapControllers();

        // Status controleren
        app.MapGet("/api/status", () => new
        {
            message = "API werkt"
        });
    }
}
