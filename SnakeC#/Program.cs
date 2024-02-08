﻿// Add main menu with color and speed options w/ Maybe special game modes :/
// Add randomized locations for food and eating
// Make snake die if it hits itself
// Add score and highscore using files and eating
namespace Snake
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class Snake
    {
        private List<Point> bodyLocations = new List<Point>();
        private Direction direction;

        public Snake()
        {
            direction = Direction.Right;
            bodyLocations = new List<Point> {
                new Point(10, 2),
                new Point(11, 2),
                new Point(12, 2),
                new Point(13, 2),
                new Point(14, 2),
            };

            foreach (var p in bodyLocations)
            {
                Console.SetCursorPosition(p.X, p.Y);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('#');
            }
        }

        public void UpdateSnake()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                switch(key)
                {
                    case ConsoleKey.UpArrow:
                        if (direction != Direction.Down)
                        {
                            direction = Direction.Up;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (direction != Direction.Up)
                        {
                            direction = Direction.Down;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (direction != Direction.Left)
                        {
                            direction = Direction.Right;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (direction != Direction.Right)
                        {
                            direction = Direction.Left;
                        }
                        break;
                    default:
                        break;
                }
            }
            MoveSnake(direction);
            DrawSnake(direction);
        }

        public void MoveSnake(Direction direction)
        {
            switch(direction)
            {
                case Direction.Up:
                    bodyLocations.Add(new Point(bodyLocations.Last().X, bodyLocations.Last().Y - 1));
                    break;
                case Direction.Down:
                    bodyLocations.Add(new Point(bodyLocations.Last().X, bodyLocations.Last().Y + 1));
                    break;
                case Direction.Right:
                    bodyLocations.Add(new Point(bodyLocations.Last().X + 1, bodyLocations.Last().Y));
                    break;
                case Direction.Left:
                    bodyLocations.Add(new Point(bodyLocations.Last().X - 1, bodyLocations.Last().Y));
                    break;
                default:
                    break;
            }
        }

        public void DrawSnake(Direction direction)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            //Remove tail of snake
            bodyLocations.RemoveAt(0);
            if (direction == Direction.Left)
            {
                Console.SetCursorPosition(bodyLocations.First().X + 1, bodyLocations.First().Y);
                Console.Write(' ');
            }
            else if (direction == Direction.Right)
            {
                Console.SetCursorPosition(bodyLocations.First().X - 1, bodyLocations.First().Y);
                Console.Write(' ');
            }
            else if (direction == Direction.Up)
            {
                Console.SetCursorPosition(bodyLocations.First().X, bodyLocations.First().Y + 1);
                Console.Write(' ');
            }
            else if (direction == Direction.Down)
            {
                Console.SetCursorPosition(bodyLocations.First().X, bodyLocations.First().Y - 1);
                Console.Write(' ');
            }
            //Draw new head of snake
            Console.SetCursorPosition(bodyLocations.Last().X, bodyLocations.Last().Y);
            Console.Write('#');

            //Kill snake if it touches walls
            if (bodyLocations.Last().X >= 129 || bodyLocations.Last().X <= 1 || bodyLocations.Last().Y >= 29 || bodyLocations.Last().Y <= 1)
            {
                Environment.Exit(0);
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            int screenWidth = Console.WindowWidth;
            int screenHeight = Console.WindowHeight;
            int updateSpeed = 100;
            bool gameOver = false;
            Console.CursorVisible = false;
            Snake snake = new();

            char wallChar = '■';
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int i = 0; i < screenWidth; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write(wallChar);

                Console.SetCursorPosition(i, screenHeight - 1);
                Console.Write(wallChar);
            }

            for (int i = 0; i < screenHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(wallChar);

                Console.SetCursorPosition(screenWidth - 1, i);
                Console.Write(wallChar);
            }

            //GameLoop
            while (gameOver != true)
            {
                snake.UpdateSnake();
                Thread.Sleep(updateSpeed);
            }
        }
    }
}