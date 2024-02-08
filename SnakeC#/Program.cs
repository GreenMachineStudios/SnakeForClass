// Add main menu with color and speed options w/ Maybe special game modes :/
// Add score and highscore using files and eating
// Add differient colored head
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

    public class Food
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Food(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void AddFood()
        {
            Random rng = new Random();
            X = rng.Next(2, 117);
            Y = rng.Next(2,27);
        }
    }

    public class Snake
    {
        public List<Point> bodyLocations = new List<Point>();
        private Direction direction;
        private Food food;
        public bool gameOver = false;

        private int startingX = (Console.WindowWidth / 2) - 10;
        private int startingY = (Console.WindowHeight / 2) - 1;

        public Snake()
        {
            direction = Direction.Right;
            bodyLocations = new List<Point> {
                new Point(startingX, startingY),
                new Point(startingX + 1, startingY),
                new Point(startingX + 2, startingY),
                new Point(startingX + 3, startingY),
                new Point(startingX + 4, startingY),
            };

            foreach (var p in bodyLocations)
            {
                Console.SetCursorPosition(p.X, p.Y);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('#');
            }

            Console.SetCursorPosition(bodyLocations.First().X, bodyLocations.First().Y);
            Console.Write(' ');

            food = new Food(0, 0);
            food.AddFood();
            if (bodyLocations.Contains(new Point(food.X, food.Y)))
            {
                food.AddFood();
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

            if (bodyLocations.Last().X == food.X && bodyLocations.Last().Y == food.Y)
            {
                bodyLocations.Insert(0, new Point(food.X, food.Y));
                food.AddFood();
            }

            Draw();
        }

        public void MoveSnake(Direction direction)
        {
            Point newHead = new Point();

            switch (direction)
            {
                case Direction.Up:
                    newHead = new Point(bodyLocations.Last().X, bodyLocations.Last().Y - 1);
                    break;
                case Direction.Down:
                    newHead = new Point(bodyLocations.Last().X, bodyLocations.Last().Y + 1);
                    break;
                case Direction.Right:
                    newHead = new Point(bodyLocations.Last().X + 1, bodyLocations.Last().Y);
                    break;
                case Direction.Left:
                    newHead = new Point(bodyLocations.Last().X - 1, bodyLocations.Last().Y);
                    break;
                default:
                    break;
            }
            if (bodyLocations.Last().X >= 118 || bodyLocations.Last().X <= 1 || bodyLocations.Last().Y >= 28 || bodyLocations.Last().Y <= 1)
            {
                gameOver = true;
            }
            else if (bodyLocations.Contains(newHead))
            {
                gameOver = true;
            }

            bodyLocations.Add(newHead);
            bodyLocations.RemoveAt(0);
        }
        public void Draw()
        {
            Console.SetCursorPosition(bodyLocations.First().X, bodyLocations.First().Y);
            Console.Write(' ');

            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(bodyLocations.Last().X, bodyLocations.Last().Y);
            Console.Write('#');

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(food.X, food.Y);
            Console.Write('@');
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            int screenWidth = Console.WindowWidth;
            int screenHeight = Console.WindowHeight;
            int updateSpeed = 70;
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
            while (!snake.gameOver)
            {
                snake.UpdateSnake();
                Thread.Sleep(updateSpeed);
            }
        }
    }
}