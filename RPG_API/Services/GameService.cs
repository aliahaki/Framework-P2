
namespace Game.Api.Services;

public class GameService
{
    // Algemene informatie over onze quizgame
    public SpelInformatie GetInfo()
    {
        return new SpelInformatie(
            "Quiz Challenge",
            "Beantwoord 10 vragen, verdien punten en gebruik je joker!",
            10
        );
    }

    // De vragen en antwoorden van de quiz
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
                "Welke HTTP-methode gebruikt een API om gegevens op te halen?",
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
}

// Informatie over het spel
public record SpelInformatie(
    string Titel,
    string Omschrijving,
    int AantalVragen
);

// Eén quizvraag met vier antwoorden
public record QuizVraag(
    int Id,
    string Vraag,
    string[] Antwoorden,
    int JuistAntwoord
);
