
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

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
    }

    private static void Configure(WebApplication app)
    {
        app.MapControllers();

        app.MapGet("/api/status", () => new
        {
            message = "API werkt"
        });
    }
}
