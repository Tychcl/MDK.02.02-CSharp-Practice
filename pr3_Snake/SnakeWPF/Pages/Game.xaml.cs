using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace SnakeWPF.Pages
{
    public partial class Game : Page
    {
        // Кэшированные ресурсы
        private readonly SolidColorBrush _headBrush = new SolidColorBrush(Color.FromRgb(31, 71, 15));
        private readonly SolidColorBrush _bodyBrush = new SolidColorBrush(Color.FromRgb(57, 99, 41));
        private readonly ImageBrush _appleBrush;

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

        public async void CreateUI()
        {
            try
            {
                int _d = MainWindow.mainWindow.d;
                if (_d <= 0) _d = 1;

                var previousState = MainWindow.mainWindow._ViewModelGames;
                var currentState = MainWindow.mainWindow.ViewModelGames;
                var previousOthers = MainWindow.mainWindow._ViewModelGamesList ?? new List<ViewModelGames>();
                var currentOthers = MainWindow.mainWindow.ViewModelGamesList ?? new List<ViewModelGames>();

                if (previousState == null || currentState == null)
                    return;

                int delay = Math.Max(1, MainWindow.mainWindow._sleep / (_d + 1));

                await Dispatcher.InvokeAsync(async () =>
                {
                    canvas.Children.Clear();
                    RenderApple();

                    for (int frame = 0; frame <= _d; frame++)
                    {
                        canvas.Children.Clear();
                        RenderApple();

                        var interpolatedSnake = InterpolateSnake(
                            previousState.SnakesPlayer,
                            currentState.SnakesPlayer,
                            frame, _d);
                        RenderSnake(interpolatedSnake);

                        for (int i = 0; i < Math.Min(previousOthers.Count, currentOthers.Count); i++)
                        {
                            var interpolatedOther = InterpolateSnake(
                                previousOthers[i].SnakesPlayer,
                                currentOthers[i].SnakesPlayer,
                                frame, _d);
                            RenderSnake(interpolatedOther);
                        }

                        if (frame < _d)
                        {
                            await Task.Delay(delay);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateUI: {ex.Message}");
            }
        }

        private Snakes InterpolateSnake(Snakes previous, Snakes current, int currentFrame, int totalFrames)
        {
            if (previous?.Points == null || current?.Points == null)
                return current ?? previous;

            if (previous.Points.Count != current.Points.Count)
                return current;

            var interpolated = new Snakes
            {
                Points = new List<Snakes.Point>(),
                direction = current.direction,
                GameOver = current.GameOver
            };

            float t = (float)currentFrame / totalFrames;
            t = Math.Max(0, Math.Min(1, t)); // Ограничиваем от 0 до 1

            for (int i = 0; i < previous.Points.Count; i++)
            {
                var prevPoint = previous.Points[i];
                var currPoint = current.Points[i];

                var interpolatedPoint = new Snakes.Point
                {
                    X = (int)(prevPoint.X + (currPoint.X - prevPoint.X) * t),
                    Y = (int)(prevPoint.Y + (currPoint.Y - prevPoint.Y) * t)
                };

                interpolated.Points.Add(interpolatedPoint);
            }

            return interpolated;
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
    }
}