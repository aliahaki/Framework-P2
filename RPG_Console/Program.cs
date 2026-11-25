
using System.Net.Http.Json;
using System.Text.Json;

namespace RPG_Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Week 2: verbinding met de API maken
        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5193/"),
            Timeout = TimeSpan.FromSeconds(5)
        };

        // API-status ophalen
        await ToonApiStatus(httpClient);

        bool spelActief = true;

        // Het menu blijft draaien totdat de speler afsluit
        while (spelActief)
        {
            ToonMenu();

            Console.Write("Maak je keuze: ");
            string keuze = Console.ReadLine()?.Trim() ?? "";

            switch (keuze)
            {
                case "1":
                    StartNieuwSpel();
                    break;

                case "2":
                    Console.WriteLine("Bedankt voor het spelen!");
                    spelActief = false;
                    break;

                case "3":
                    await ToonSpelInformatie(httpClient);
                    break;

                default:
                    Console.WriteLine("Ongeldige keuze. Kies 1, 2 of 3.");
                    break;
            }

            Console.WriteLine();
        }
    }

    // Week 1: het hoofdmenu tonen
    private static void ToonMenu()
    {
        Console.WriteLine("==========================");
        Console.WriteLine("      DUNGEON CRAWLER");
        Console.WriteLine("==========================");
        Console.WriteLine("1. Nieuw spel");
        Console.WriteLine("2. Afsluiten");
        Console.WriteLine("3. Spelinformatie");
        Console.WriteLine("==========================");
    }

    // Week 1: een nieuw spel beginnen
    private static void StartNieuwSpel()
    {
        Console.Write("Voer je naam in: ");
        string naam = Console.ReadLine()?.Trim() ?? "";

        // Controleren of de naam leeg is
        if (string.IsNullOrWhiteSpace(naam))
        {
            Console.WriteLine("Je moet een naam invullen!");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"Welkom, {naam}!");
        Console.WriteLine("Je avontuur begint!");
    }

    // Week 2: de API-status ophalen
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

    // Week 2: spelinformatie ophalen via de API
    private static async Task ToonSpelInformatie(HttpClient httpClient)
    {
        try
        {
            SpelInfoDto? info =
                await httpClient.GetFromJsonAsync<SpelInfoDto>(
                    "api/game/info"
                );

            if (info == null)
            {
                Console.WriteLine("Geen spelinformatie gevonden.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("===== SPELINFORMATIE =====");
            Console.WriteLine($"Titel: {info.Titel}");
            Console.WriteLine($"Omschrijving: {info.Omschrijving}");
            Console.WriteLine($"Aantal levels: {info.AantalLevels}");
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

// Week 2: gegevens van de API ontvangen
public record StatusDto(string Message);

public record SpelInfoDto(
    string Titel,
    string Omschrijving,
    int AantalLevels
);
