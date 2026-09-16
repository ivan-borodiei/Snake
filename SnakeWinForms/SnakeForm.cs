using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SnakeWinForms
{
    public class SnakeForm : Form
    {
        readonly int FieldWidth = 40;
        readonly int FieldHeight = 20;
        readonly int CellSize = 15;

        struct Cell { public int X; public int Y; public Cell(int x, int y) { X = x; Y = y; } }

        List<Cell> snake;
        Cell food;
        int dx, dy;
        System.Windows.Forms.Timer timer;
        Random rng = new Random();
        bool gameOver;
        int score;

        public SnakeForm()
        {
            // Build form dynamically
            this.Text = "Snake";
            this.ClientSize = new Size(FieldWidth * CellSize + 40, FieldHeight * CellSize + 80);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;
            this.DoubleBuffered = true;

            this.KeyDown += SnakeForm_KeyDown;
            this.FormClosed += (s, e) => { timer?.Stop(); };

            StartGame();
        }

        void StartGame()
        {
            snake = new List<Cell>();
            int startX = FieldWidth / 2;
            int startY = FieldHeight / 2;
            int initialLength = 5;
            for (int i = 0; i < initialLength; i++) snake.Add(new Cell(startX - i, startY));

            dx = 1; dy = 0;
            score = 0;
            gameOver = false;

            PlaceFood();

            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 120;
            timer.Tick += Timer_Tick;
            timer.Start();
            Invalidate();
        }

        void PlaceFood()
        {
            while (true)
            {
                int fx = rng.Next(0, FieldWidth);
                int fy = rng.Next(0, FieldHeight);
                bool onSnake = snake.Any(p => p.X == fx && p.Y == fy);
                if (!onSnake) { food = new Cell(fx, fy); return; }
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (gameOver) return;

            var head = snake[0];
            var newHead = new Cell(head.X + dx, head.Y + dy);

            // wall collision
            if (newHead.X < 0 || newHead.X >= FieldWidth || newHead.Y < 0 || newHead.Y >= FieldHeight)
            {
                GameOver();
                return;
            }

            // self collision
            if (snake.Skip(0).Any(p => p.X == newHead.X && p.Y == newHead.Y))
            {
                GameOver();
                return;
            }

            snake.Insert(0, newHead);

            // eat food
            if (newHead.X == food.X && newHead.Y == food.Y)
            {
                score++;
                PlaceFood();
                // speed up slightly
                timer.Interval = Math.Max(30, 120 - score * 4);
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }

            Invalidate();
        }

        void GameOver()
        {
            gameOver = true;
            timer.Stop();
            Invalidate();
        }

        private void SnakeForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Close(); return; }

            if (!gameOver)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        if (dy != 1) { dx = 0; dy = -1; }
                        break;
                    case Keys.Down:
                        if (dy != -1) { dx = 0; dy = 1; }
                        break;
                    case Keys.Left:
                        if (dx != 1) { dx = -1; dy = 0; }
                        break;
                    case Keys.Right:
                        if (dx != -1) { dx = 1; dy = 0; }
                        break;
                }
            }
            else
            {
                if (e.KeyCode == Keys.R) StartGame();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // draw background
            g.Clear(Color.Black);

            // draw border
            using (var pen = new Pen(Color.White))
            {
                g.DrawRectangle(pen, OffsetX(), OffsetY(), FieldWidth * CellSize + 1, FieldHeight * CellSize + 1);
            }

            // draw food
            var foodRect = CellRect(food);
            g.FillEllipse(Brushes.Yellow, foodRect);

            // draw snake
            foreach (var s in snake)
            {
                var r = CellRect(s);
                g.FillRectangle(Brushes.Green, r);
                g.DrawRectangle(Pens.DarkGreen, r);
            }

            // draw score
            g.DrawString($"Score: {score}", this.Font, Brushes.Cyan, OffsetX(), OffsetY() + FieldHeight * CellSize + 8);

            if (gameOver)
            {
                var msg = "GAME OVER - Press R to restart or Esc to exit";
                var sz = g.MeasureString(msg, this.Font);
                g.FillRectangle(Brushes.Black, (ClientSize.Width - sz.Width) / 2 - 4, (ClientSize.Height - sz.Height) / 2 - 4, sz.Width + 8, sz.Height + 8);
                g.DrawString(msg, this.Font, Brushes.Red, (ClientSize.Width - sz.Width) / 2, (ClientSize.Height - sz.Height) / 2);
            }
        }

        int OffsetX() => 10;
        int OffsetY() => 10;

        Rectangle CellRect(Cell c)
        {
            return new Rectangle(OffsetX() + c.X * CellSize, OffsetY() + c.Y * CellSize, CellSize, CellSize);
        }
    }
}
