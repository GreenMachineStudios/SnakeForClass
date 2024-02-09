/* Snake Game In The C# Console
 * Created by Marco Videla
 * Finished: 2/8/2024
 * 
 * Changelog:
 * Added dashing when pressing space bar
 * Colored head
 */

using Spectre.Console;
using System.Media;
using NAudio.Wave;

namespace Snake
{
    public class Highscore
    {
        private static string filePath = "highscores.txt";

        public static void SetHighScore(int score)
        {
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(score);
            }
        }

        public static int GetHighScore()
        {
            int highestScore = 0;

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line) && int.TryParse(line, out int currentScore))
                        {
                            if (currentScore > highestScore)
                            {
                                highestScore = currentScore;
                            }
                        }
                    }
                }
            }

            return highestScore;
        }
    }

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

        public void AddFood(List<Point> snakeLocations)
        {
            Random rng = new Random();
            bool isValidPosition = false;

            while (!isValidPosition)
            {
                X = rng.Next(2, 117);
                Y = rng.Next(5, 27);

                // Check if snake body is in the way
                isValidPosition = true;
                foreach (Point snakeLocation in snakeLocations)
                {
                    if (snakeLocation.X == X && snakeLocation.Y == Y)
                    {
                        isValidPosition = false;
                        break;
                    }
                }
            }
        }
    }

    public class Snake
    {
        public List<Point> bodyLocations = new List<Point>();
        private Direction direction;
        private Food food;
        public bool gameOver = false;

        private int startingX = (Console.WindowWidth / 2) - 5;
        private int startingY = (Console.WindowHeight / 2) - 1;

        private int score = 0;
        private int scoreCounter = 0;

        public int updateSpeed = 70;

        private SoundPlayer gameOverMusic = new SoundPlayer("gameOver.wav");
        private SoundPlayer munch = new SoundPlayer("munch.wav");
        private SoundPlayer bong = new SoundPlayer("bong.wav");

        private WaveOutEvent waveOutMusic = new WaveOutEvent();
        private AudioFileReader readerMusic = new AudioFileReader("game.wav");

        public Snake()
        {
            waveOutMusic.Init(readerMusic);
            waveOutMusic.Play();


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
                Console.Write('█');
            }

            Console.SetCursorPosition(bodyLocations.First().X, bodyLocations.First().Y);
            Console.Write(' ');

            food = new Food(0, 0);
            food.AddFood(bodyLocations);
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
                food.AddFood(bodyLocations);
                munch.Play();
                score++;
                if (score > Highscore.GetHighScore())
                {
                    Highscore.SetHighScore(score);
                }
                scoreCounter++;
                //Speed up the snake if you reach 5+ points everytime
                if (scoreCounter == 5)
                {
                    updateSpeed -= 5;
                    scoreCounter = 0;
                }
            }

            Draw();

            if (gameOver)
            {
                waveOutMusic.Stop();
                GameOver();
            }
        }

        public void MoveSnake(Direction direction)
        {
            Point newHead = new Point();

            switch (direction)
            {
                case Direction.Up:
                    if (bodyLocations.Last().Y <= 5)
                    {
                        gameOver = true;
                    }
                    newHead = new Point(bodyLocations.Last().X, bodyLocations.Last().Y - 1);
                    break;
                case Direction.Down:
                    if (bodyLocations.Last().Y >= 28)
                    {
                        gameOver = true;
                    }
                    newHead = new Point(bodyLocations.Last().X, bodyLocations.Last().Y + 1);
                    break;
                case Direction.Right:
                    if (bodyLocations.Last().X >= 118)
                    {
                        gameOver = true;
                    }
                    newHead = new Point(bodyLocations.Last().X + 1, bodyLocations.Last().Y);
                    break;
                case Direction.Left:
                    if (bodyLocations.Last().X <= 1)
                    {
                        gameOver = true;
                    }
                    newHead = new Point(bodyLocations.Last().X - 1, bodyLocations.Last().Y);
                    break;
                default:
                    break;
            }

            if (bodyLocations.Contains(newHead))
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
            Console.Write('█');

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(food.X, food.Y);
            Console.Write('#');

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(startingX - 5, 2);
            Console.Write("Score: " + score);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.SetCursorPosition(startingX + 5, 2);
            Console.Write("Highscore: " + Highscore.GetHighScore());
        }

        public void DrawBorders()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            for (int i = 1; i < 119; i++)
            {
                Console.SetCursorPosition(i, 4);
                Console.Write('═');
                Console.SetCursorPosition(i, 29);
                Console.Write('═');
            }

            for (int i = 4; i < 29; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write('║');
                Console.SetCursorPosition(119, i);
                Console.Write('║');
            }

            Console.SetCursorPosition(0, 4);
            Console.Write('╔');
            Console.SetCursorPosition(0, 29);
            Console.Write('╚');
            Console.SetCursorPosition(119, 4);
            Console.Write('╗');
            Console.SetCursorPosition(119, 29);
            Console.Write('╝');
        }

        private void GameOver()
        {
            string gameOverSnake = @"
                       _____
                    .-'`     '.
                 __/  __       \
                /  \ /  \       |    ___
               | /`\| /`\|      | .-'  /^\/^\
               | \(/| \(/|      |/     |) |)|
              .-\__/ \__/       |      \_/\_/__..._
      _...---'-.                /   _              '.
     /,      ,             \   '|  `\                \                      
    | ))     ))           /`|   \    `.       /)  /) |   
    | `      `          .'       |     `-._         /
    \                 .'         |     ,_  `--....-'
     `.           __.' ,         |     / /`'''`
       `'-.____.-' /  /,         |    / /
           `. `-.-` .'  \        /   / |
             `-.__.'|    \      |   |  |-.
                _.._|     |     /   |  |  `'.
          .-''``    |     |     |   /  |     `-.
       .'`         /      /     /  |   |        '.
     /`           /      /     |   /   |\         \
    /            |      |      |   |   /\          |
   ||            |      /      |   /     '.        |
   |\            \      |      /   |       '.      /
   \ `.           '.    /      |    \        '---'/
    \  '.           `-./        \    '.          /
     '.  `'.            `-._     '.__  '-._____.'--'''''--.
       '-.  `'--._          `.__     `';----`              \
          `-.     `-.          `.'''```                    ;
             `'-..,_ `-.         `'-.                     /";
            bong.Play();
            Thread.Sleep(100);
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var p in bodyLocations)
            {
                Console.SetCursorPosition(p.X, p.Y);
                Console.Write('@');
                Thread.Sleep(50);
            }
            Thread.Sleep(100);
            Console.Clear();
            gameOverMusic.PlayLooping();
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(gameOverSnake);
            Console.SetCursorPosition(76, 6);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("-----GAME OVER-----");
            Console.SetCursorPosition(79, 8);
            Console.ForegroundColor = ConsoleColor.Magenta;
            AnsiConsole.Markup("Final Score: " + score);
            Console.SetCursorPosition(79, 11);
            Console.ForegroundColor = ConsoleColor.Yellow;
            AnsiConsole.Markup("High Score: " + Highscore.GetHighScore());

            while (true)
            {
                Console.SetCursorPosition(70, 16);
                Console.ForegroundColor = ConsoleColor.White;
                AnsiConsole.MarkupLine("Press [springgreen2]R[/] to Restart or [darkred_1]Q[/] to Quit.");
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.R)
                {
                    Console.Clear();
                    gameOver = false;
                    score = 0;
                    scoreCounter = 0;
                    updateSpeed = 70;
                    direction = Direction.Right;
                    bodyLocations = new List<Point> {
                        new Point(startingX, startingY),
                        new Point(startingX + 1, startingY),
                        new Point(startingX + 2, startingY),
                        new Point(startingX + 3, startingY),
                        new Point(startingX + 4, startingY),
                    };
                    food = new Food(0, 0);
                    food.AddFood(bodyLocations);
                    gameOverMusic.Stop();
                    DrawBorders();
                    readerMusic.Position = 0;
                    waveOutMusic.Init(readerMusic);
                    waveOutMusic.Play();
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Q)
                {
                    gameOverMusic.Stop();
                    Environment.Exit(0);
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
            Snake snake = new();
            snake.DrawBorders();

            //GameLoop
            Console.CursorVisible = false;
            while (true)
            {
                snake.UpdateSnake();
                Thread.Sleep(snake.updateSpeed);
            }
        }

        static void MainMenu()
        {
            //Main menu music for coolness :)
            SoundPlayer menuMusic = new SoundPlayer("menu.wav");
            menuMusic.PlayLooping();

            int highscore = Highscore.GetHighScore();

            bool startGame = false;
            while (!startGame)
            {
                Console.SetCursorPosition(0, 0);
                string snakeyIcon = @"
  _________              __           
 /   _____/ ____ _____  |  | __ ____  
 \_____  \ /    \\__  \ |  |/ // __ \ 
 /        \   |  \/ __ \|    <\  ___/ 
/_________/___|__(______/__|__\\____/
                                       
                                            




                                                                    /^\/^\
                                                                  _|__|  O|
                                                         \/     /~     \_/ \
                                                          \____|__________/  \
                                                                 \_______      \
                                                                         `\     \                 \
                                                                           |     |                  \
                                                                          /      /                    \
                                                                         /     /                       \\
                                                                       /      /                         \ \
                                                                      /     /                            \  \
                                                                    /     /             _----_            \   \
                                                                   /     /           _-~      ~-_         |   |
                                                                  (      (        _-~    _--_    ~-_     _/   |
                                                                   \      ~-____-~    _-~    ~-_    ~-_-~    /
                                                                     ~-_           _-~          ~-_       _-~
                                                                        ~--______-~                ~-___-~";

                AnsiConsole.Markup("[springgreen3]" + snakeyIcon + "[/]");
                Console.SetCursorPosition(0, 6);
                AnsiConsole.MarkupLine("[springgreen2]Created by Marco Videla in 12 hours[/]");
                Console.SetCursorPosition(0, 8);
                AnsiConsole.MarkupLine("[darkorange3]HIGHSCORE = " + highscore + "[/]");
                Console.SetCursorPosition(0, 10);
                AnsiConsole.MarkupLine("[lightseagreen](1) PLAY[/]");
                AnsiConsole.MarkupLine("[darkred_1](2) QUIT[/]");
                AnsiConsole.Markup("\n[teal]Command: [/]");
                int input = Convert.ToInt32(Console.ReadLine());

                switch (input)
                {
                    case 1:
                        Console.Clear();
                        menuMusic.Stop();
                        startGame = true;
                        break;
                    case 2:
                        Console.Clear();
                        menuMusic.Stop();
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }

                Console.Clear();
            }
        }
    }
}