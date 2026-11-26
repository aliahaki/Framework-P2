
namespace Game.Api.Models;

// Gegevens die de console naar de API stuurt
public class StartGameRequest
{
    public required string PlayerName { get; set; }
}

// Gegevens die de API terugstuurt naar de console
public class GameResponse
{
    public int Id { get; set; }

    public string CurrentLocation { get; set; } = "";

    public string Message { get; set; } = "";
}
