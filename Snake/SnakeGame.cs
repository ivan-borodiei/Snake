using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class SnakeGame
{
    readonly int FieldWidth = 40;
    readonly int FieldHeight = 20;
    readonly int OffsetX = 2;
    readonly int OffsetY = 1;

    struct Point
    {
        public int X;
        public int Y;
        public Point(int x, int y) { X = x; Y = y; }
    }

    public void Run()
    {
        // Ensure console is large enough to display the field
        try { Console.SetWindowSize(Math.Max(Console.WindowWidth, FieldWidth + OffsetX + 2), Math.Max(Console.WindowHeight, FieldHeight + OffsetY + 4)); } catch { }

        DrawBorder();

        var rng = new Random();
        var snake = new List<Point>();
        int startX = FieldWidth / 2;
        int startY = FieldHeight / 2;
        int initialLength = 5;
        for (int i = 0; i < initialLength; i++) snake.Add(new Point(startX - i, startY));

        Point food = PlaceFood(rng, snake);
        DrawPoint(food, '@', ConsoleColor.Yellow);

        int dx = 1, dy = 0; // moving right initially
        int score = 0;
        bool gameOver = false;

        // draw initial snake
        foreach (var p in snake) DrawPoint(p, 'O', ConsoleColor.Green);

        int baseDelay = 120; // milliseconds

        while (!gameOver)
        {
            // input
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (dy != 1) { dx = 0; dy = -1; }
                        break;
                    case ConsoleKey.DownArrow:
                        if (dy != -1) { dx = 0; dy = 1; }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (dx != 1) { dx = -1; dy = 0; }
                        break;
                    case ConsoleKey.RightArrow:
                        if (dx != -1) { dx = 1; dy = 0; }
                        break;
                    case ConsoleKey.Escape:
                        // immediate exit to OS prompt (treat as game over and then quit)
                        gameOver = true; break;
                }
            }

            // move
            var head = snake[0];
            var newHead = new Point(head.X + dx, head.Y + dy);

            // collisions with walls
            if (newHead.X < 0 || newHead.X >= FieldWidth || newHead.Y < 0 || newHead.Y >= FieldHeight)
            {
                gameOver = true; break;
            }

            // collision with self
            if (snake.Skip(0).Any(p => p.X == newHead.X && p.Y == newHead.Y))
            {
                gameOver = true; break;
            }

            // add head
            snake.Insert(0, newHead);
            DrawPoint(newHead, 'O', ConsoleColor.Green);

            // ate food?
            if (newHead.X == food.X && newHead.Y == food.Y)
            {
                score += 1;
                food = PlaceFood(rng, snake);
                DrawPoint(food, '@', ConsoleColor.Yellow);
            }
            else
            {
                // remove tail
                var tail = snake[snake.Count - 1];
                ErasePoint(tail);
                snake.RemoveAt(snake.Count - 1);
            }

            // draw score
            DrawStatus(score);

            // adjust speed slightly by score
            int delay = Math.Max(30, baseDelay - score * 4);
            Thread.Sleep(delay);
        }

        // Game over
        DrawGameOver();
    }

    void DrawBorder()
    {
        Console.Clear();
        // top border
        for (int x = -1; x <= FieldWidth; x++)
        {
            DrawAt(x, -1, '#', ConsoleColor.White);
            DrawAt(x, FieldHeight, '#', ConsoleColor.White);
        }
        for (int y = 0; y < FieldHeight; y++)
        {
            DrawAt(-1, y, '#', ConsoleColor.White);
            DrawAt(FieldWidth, y, '#', ConsoleColor.White);
        }
        DrawStatus(0);
    }

    void DrawStatus(int score)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.SetCursorPosition(OffsetX, OffsetY + FieldHeight + 1);
        Console.Write($" Score: {score}    (Arrows to move, Esc to quit, R to restart)      ");
        Console.ResetColor();
    }

    void DrawPoint(Point p, char ch, ConsoleColor color)
    {
        DrawAt(p.X, p.Y, ch, color);
    }

    void ErasePoint(Point p)
    {
        DrawAt(p.X, p.Y, ' ', ConsoleColor.Black);
    }

    void DrawAt(int x, int y, char ch, ConsoleColor color)
    {
        try
        {
            Console.SetCursorPosition(OffsetX + x + 1, OffsetY + y + 1);
            Console.ForegroundColor = color;
            Console.Write(ch);
            Console.ResetColor();
        }
        catch
        {
            // ignore drawing errors if console is resized too small
        }
    }

    Point PlaceFood(Random rng, List<Point> snake)
    {
        while (true)
        {
            int fx = rng.Next(0, FieldWidth);
            int fy = rng.Next(0, FieldHeight);
            bool onSnake = snake.Any(p => p.X == fx && p.Y == fy);
            if (!onSnake) return new Point(fx, fy);
        }
    }

    void DrawGameOver()
    {
        string msg = " GAME OVER ";
        int x = OffsetX + (FieldWidth / 2) - (msg.Length / 2) + 1;
        int y = OffsetY + (FieldHeight / 2) + 1;
        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(msg);
        Console.ResetColor();
        Console.SetCursorPosition(OffsetX + 1, OffsetY + FieldHeight + 2);
        Console.Write("Press R to restart or Esc to exit...");
    }
}
