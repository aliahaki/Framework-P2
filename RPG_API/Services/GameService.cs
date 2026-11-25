
namespace Game.Api.Services;

public class GameService
{
    // Spelinformatie ophalen
    public SpelInformatie GetInfo()
    {
        return new SpelInformatie(
            "Dungeon Crawler",
            "Ontdek kerkers, versla monsters en vind de uitgang.",
            8
        );
    }
}

// Gegevens van het spel
public record SpelInformatie(
    string Titel,
    string Omschrijving,
    int AantalLevels
);
