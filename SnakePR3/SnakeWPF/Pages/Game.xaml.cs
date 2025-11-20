using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SnakeWPF.Pages
{
    public partial class Game : Page
    {
        // Кэшированные ресурсы
        private readonly SolidColorBrush _headBrush = new SolidColorBrush(Color.FromRgb(31, 71, 15));
        private readonly SolidColorBrush _bodyBrush = new SolidColorBrush(Color.FromRgb(57, 99, 41));
        private readonly ImageBrush _appleBrush;

        // Кэш элементов
        private readonly Dictionary<Snakes, List<Rectangle>> _snakeElementsCache = new Dictionary<Snakes, List<Rectangle>>();
        private Ellipse _appleElement;

        public Game()
        {
            InitializeComponent();

            _appleBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Image/apple.png"))
            };

            _headBrush.Freeze();
            _bodyBrush.Freeze();
            _appleBrush.Freeze();
        }

        public void CreateUI()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    canvas.Children.Clear();

                    RenderSnake(MainWindow.mainWindow.ViewModelGames.SnakesPlayer);

                    RenderApple();

                    foreach (var gameState in MainWindow.mainWindow.ViewModelGamesList)
                    {
                        RenderSnake(gameState.SnakesPlayer);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateUI: {ex.Message}");
            }
        }

        private void RenderSnake(Snakes snake)
        {
            if (snake?.Points == null || snake.Points.Count == 0)
                return;

            for (int i = 0; i < snake.Points.Count; i++)
            {
                var point = snake.Points[i];
                var isHead = i == 0;

                var rectangle = new Rectangle()
                {
                    Width = 10,
                    Height = 10,
                    Margin = new Thickness(point.X - 10, point.Y - 10, 0, 0),
                    Fill = isHead ? _headBrush : _bodyBrush,
                    Stroke = Brushes.Black
                };

                canvas.Children.Add(rectangle);
            }
        }

        private void RenderApple()
        {
            if (MainWindow.mainWindow.ViewModelGames?.Points == null)
                return;

            var apple = new Ellipse()
            {
                Width = 30,
                Height = 30,
                Margin = new Thickness(
                    MainWindow.mainWindow.ViewModelGames.Points.X - 15,
                    MainWindow.mainWindow.ViewModelGames.Points.Y - 15, 0, 0),
                Fill = _appleBrush
            };

            canvas.Children.Add(apple);
        }

        public void UpdateGameLogic()
        {
            MoveSnake(MainWindow.mainWindow.ViewModelGames.SnakesPlayer);

            foreach (var gameState in MainWindow.mainWindow.ViewModelGamesList)
            {
                MoveSnake(gameState.SnakesPlayer);
            }
        }

        private void MoveSnake(Snakes snake)
        {
            if (snake?.Points == null || snake.Points.Count == 0 || snake.direction == Snakes.Direction.Start)
                return;

            var head = snake.Points[0];
            var newHead = new Snakes.Point(head.X, head.Y);

            switch (snake.direction)
            {
                case Snakes.Direction.Up:
                    newHead.Y -= 20;
                    break;
                case Snakes.Direction.Down:
                    newHead.Y += 20;
                    break;
                case Snakes.Direction.Left:
                    newHead.X -= 20;
                    break;
                case Snakes.Direction.Right:
                    newHead.X += 20;
                    break;
            }

            snake.Points.Insert(0, newHead);
            snake.Points.RemoveAt(snake.Points.Count - 1);
        }

        public void ChangeDirection(Snakes.Direction newDirection)
        {
            var snake = MainWindow.mainWindow.ViewModelGames.SnakesPlayer;

            if ((newDirection == Snakes.Direction.Up && snake.direction != Snakes.Direction.Down) ||
                (newDirection == Snakes.Direction.Down && snake.direction != Snakes.Direction.Up) ||
                (newDirection == Snakes.Direction.Left && snake.direction != Snakes.Direction.Right) ||
                (newDirection == Snakes.Direction.Right && snake.direction != Snakes.Direction.Left))
            {
                snake.direction = newDirection;
            }
        }

        public bool CheckCollisions()
        {
            var snake = MainWindow.mainWindow.ViewModelGames.SnakesPlayer;
            var head = snake.Points[0];
            var apple = MainWindow.mainWindow.ViewModelGames.Points;

            if (Math.Abs(head.X - apple.X) < 20 && Math.Abs(head.Y - apple.Y) < 20)
            {
                var tail = snake.Points[snake.Points.Count - 1];
                snake.Points.Add(new Snakes.Point(tail.X, tail.Y));

                GenerateNewApple();
                return true;
            }

            for (int i = 1; i < snake.Points.Count; i++)
            {
                if (head.X == snake.Points[i].X && head.Y == snake.Points[i].Y)
                {
                    snake.GameOver = true;
                    return true;
                }
            }

            if (head.X < 0 || head.X > canvas.ActualWidth || head.Y < 0 || head.Y > canvas.ActualHeight)
            {
                snake.GameOver = true;
                return true;
            }

            return false;
        }

        private void GenerateNewApple()
        {
            var random = new Random();
            MainWindow.mainWindow.ViewModelGames.Points = new Snakes.Point
            {
                X = random.Next(1, (int)(canvas.ActualWidth / 20)) * 20,
                Y = random.Next(1, (int)(canvas.ActualHeight / 20)) * 20
            };
        }

        public void ClearCache()
        {
            _snakeElementsCache.Clear();
            canvas.Children.Clear();
        }
    }
}