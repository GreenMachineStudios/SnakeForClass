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
        public string Value { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Value = null;
        }

        public void ClearValue()
        {
            Value = " ";
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
        }

        public void UpdateSnake()
        {
            //Move in direction
        }

        public void DrawSnake()
        {
            foreach (var p in bodyLocations)
            {
                Console.SetCursorPosition(p.X, p.Y);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('■');
            }
            Console.ResetColor();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            int updateSpeed = 100;
            bool gameOver = false;
            Console.CursorVisible = false;
            Snake snake = new();

            //GameLoop
            while(gameOver != true)
            {
                snake.DrawSnake();
                Thread.Sleep(updateSpeed);
            }
        }
    }
}