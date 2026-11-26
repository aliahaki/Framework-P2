
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;

namespace RPG_Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Verbinding maken met onze API
        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5193/"),
            Timeout = TimeSpan.FromSeconds(5)
        };

        await ToonApiStatus(httpClient);

        bool programmaActief = true;

        while (programmaActief)
        {
            ToonMenu();

            Console.Write("Maak je keuze: ");
            string keuze = Console.ReadLine()?.Trim() ?? "";

            Console.WriteLine();

            switch (keuze)
            {
                case "1":
                    await StartQuiz(httpClient);
                    break;

                case "2":
                    Console.WriteLine("Bedankt voor het spelen!");
                    programmaActief = false;
                    break;

                case "3":
                    await ToonSpelInformatie(httpClient);
                    break;

                default:
                    Console.WriteLine("Kies 1, 2 of 3.");
                    break;
            }

            Console.WriteLine();
        }
    }

    // Hoofdmenu van de game
    private static void ToonMenu()
    {
        Console.WriteLine("================================");
        Console.WriteLine("         QUIZ CHALLENGE");
        Console.WriteLine("================================");
        Console.WriteLine("1. Nieuw spel");
        Console.WriteLine("2. Afsluiten");
        Console.WriteLine("3. Spelinformatie");
        Console.WriteLine("================================");
    }

    // Quiz starten
    private static async Task StartQuiz(HttpClient httpClient)
    {
        List<QuizVraagDto>? vragen;

        try
        {
            // De vragen ophalen vanuit de API
            vragen = await httpClient.GetFromJsonAsync<List<QuizVraagDto>>(
                "api/game/vragen"
            );
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Kan geen verbinding maken met de API.");
            return;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("De API reageert niet op tijd.");
            return;
        }
        catch (JsonException)
        {
            Console.WriteLine("De API heeft ongeldige gegevens gestuurd.");
            return;
        }

        if (vragen == null || vragen.Count == 0)
        {
            Console.WriteLine("Er zijn geen quizvragen gevonden.");
            return;
        }

        Console.Write("Wat is jouw naam? ");
        string naam = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(naam))
        {
            Console.WriteLine("Je moet een naam invullen!");
            return;
        }

        // Vragen in een willekeurige volgorde zetten
        vragen = vragen
            .OrderBy(vraag => Random.Shared.Next())
            .ToList();

        int score = 0;
        int levens = 3;
        int beantwoordeVragen = 0;
        bool jokerGebruikt = false;

        Console.WriteLine();
        Console.WriteLine($"Welkom bij Quiz Challenge, {naam}!");
        Console.WriteLine("Je krijgt 10 punten per goed antwoord.");
        Console.WriteLine("Bij een fout antwoord verlies je 1 leven.");
        Console.WriteLine("Je hebt 1 joker die 2 foute antwoorden weghaalt.");
        Console.WriteLine("Typ 0 om terug te gaan naar het hoofdmenu.");

        // Alle vragen doorlopen zolang de speler levens heeft
        for (int i = 0; i < vragen.Count && levens > 0; i++)
        {
            QuizVraagDto vraag = vragen[i];

            // Antwoorden die door de joker zijn verwijderd
            HashSet<int> verwijderdeAntwoorden = new();

            bool vraagBeantwoord = false;

            while (!vraagBeantwoord)
            {
                Console.WriteLine();
                Console.WriteLine("================================");
                Console.WriteLine($"Speler: {naam}");
                Console.WriteLine($"Score: {score} punten");
                Console.WriteLine($"Levens: {levens}");
                Console.WriteLine(
                    $"Joker: {(jokerGebruikt ? "Gebruikt" : "Beschikbaar")}"
                );
                Console.WriteLine("================================");

                Console.WriteLine($"Vraag {i + 1} van {vragen.Count}");
                Console.WriteLine(vraag.Vraag);
                Console.WriteLine();

                // Antwoordmogelijkheden tonen
                for (int nummer = 1; nummer <= vraag.Antwoorden.Length; nummer++)
                {
                    if (verwijderdeAntwoorden.Contains(nummer))
                    {
                        Console.WriteLine($"{nummer}. [Weggehaald]");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"{nummer}. {vraag.Antwoorden[nummer - 1]}"
                        );
                    }
                }

                Console.WriteLine();
                Console.Write("Antwoord (1-4, J = joker, 0 = menu): ");

                string invoer =
                    Console.ReadLine()?.Trim().ToUpperInvariant() ?? "0";

                // Terug naar het hoofdmenu
                if (invoer == "0")
                {
                    Console.WriteLine("Je gaat terug naar het hoofdmenu.");
                    return;
                }

                // Joker gebruiken
                if (invoer == "J")
                {
                    if (jokerGebruikt)
                    {
                        Console.WriteLine("Je hebt je joker al gebruikt!");
                        continue;
                    }

                    jokerGebruikt = true;

                    // Twee willekeurige foute antwoorden verwijderen
                    int[] fouteAntwoorden = Enumerable.Range(1, 4)
                        .Where(nummer => nummer != vraag.JuistAntwoord)
                        .OrderBy(nummer => Random.Shared.Next())
                        .Take(2)
                        .ToArray();

                    verwijderdeAntwoorden.UnionWith(fouteAntwoorden);

                    Console.WriteLine();
                    Console.WriteLine("JOKER GEBRUIKT!");
                    Console.WriteLine("Twee foute antwoorden zijn weggehaald.");
                    continue;
                }

                // Controleren of de invoer geldig is
                if (!int.TryParse(invoer, out int antwoord) ||
                    antwoord < 1 ||
                    antwoord > 4)
                {
                    Console.WriteLine("Kies een antwoord van 1 tot en met 4.");
                    continue;
                }

                if (verwijderdeAntwoorden.Contains(antwoord))
                {
                    Console.WriteLine("Dit antwoord is weggehaald!");
                    continue;
                }

                // Antwoord controleren
                if (antwoord == vraag.JuistAntwoord)
                {
                    score += 10;

                    Console.WriteLine();
                    Console.WriteLine("GOED GEDAAN! +10 punten!");
                }
                else
                {
                    levens--;

                    Console.WriteLine();
                    Console.WriteLine("HELAAS, FOUT ANTWOORD!");
                    Console.WriteLine("Je verliest 1 leven.");

                    Console.WriteLine(
                        $"Het juiste antwoord was: " +
                        $"{vraag.Antwoorden[vraag.JuistAntwoord - 1]}"
                    );
                }

                beantwoordeVragen++;
                vraagBeantwoord = true;
            }
        }

        // Eindresultaat tonen
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine("          EINDSCORE");
        Console.WriteLine("================================");
        Console.WriteLine($"Speler: {naam}");
        Console.WriteLine($"Vragen beantwoord: {beantwoordeVragen}");
        Console.WriteLine($"Score: {score} punten");
        Console.WriteLine($"Levens over: {levens}");

        if (levens == 0)
        {
            Console.WriteLine();
            Console.WriteLine("GAME OVER!");
            Console.WriteLine("Je hebt geen levens meer.");
        }
        else if (score == vragen.Count * 10)
        {
            Console.WriteLine();
            Console.WriteLine("PERFECTE SCORE!");
            Console.WriteLine("Je hebt alle vragen goed!");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("GOED GESPEELD!");
            Console.WriteLine("Je hebt de quiz afgerond!");
        }

        Console.WriteLine("================================");
    }

    // Status van de API controleren
    private static async Task ToonApiStatus(HttpClient httpClient)
    {
        try
        {
            StatusDto? status =
                await httpClient.GetFromJsonAsync<StatusDto>(
                    "api/game/status"
                );

            Console.WriteLine($"API-status: {status?.Message}");
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Kan geen verbinding maken met de API.");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("De API reageert niet op tijd.");
        }
        catch (JsonException)
        {
            Console.WriteLine("Ongeldig antwoord van de API.");
        }
    }

    // Spelinformatie ophalen uit de API
    private static async Task ToonSpelInformatie(HttpClient httpClient)
    {
        try
        {
            SpelInfoDto? informatie =
                await httpClient.GetFromJsonAsync<SpelInfoDto>(
                    "api/game/info"
                );

            if (informatie == null)
            {
                Console.WriteLine("Geen spelinformatie gevonden.");
                return;
            }

            Console.WriteLine("===== SPELINFORMATIE =====");
            Console.WriteLine($"Titel: {informatie.Titel}");
            Console.WriteLine($"Omschrijving: {informatie.Omschrijving}");
            Console.WriteLine($"Aantal vragen: {informatie.AantalVragen}");
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Kan geen verbinding maken met de API.");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("De API reageert niet op tijd.");
        }
        catch (JsonException)
        {
            Console.WriteLine("Ongeldig antwoord van de API.");
        }
    }
}

// DTO voor de API-status
public record StatusDto(string Message);

// DTO voor de spelinformatie
public record SpelInfoDto(
    string Titel,
    string Omschrijving,
    int AantalVragen
);

// DTO voor één quizvraag
public record QuizVraagDto(
    int Id,
    string Vraag,
    string[] Antwoorden,
    int JuistAntwoord
);
