using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FlappyBird
{
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();

        double velocity = 0;
        double gravityForce = 0.5;
        double jumpForce = -8;

        bool gameStarted = false;
        int score = 0;

        Rectangle topPipe1;
        Rectangle bottomPipe1;

        Rectangle topPipe2;
        Rectangle bottomPipe2;

        Random rand = new Random();
        double pipeSpeed = 5;

        private readonly string scoresFilePath =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scores.txt");

        public enum GameMode
        {
            Normal,
            Nap,
            Ur
        }

        private GameMode currentMode = GameMode.Normal;

        private const double NormalGravity = 0.5;
        private const double NapGravity = 1.0;
        private const double UrGravity = 0.2;

        private const double NormalJump = -8;
        private const double NapJump = -10;
        private const double UrJump = -6;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameEngine;
        }

        private Brush MakeBackground(string packPath)
        {
            try
            {
                return new ImageBrush(new BitmapImage(new Uri(packPath, UriKind.Absolute)))
                {
                    Stretch = Stretch.Fill
                };
            }
            catch
            {
                return Brushes.LightBlue;
            }
        }

        private void ApplyModeSettings()
        {
            switch (currentMode)
            {
                case GameMode.Nap:
                    gravityForce = NapGravity;
                    jumpForce = NapJump;
                    canvas.Background = MakeBackground("pack://application:,,,/kepek/nap.jpg");
                    break;

                case GameMode.Ur:
                    gravityForce = UrGravity;
                    jumpForce = UrJump;
                    canvas.Background = MakeBackground("pack://application:,,,/kepek/ur.jpg");
                    break;

                default:
                    gravityForce = NormalGravity;
                    jumpForce = NormalJump;
                    canvas.Background = MakeBackground("pack://application:,,,/kepek/background.png");
                    break;
            }
        }

        private void StartGame()
        {
            score = 0;
            scoreText.Content = "Score: 0";
            velocity = 0;
            gameStarted = true;

            Canvas.SetTop(flappyBird, 196);
            Canvas.SetLeft(flappyBird, 40);

            ApplyModeSettings();

            
            szintekButton.Visibility = Visibility.Hidden;
            eredmenyekButton.Visibility = Visibility.Hidden;
            simaButton.Visibility = Visibility.Hidden;
            esosButton.Visibility = Visibility.Hidden;
            kodosButton.Visibility = Visibility.Hidden;
            VisszaMainButton.Visibility = Visibility.Hidden;
            VisszaAFőmenübe.Visibility = Visibility.Hidden;
            ScoreTextblock.Visibility = Visibility.Hidden;

            flappyBird.Visibility = Visibility.Visible;
            scoreText.Visibility = Visibility.Visible;

            RemoveOldPipes();
            CreatePipes();

            canvas.Focus();
            gameTimer.Start();
        }

        private void GameEngine(object? sender, EventArgs e)
        {
            velocity += gravityForce;
            Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + velocity);

            Rect birdHitBox = new Rect(
                Canvas.GetLeft(flappyBird),
                Canvas.GetTop(flappyBird),
                flappyBird.Width,
                flappyBird.Height);

            void UpdatePipePair(Rectangle topPipe, Rectangle bottomPipe)
            {
                Canvas.SetLeft(topPipe, Canvas.GetLeft(topPipe) - pipeSpeed);
                Canvas.SetLeft(bottomPipe, Canvas.GetLeft(bottomPipe) - pipeSpeed);

                Rect topPipeHitBox = new Rect(
                    Canvas.GetLeft(topPipe),
                    Canvas.GetTop(topPipe),
                    topPipe.Width,
                    topPipe.Height);

                Rect bottomPipeHitBox = new Rect(
                    Canvas.GetLeft(bottomPipe),
                    Canvas.GetTop(bottomPipe),
                    bottomPipe.Width,
                    bottomPipe.Height);

                if (birdHitBox.IntersectsWith(topPipeHitBox) ||
                    birdHitBox.IntersectsWith(bottomPipeHitBox))
                {
                    GameOver();
                    return;
                }

                if (Canvas.GetLeft(topPipe) + topPipe.Width < 0)
                {
                    score++;
                    scoreText.Content = "Score: " + score;

                    double gap = 140;
                    double minTopHeight = 50;
                    double maxTopHeight = 250;
                    double topHeight = rand.Next((int)minTopHeight, (int)maxTopHeight);

                    double startX = canvas.Width + 200;

                    Canvas.SetLeft(topPipe, startX);
                    topPipe.Height = topHeight;
                    Canvas.SetTop(topPipe, 0);

                    Canvas.SetLeft(bottomPipe, startX);
                    bottomPipe.Height = canvas.Height - topHeight - gap;
                    Canvas.SetTop(bottomPipe, topHeight + gap);
                }
            }

            if (topPipe1 != null && bottomPipe1 != null)
            {
                UpdatePipePair(topPipe1, bottomPipe1);
            }

            if (topPipe2 != null && bottomPipe2 != null)
            {
                UpdatePipePair(topPipe2, bottomPipe2);
            }

            if (Canvas.GetTop(flappyBird) > canvas.Height || Canvas.GetTop(flappyBird) < -50)
            {
                GameOver();
            }
        }

        private void CreatePipePair(ref Rectangle topPipe, ref Rectangle bottomPipe, double startX)
        {
            double gap = 140;
            double minTopHeight = 50;
            double maxTopHeight = 250;
            double topHeight = rand.Next((int)minTopHeight, (int)maxTopHeight);

            topPipe = new Rectangle
            {
                Width = 80,
                Height = topHeight,
                Fill = Brushes.Green
            };

            bottomPipe = new Rectangle
            {
                Width = 80,
                Height = canvas.Height - topHeight - gap,
                Fill = Brushes.Green
            };

            Canvas.SetLeft(topPipe, startX);
            Canvas.SetTop(topPipe, 0);

            Canvas.SetLeft(bottomPipe, startX);
            Canvas.SetTop(bottomPipe, topHeight + gap);

            canvas.Children.Add(topPipe);
            canvas.Children.Add(bottomPipe);
        }

        private void CreatePipes()
        {
            double startX1 = canvas.Width;
            double startX2 = canvas.Width + 300;

            CreatePipePair(ref topPipe1, ref bottomPipe1, startX1);
            CreatePipePair(ref topPipe2, ref bottomPipe2, startX2);
        }

        private void RemoveOldPipes()
        {
            if (topPipe1 != null)
            {
                canvas.Children.Remove(topPipe1);
                topPipe1 = null;
            }
            if (bottomPipe1 != null)
            {
                canvas.Children.Remove(bottomPipe1);
                bottomPipe1 = null;
            }

            if (topPipe2 != null)
            {
                canvas.Children.Remove(topPipe2);
                topPipe2 = null;
            }
            if (bottomPipe2 != null)
            {
                canvas.Children.Remove(bottomPipe2);
                bottomPipe2 = null;
            }
        }

        private void SaveScore(int s)
        {
            try
            {
                File.AppendAllLines(scoresFilePath, new[] { s.ToString() });
            }
            catch
            {
                // file hiba ignorálása
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            gameStarted = false;

            RemoveOldPipes();

            SaveScore(score);

            szintekButton.Visibility = Visibility.Visible;
            eredmenyekButton.Visibility = Visibility.Visible;

            simaButton.Visibility = Visibility.Hidden;
            esosButton.Visibility = Visibility.Hidden;
            kodosButton.Visibility = Visibility.Hidden;
            VisszaMainButton.Visibility = Visibility.Hidden;
            VisszaAFőmenübe.Visibility = Visibility.Hidden;
            ScoreTextblock.Visibility = Visibility.Hidden;

            flappyBird.Visibility = Visibility.Visible;
            scoreText.Visibility = Visibility.Visible;

            MessageBox.Show("Vége a játéknak! Pontszám: " + score);
        }

        private void szintekButton_Click(object sender, RoutedEventArgs e)
        {
            szintekButton.Visibility = Visibility.Hidden;
            eredmenyekButton.Visibility = Visibility.Hidden;
            simaButton.Visibility = Visibility.Visible;
            esosButton.Visibility = Visibility.Visible;
            kodosButton.Visibility = Visibility.Visible;
            VisszaMainButton.Visibility = Visibility.Visible;
            ScoreTextblock.Visibility = Visibility.Hidden;
            VisszaAFőmenübe.Visibility = Visibility.Hidden;
            flappyBird.Visibility = Visibility.Visible;
            scoreText.Visibility = Visibility.Visible;
        }

        private void VisszaMainButton_Click(object sender, RoutedEventArgs e)
        {
            szintekButton.Visibility = Visibility.Visible;
            eredmenyekButton.Visibility = Visibility.Visible;
            simaButton.Visibility = Visibility.Hidden;
            esosButton.Visibility = Visibility.Hidden;
            kodosButton.Visibility = Visibility.Hidden;
            VisszaMainButton.Visibility = Visibility.Hidden;
            ScoreTextblock.Visibility = Visibility.Hidden;
            VisszaAFőmenübe.Visibility = Visibility.Hidden;
            flappyBird.Visibility = Visibility.Visible;
            scoreText.Visibility = Visibility.Visible;

            gameTimer.Stop();
            gameStarted = false;
        }

        private void VisszaAFőmenübe_Click(object sender, RoutedEventArgs e)
        {
            szintekButton.Visibility = Visibility.Visible;
            eredmenyekButton.Visibility = Visibility.Visible;
            simaButton.Visibility = Visibility.Hidden;
            esosButton.Visibility = Visibility.Hidden;
            kodosButton.Visibility = Visibility.Hidden;
            VisszaMainButton.Visibility = Visibility.Hidden;
            ScoreTextblock.Visibility = Visibility.Hidden;
            VisszaAFőmenübe.Visibility = Visibility.Hidden;
            flappyBird.Visibility = Visibility.Visible;
            scoreText.Visibility = Visibility.Visible;

            gameTimer.Stop();
            gameStarted = false;
        }

        private void eredmenyekButton_Click(object sender, RoutedEventArgs e)
        {
            szintekButton.Visibility = Visibility.Hidden;
            eredmenyekButton.Visibility = Visibility.Hidden;
            VisszaAFőmenübe.Visibility = Visibility.Visible;
            ScoreTextblock.Visibility = Visibility.Visible;
            flappyBird.Visibility = Visibility.Hidden;
            scoreText.Visibility = Visibility.Hidden;

            gameTimer.Stop();
            gameStarted = false;

            ScoreTextblock.Text = "Eddig elért eredmények:\n";

            try
            {
                if (File.Exists(scoresFilePath))
                {
                    var scores = File.ReadAllLines(scoresFilePath)
                        .Select(line =>
                        {
                            int v;
                            return int.TryParse(line, out v) ? v : (int?)null;
                        })
                        .Where(v => v.HasValue)
                        .Select(v => v.Value)
                        .OrderByDescending(v => v)
                        .ToList();

                    int rank = 1;
                    foreach (var s in scores)
                    {
                        ScoreTextblock.Text += $"{rank}. {s}\n";
                        rank++;
                    }

                    if (scores.Count == 0)
                    {
                        ScoreTextblock.Text += "Nincs még mentett pontszám.";
                    }
                }
                else
                {
                    ScoreTextblock.Text += "Nincs még mentett pontszám.";
                }
            }
            catch
            {
                ScoreTextblock.Text += "Hiba történt a pontszámok betöltésekor.";
            }
        }

        private void simaButton_Click(object sender, RoutedEventArgs e)
        {
            ScoreTextblock.Visibility = Visibility.Hidden;
            VisszaAFőmenübe.Visibility = Visibility.Hidden;

            currentMode = GameMode.Normal;
            StartGame();
        }

        private void esosButton_Click(object sender, RoutedEventArgs e)
        {
            ScoreTextblock.Visibility = Visibility.Hidden;
            VisszaAFőmenübe.Visibility = Visibility.Hidden;

            currentMode = GameMode.Nap;
            StartGame();
        }

        private void kodosButton_Click(object sender, RoutedEventArgs e)
        {
            ScoreTextblock.Visibility = Visibility.Hidden;
            VisszaAFőmenübe.Visibility = Visibility.Hidden;

            currentMode = GameMode.Ur;
            StartGame();
        }

        private void Canvas_KeyisDown(object sender, KeyEventArgs e)
        {
            if (!gameStarted) return;

            if (e.Key == Key.Space)
            {
                flappyBird.RenderTransform =
                    new RotateTransform(-20, flappyBird.Width / 2, flappyBird.Height / 2);
                velocity = jumpForce;
            }
        }

        private void Canvas_KeyisUp(object sender, KeyEventArgs e)
        {
            if (!gameStarted) return;

            flappyBird.RenderTransform =
                new RotateTransform(5, flappyBird.Width / 2, flappyBird.Height / 2);
        }
    }
}
