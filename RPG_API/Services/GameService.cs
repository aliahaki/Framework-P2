
using Game.Api.Models;

namespace Game.Api.Services;

public class GameService
{
    // Week 2: informatie over Quiz Challenge
    public SpelInformatie GetInfo()
    {
        return new SpelInformatie(
            "Quiz Challenge",
            "Beantwoord 10 vragen, verdien punten en gebruik je joker!",
            10
        );
    }

    // Week 2: de quizvragen ophalen
    public List<QuizVraag> GetVragen()
    {
        return new List<QuizVraag>
        {
            new(
                1,
                "Wat is de hoofdstad van Frankrijk?",
                new[] { "Berlijn", "Parijs", "Madrid", "Rome" },
                2
            ),

            new(
                2,
                "Welke planeet wordt de rode planeet genoemd?",
                new[] { "Venus", "Jupiter", "Mars", "Saturnus" },
                3
            ),

            new(
                3,
                "Welke bestandsextensie gebruikt C#?",
                new[] { ".cs", ".html", ".css", ".php" },
                1
            ),

            new(
                4,
                "Hoeveel minuten zitten er in een uur?",
                new[] { "30", "60", "90", "100" },
                2
            ),

            new(
                5,
                "Welk dier zegt miauw?",
                new[] { "Hond", "Kat", "Koe", "Schaap" },
                2
            ),

            new(
                6,
                "Wat is 7 x 8?",
                new[] { "48", "54", "56", "64" },
                3
            ),

            new(
                7,
                "Welke HTTP-methode haalt gegevens op?",
                new[] { "GET", "DELETE", "PATCH", "POST" },
                1
            ),

            new(
                8,
                "Welke kleur krijg je als je blauw en geel mengt?",
                new[] { "Paars", "Oranje", "Rood", "Groen" },
                4
            ),

            new(
                9,
                "Welke oceaan ligt tussen Europa en Amerika?",
                new[]
                {
                    "Indische Oceaan",
                    "Atlantische Oceaan",
                    "Stille Oceaan",
                    "Zuidelijke Oceaan"
                },
                2
            ),

            new(
                10,
                "Hoeveel zijden heeft een zeshoek?",
                new[] { "4", "5", "6", "8" },
                3
            )
        };
    }

    // Week 3: een nieuw spel starten via de API
    public Task<GameResponse> StartAsync(StartGameRequest request)
    {
        // Spelernaam controleren
        if (string.IsNullOrWhiteSpace(request.PlayerName))
        {
            throw new ArgumentException(
                "Je moet eerst een spelernaam invullen."
            );
        }

        string spelerNaam = request.PlayerName.Trim();

        // Tijdelijk spel zonder database
        GameResponse spel = new GameResponse
        {
            Id = 1,
            CurrentLocation = "Quizzaal",
            Message = $"Welkom bij Quiz Challenge, {spelerNaam}!"
        };

        // Tijdelijk resultaat teruggeven als Task
        return Task.FromResult(spel);
    }
}

// Spelinformatie uit Week 2
public record SpelInformatie(
    string Titel,
    string Omschrijving,
    int AantalVragen
);

// Quizvraag uit Week 2
public record QuizVraag(
    int Id,
    string Vraag,
    string[] Antwoorden,
    int JuistAntwoord
);
