
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;

namespace RPG_Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Eén HttpClient voor alle API-verzoeken
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
                    await StartNieuwSpel(httpClient);
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

    // Hoofdmenu
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

    // Week 3: nieuw spel starten via POST
    private static async Task StartNieuwSpel(HttpClient httpClient)
    {
        Console.Write("Wat is jouw naam? ");
        string naam = Console.ReadLine()?.Trim() ?? "";

        GameResponseDto? nieuwSpel;

        try
        {
            // Spelernaam versturen naar de API
            using HttpResponseMessage antwoord =
                await httpClient.PostAsJsonAsync(
                    "api/game",
                    new StartGameRequestDto(naam)
                );

            // Controleren of de aanvraag gelukt is
            if (!antwoord.IsSuccessStatusCode)
            {
                if ((int)antwoord.StatusCode == 400)
                {
                    Console.WriteLine(
                        "Je moet eerst een spelernaam invullen."
                    );
                }
                else
                {
                    Console.WriteLine(
                        $"Spel starten mislukt. HTTP {(int)antwoord.StatusCode}."
                    );
                }

                return;
            }

            // JSON-response omzetten naar een DTO
            nieuwSpel =
                await antwoord.Content.ReadFromJsonAsync<GameResponseDto>();
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
            Console.WriteLine("Ongeldig antwoord van de API.");
            return;
        }

        if (nieuwSpel == null)
        {
            Console.WriteLine("Geen spelgegevens ontvangen.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine(nieuwSpel.Message);
        Console.WriteLine($"Spel-ID: {nieuwSpel.Id}");
        Console.WriteLine($"Locatie: {nieuwSpel.CurrentLocation}");

        // Pas na een succesvolle POST begint de quiz
        await SpeelQuiz(httpClient, naam);
    }

    // De bestaande quiz spelen
    private static async Task SpeelQuiz(HttpClient httpClient, string naam)
    {
        List<QuizVraagDto>? vragen;

        try
        {
            // Quizvragen ophalen met GET
            vragen =
                await httpClient.GetFromJsonAsync<List<QuizVraagDto>>(
                    "api/game/vragen"
                );
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Kan de quizvragen niet ophalen.");
            return;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("De API reageert niet op tijd.");
            return;
        }
        catch (JsonException)
        {
            Console.WriteLine("Ongeldige quizvragen ontvangen.");
            return;
        }

        if (vragen == null || vragen.Count == 0)
        {
            Console.WriteLine("Er zijn geen vragen gevonden.");
            return;
        }

        // Willekeurige volgorde van vragen
        vragen = vragen
            .OrderBy(vraag => Random.Shared.Next())
            .ToList();

        int score = 0;
        int levens = 3;
        int beantwoordeVragen = 0;
        bool jokerGebruikt = false;

        Console.WriteLine();
        Console.WriteLine("Je krijgt 10 punten per goed antwoord.");
        Console.WriteLine("Bij een fout antwoord verlies je 1 leven.");
        Console.WriteLine("Je hebt 1 joker.");
        Console.WriteLine("Typ 0 om terug te gaan naar het menu.");

        for (int i = 0; i < vragen.Count && levens > 0; i++)
        {
            QuizVraagDto vraag = vragen[i];

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

                // De vier antwoordmogelijkheden tonen
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

                // Terug naar hoofdmenu
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

                    // Twee verkeerde antwoorden weghalen
                    int[] fouteAntwoorden = Enumerable.Range(1, 4)
                        .Where(nummer => nummer != vraag.JuistAntwoord)
                        .OrderBy(nummer => Random.Shared.Next())
                        .Take(2)
                        .ToArray();

                    verwijderdeAntwoorden.UnionWith(fouteAntwoorden);

                    Console.WriteLine();
                    Console.WriteLine("JOKER GEBRUIKT!");
                    Console.WriteLine(
                        "Twee foute antwoorden zijn weggehaald."
                    );

                    continue;
                }

                // Antwoord controleren
                if (!int.TryParse(invoer, out int antwoord) ||
                    antwoord < 1 ||
                    antwoord > 4)
                {
                    Console.WriteLine("Kies 1, 2, 3 of 4.");
                    continue;
                }

                if (verwijderdeAntwoorden.Contains(antwoord))
                {
                    Console.WriteLine("Dit antwoord is weggehaald!");
                    continue;
                }

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

        // Eindresultaat
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
            Console.WriteLine("GAME OVER!");
            Console.WriteLine("Je hebt geen levens meer.");
        }
        else if (score == vragen.Count * 10)
        {
            Console.WriteLine("PERFECTE SCORE!");
            Console.WriteLine("Je hebt alle vragen goed!");
        }
        else
        {
            Console.WriteLine("GOED GESPEELD!");
            Console.WriteLine("Je hebt de quiz afgerond!");
        }

        Console.WriteLine("================================");
    }

    // Status ophalen via GET
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

    // Spelinformatie ophalen via GET
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

// DTO's voor de communicatie met de API

public record StatusDto(string Message);

public record SpelInfoDto(
    string Titel,
    string Omschrijving,
    int AantalVragen
);

public record QuizVraagDto(
    int Id,
    string Vraag,
    string[] Antwoorden,
    int JuistAntwoord
);

// Week 3: gegevens voor POST
public record StartGameRequestDto(string PlayerName);

// Week 3: gegevens uit de POST-response
public record GameResponseDto(
    int Id,
    string CurrentLocation,
    string Message
);
