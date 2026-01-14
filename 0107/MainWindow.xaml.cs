using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace _0107
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();

        double score;
        int gravity = 8;
        bool gameOver;
        Rect flappyBirdHiBox;
        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += MainEventTimer;
            gameTimer.Interval = TimeSpan.FromMicroseconds(20);
            StartGame();

        }

		private void MainEventTimer(object sender, EventArgs e)
		{
			
		}

		private void KeyIsDown(object sender, KeyEventArgs e)
		{
            if (e.Key == Key.Space)
            {
                madar.RenderTransform = new RotateTransform(-20, madar.Width /2, madar.Height /2);
            }
		}

		private void KeyIsUp(object sender, KeyEventArgs e)
		{

		}

        private void StartGame()
        {
            MyCanvas.Focus();

            int temp = 300;

            score = 0;

            gameOver = false;
            Canvas.SetTop(madar, 190);

            foreach (var x in MyCanvas.Children.OfType<Image>())
            { 
                if((string)x.Tag == "oszlop1")
                {
                    Canvas.SetLeft(x, 500);
                }
				if ((string)x.Tag == "oszlop2")
				{
					Canvas.SetLeft(x, 800);
				}
				if ((string)x.Tag == "oszlop3")
				{
					Canvas.SetLeft(x, 1100);
				}
			}

            gameTimer.Start();

        }


        private void EndGame()
        {

        }

		//YouTube: Moo ICT: WPF C# Tutorial How to make a Flappy Bird Game in Visual Studio
        //8 perc 40 másodperctől kell majd folytatni
	}
}