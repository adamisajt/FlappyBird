using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FlappyBird
{
    public partial class MainWindow : Window
    {
        readonly DispatcherTimer _ido = new DispatcherTimer();

        double _Y;          
        double velocity;      
        const double gravitacio = 0.5; 
        const double ugrassebesseg = -8; 

        public MainWindow()
        {
            InitializeComponent();

            _Y = 150;
            Canvas.SetLeft(Bird, 150);
            Canvas.SetTop(Bird, _Y);

            _ido.Interval = TimeSpan.FromMilliseconds(16); 
            _ido.Tick += GameLoop;
            _ido.Start();
        }

        void GameLoop(object? sender, EventArgs e)
        {
            velocity += gravitacio;

            _Y += velocity;

            double minY = 0;
            double maxY = GameCanvas.ActualHeight - Bird.ActualHeight;
            if (maxY <= 0) maxY = 400; 

            if (_Y < minY)
            {
                _Y = minY;
                velocity = 0;
            }
            else if (_Y > maxY)
            {
                _Y = maxY;
                velocity = 0;
            }

            Canvas.SetTop(Bird, _Y);
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                velocity = ugrassebesseg; 
            }
        }
    }
}