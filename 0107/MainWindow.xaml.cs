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
using System.IO;

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
        Rect flappyBirdHitBox;
		bool rainActive = false;
		int fogTimer = 0;
		bool rainEnabled = false;
		bool fogEnabled = false;
		List<int> highScores = new List<int>();

		


		



		enum GameMode
		{
			Normal,
			Rain,
			Fog
		}

		GameMode currentMode;



		int normalJump = -8;
		int rainJump = -4;

		private void NewGame_Click(object sender, RoutedEventArgs e)
		{
			StartScreen.Visibility = Visibility.Hidden;
			ModeScreen.Visibility = Visibility.Visible;
		}

		private void NormalMode_Click(object sender, RoutedEventArgs e)
		{
			StartSelectedGame(GameMode.Normal);
		}

		private void RainMode_Click(object sender, RoutedEventArgs e)
		{
			StartSelectedGame(GameMode.Rain);
		}

		private void FogMode_Click(object sender, RoutedEventArgs e)
		{
			StartSelectedGame(GameMode.Fog);
		}

		private void StartSelectedGame(GameMode mode)
		{
			currentMode = mode;

			ModeScreen.Visibility = Visibility.Hidden;
			GameScreen.Visibility = Visibility.Visible;

			rainEnabled = (mode == GameMode.Rain);
			fogEnabled = (mode == GameMode.Fog);

			StartGame();
		}


		public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += MainEventTimer;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            StartGame();
			LoadScores();


		}

		private void MainEventTimer(object sender, EventArgs e)
		{
			txtScore.Content= "Score: " + score;

			rainActive = false;

			flappyBirdHitBox = new Rect(Canvas.GetLeft(madar), Canvas.GetTop(madar), madar.Width, madar.Height);

            Canvas.SetTop(madar, Canvas.GetTop(madar) + gravity);

            if (Canvas.GetTop(madar) < -10 || Canvas.GetTop(madar) > 475)
            {
                EndGame();
            }

            foreach(var x in MyCanvas.Children.OfType<Image>())
            {
                if((string)x.Tag == "oszlop1" | (string)x.Tag == "oszlop2" | (string)x.Tag == "oszlop3")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) -5);

                    if (Canvas.GetLeft(x) < -100)
                    {
                        Canvas.SetLeft(x, 800);

                        score += 0.5;
                    }

                    Rect pipeHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (flappyBirdHitBox.IntersectsWith(pipeHitBox))
                    {
                        EndGame();
                    }
				}

				if (rainEnabled)
				{
					foreach (var r in MyCanvas.Children.OfType<Rectangle>())
					{
						if ((string)r.Tag == "rain")
						{
							Canvas.SetTop(r, Canvas.GetTop(r) + 10);

							if (Canvas.GetTop(r) > 500)
							{
								Canvas.SetTop(r, -100);
							}

							Rect rainHitBox = new Rect(
								Canvas.GetLeft(r),
								Canvas.GetTop(r),
								r.Width,
								r.Height
							);

							if (flappyBirdHitBox.IntersectsWith(rainHitBox))
							{
								rainActive = true;
							}

						}
					}
				}

				if (fogEnabled)
				{

					fogLayer.Visibility = fogEnabled ? Visibility.Visible : Visibility.Hidden;

					fogTimer++;

					if (fogTimer > 300 && fogTimer < 600)
					{
						fogLayer.Visibility = Visibility.Visible;
					}
					else
					{
						fogLayer.Visibility = Visibility.Hidden;
					}

					if (fogTimer > 600)
					{
						fogTimer = 0;
					}
				}

				highScores.Add((int)score);
				highScores = highScores
					.OrderByDescending(x => x)
					.Take(5)
					.ToList();

				SaveScores();

			}
		}

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
				madar.RenderTransform = new RotateTransform(-20);

				gravity = -8;
            }
            if (e.Key == Key.R && gameOver == true)
            {
                StartGame();
            }
			if (e.Key == Key.Space)
			{
				madar.RenderTransform = new RotateTransform(-20);

				gravity = rainActive ? rainJump : normalJump;
			}

		}

		private void KeyIsUp(object sender, KeyEventArgs e)
		{
			madar.RenderTransform = new RotateTransform(5);

			gravity = 8;
		}

        private void StartGame()
        {
			vegeszoveg.Visibility = Visibility.Hidden;

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

		void SaveScores()
		{
			File.WriteAllLines("scores.txt", highScores.Select(x => x.ToString()));
		}

		void LoadScores()
		{
			if (File.Exists("scores.txt"))
			{
				highScores = File.ReadAllLines("scores.txt")
								 .Select(int.Parse)
								 .ToList();
			}
		}

		private void Leaderboard_Click(object sender, RoutedEventArgs e)
		{
			StartScreen.Visibility = Visibility.Hidden;
			LeaderboardScreen.Visibility = Visibility.Visible;

			LeaderboardList.Items.Clear();
			foreach (var s in highScores)
				LeaderboardList.Items.Add(s);
		}

		private void BackToMenu_Click(object sender, RoutedEventArgs e)
		{
			StartScreen.Visibility = Visibility.Visible;
			ModeScreen.Visibility = Visibility.Hidden;
			GameScreen.Visibility = Visibility.Hidden;
			LeaderboardScreen.Visibility = Visibility.Hidden;
		}

		private void BackToMenuFromGame_Click(object sender, RoutedEventArgs e)
		{
			// Elrejtjük a GameScreen-et
			GameScreen.Visibility = Visibility.Hidden;

			// Vissza a kezdőképernyőre
			StartScreen.Visibility = Visibility.Visible;

			// Reset gomb + Game Over felirat
			BackToMenuButton.Visibility = Visibility.Hidden;
			vegeszoveg.Visibility = Visibility.Hidden;

			// Reset a játékállapotot, ha újra játszanak
			rainEnabled = false;
			fogEnabled = false;
		}


		private void EndGame()
        {
            gameTimer.Stop();
            gameOver = true;
            vegeszoveg.Content = " Game Over";
			vegeszoveg.Visibility = Visibility.Visible;
			BackToMenuButton.Visibility = Visibility.Visible;
		}

		//YouTube: Moo ICT: WPF C# Tutorial How to make a Flappy Bird Game in Visual Studio
		//hibákat javítani kell
	}
}