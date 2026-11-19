
namespace RPG_Console;

public class Program
{
    public static void Main(string[] args)
    {
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

                default:
                    Console.WriteLine("Ongeldige keuze. Kies 1 of 2.");
                    break;
            }

            Console.WriteLine();
        }
    }

    // Het hoofdmenu tonen
    private static void ToonMenu()
    {
        Console.WriteLine("==========================");
        Console.WriteLine("      DUNGEON CRAWLER");
        Console.WriteLine("==========================");
        Console.WriteLine("1. Nieuw spel");
        Console.WriteLine("2. Afsluiten");
        Console.WriteLine("==========================");
    }

    // Een nieuw spel beginnen
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
}
