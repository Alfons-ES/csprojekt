using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program
{
    // Lista med ord
    static List<string> words = new List<string>
    {
        "FIKA", "KAFFE", "TE", "KANELBULLE", "POTATIS",
        "SMÖRGÅS", "OST", "SKINKA", "GURKA", "TOMAT",
        "SEMLA", "WIENERBRÖD", "PRINSESSTÅRTA", "CHOKLADBOLLAR",
        "JULSKINKA", "KÖTTBULLAR", "POTATIS", "GRÄDDSÅS",
        "LINGONSYLT", "PROJEKTLEDNING", "SILL", "KNÄCKEBRÖD",
        "VÄSTERBOTTENOST", "PRÄSTOST", "HUSHÅLLSOST",
        "GLÖGG", "PEPPARKAKA", "LUSSEKATT", "JULMUST",
        "MIDSOMMAR", "SILL", "KRÄFTOR", "VÄSTERBOTTENPAJ",
        "RÅGBRÖD", "KNÄCKEBRÖD", "MACKA", "PÅLÄGG", "OSTMACKA",
        "KAFFEBRÖD", "BULLE", "KAKA", "TÅRTA", "GLASS",
        "CHOKLAD", "GODIS", "LÖRDAGSGODIS", "MARABOU",
        "DAMMSUGARE", "POLKAGRIS", "SALTLAKRITS", "HALLON"
    };

    // Fil för att spara highscore
    static string scoreFile = "hangman_score.json";

    static void Main()
    {
        Console.Title = "Hänga Gubbe";

        while (true)
        {
            Console.Clear();
            ShowMenu();

            Console.Write("Välj: ");
            string choice = Console.ReadLine()?.Trim().ToUpper();

            if (choice == "1")
            {
                PlayGame();
            }
            else if (choice == "2")
            {
                ShowHighScore();
            }
            else if (choice == "X")
            {
                Console.WriteLine("Hej då!");
                break;
            }
            else
            {
                Console.WriteLine("Ogiltigt val. Tryck Enter...");
                Console.ReadLine();
            }
        }
    }

    // Visar huvudmenyn
    static void ShowMenu()
    {
        Console.WriteLine("=== HÄNGA GUBBE ===");
        Console.WriteLine();
        Console.WriteLine("1. Spela");
        Console.WriteLine("2. Visa highscore");
        Console.WriteLine();
        Console.WriteLine("X. Avsluta");
        Console.WriteLine();
    }

    // spelet
    static void PlayGame()
    {
        Console.Clear();

        // Välj ett slumpmässigt ord
        Random rnd = new Random();
        string secretWord = words[rnd.Next(words.Count)];

        // Skapa en array med understreck för varje bokstav
        char[] guessed = new char[secretWord.Length];
        for (int i = 0; i < guessed.Length; i++)
            guessed[i] = '_';

        List<char> wrongGuesses = new List<char>(); // Felaktiga bokstäver
        int lives = 9;                               // Antal liv

        while (lives > 0)
        {
            Console.Clear();
            Console.WriteLine("=== HÄNGA GUBBE ===");
            Console.WriteLine();
            Console.WriteLine("Ord: " + new string(guessed));
            DrawHangman(lives);
            Console.WriteLine(string.Join(" ", wrongGuesses));
            Console.WriteLine();

            // Kolla om man vunnit
            if (new string(guessed) == secretWord)
            {
                Console.WriteLine("Grattis! Du klarade dig! Ordet var: " + secretWord);
                SaveScore(true);
                Console.WriteLine("Tryck Enter för att gå tillbaka...");
                Console.ReadLine();
                return;
            }

            Console.Write("Gissa en bokstav: ");
            string input = Console.ReadLine()?.Trim().ToUpper();

            // felhantering
            if (string.IsNullOrEmpty(input) || input.Length != 1 || !char.IsLetter(input[0]))
            {
                Console.WriteLine("Skriv en bokstav!");
                Console.WriteLine("Tryck Enter...");
                Console.ReadLine();
                continue;
            }

            char guess = input[0];

            // Har man redan gissat denna bokstav?
            if (guessed.Contains(guess) || wrongGuesses.Contains(guess))
            {
                Console.WriteLine("Du har redan gissat på " + guess);
                Console.WriteLine("Tryck Enter...");
                Console.ReadLine();
                continue;
            }

            // Kolla om bokstaven finns i ordet
            bool found = false;
            for (int i = 0; i < secretWord.Length; i++)
            {
                if (secretWord[i] == guess)
                {
                    guessed[i] = guess;
                    found = true;
                }
            }

            if (!found)
            {
                wrongGuesses.Add(guess);
                lives--;
            }
        }

        // om man har 0 liv triggas detta
        Console.Clear();
        Console.WriteLine("=== HÄNGA GUBBE ===");
        Console.WriteLine();
        DrawHangman(lives);
        Console.WriteLine("Du förlorade! Ordet var: " + secretWord);
        SaveScore(false);
        Console.WriteLine("Tryck Enter för att gå tillbaka...");
        Console.ReadLine();
    }

    // Sparar om man vann eller förlorade 
    static void SaveScore(bool won)
    {
        ScoreData score = LoadScore();

        if (won)
            score.Wins++;
        else
            score.Losses++;

        string json = JsonSerializer.Serialize(score, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(scoreFile, json);
    }

    // Läser in highscore från fil
    static ScoreData LoadScore()
    {
        if (!File.Exists(scoreFile))
            return new ScoreData();

        string json = File.ReadAllText(scoreFile);
        return JsonSerializer.Deserialize<ScoreData>(json) ?? new ScoreData();
    }

    // Visar highscore
    static void ShowHighScore()
    {
        Console.Clear();
        ScoreData score = LoadScore();

        Console.WriteLine("=== HIGHSCORE ===");
        Console.WriteLine();
        Console.WriteLine("Vinster:   " + score.Wins);
        Console.WriteLine("Förluster: " + score.Losses);
        Console.WriteLine();
        Console.WriteLine("Tryck Enter för att gå tillbaka...");
        Console.ReadLine();
    }

    // Rita gubbe beroende på hur många liv som är kvar
    static void DrawHangman(int lives)
    {
        string[] stages =
        {
// 0 
        @"
  +---+
  |   |
  O   |
 /|\  |
 / \  |
      |
=========",

// 1
        @"
  +---+
  |   |
  O   |
 /|\  |
 /    |
      |
=========",

// 2
        @"
  +---+
  |   |
  O   |
 /|\  |
      |
      |
=========",

// 3
        @"
  +---+
  |   |
  O   |
 /|   |
      |
      |
=========",

// 4
        @"
  +---+
  |   |
  O   |
  |   |
      |
      |
=========",

// 5
        @"
  +---+
  |   |
  O   |
      |
      |
      |
=========",

// 6
        @"
  +---+
  |   |
      |
      |
      |
      |
=========",

//7
        @"
  +---+
      |
      |
      |
      |
      |
=========",

//8
        @"
    
      |
      |
      |
      |
      |
=========",

//9
        @"
    
      
      
      
      
      
=========",
    };

        // Visa figur beroende på antal liv
        Console.WriteLine(stages[lives]);
    }

}

// Klass för att spara poäng 
class ScoreData
{
    public int Wins { get; set; } = 0;
    public int Losses { get; set; } = 0;
}
