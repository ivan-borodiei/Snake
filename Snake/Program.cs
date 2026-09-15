using System;

// Program entry point. Game logic has been moved to SnakeGame class.
class Program
{
    static void Main()
    {
        Console.Title = "Snake";
        Console.CursorVisible = false;

        while (true)
        {
            var game = new SnakeGame();
            game.Run();

            ConsoleKeyInfo k = Console.ReadKey(true);
            if (k.Key == ConsoleKey.Escape) break;
            if (k.Key == ConsoleKey.R) continue; // restart
            // any other key: exit loop and quit
            break;
        }
    }
}
