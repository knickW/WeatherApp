using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Projekt
{
	/// <summary>
	/// Główna klasa gry Pong.
	/// </summary>
	public partial class Pong : Form
	{
		private const int PaddleWidth = 15;
		private const int PaddleHeight = 100;
		private const int BallSize = 20;

		private int paddleSpeed = 8;
		private int initialBallSpeed = 3;
		private int maxBallSpeed = 10;
		private int playerScore = 0;
		private int computerScore = 0;
		private int level = 1;

		private Point paddlePlayer = new Point(20, 200);
		private Point paddleComputer = new Point(565, 200);
		private Point ballPosition;
		private Point ballDirection;

		private Timer gameTimer;
		private Random random = new Random();
		private ResultsForm resultsForm;
		private string playerName = "";

		/// <summary>
		/// Konstruktor klasy Pong.
		/// </summary>
		public Pong()
		{
			InitializeComponent();
			InitializeGame();
			this.FormClosing += Pong_FormClosing;
		}

		/// <summary>
		/// Inicjalizacja ustawień gry.
		/// </summary>
		private void InitializeGame()
		{
			this.Size = new Size(600, 400);
			this.MaximumSize = new Size(800, 600);
			this.MinimumSize = new Size(400, 200);
			this.Text = "Pong Game";
			this.FormBorderStyle = FormBorderStyle.FixedSingle;

			gameTimer = new Timer();
			gameTimer.Interval = 10;
			gameTimer.Tick += GameTimer_Tick;
			gameTimer.Start();

			this.KeyDown += Pong_KeyDown;

			ResetGame();
		}

		/// <summary>
		/// Poproszenie gracza o wpisanie imienia.
		/// </summary>
		private string PromptForPlayerName()
		{
			string input = "";
			// Wyświetl MessageBox z polem do wpisania imienia
			if (InputBox("Wprowadź imię", "Podaj swoje imię:", ref input) == DialogResult.OK)
			{
				return input;
			}
			return "Anonim";
		}

		/// <summary>
		/// Obsługa zdarzenia tick timera gry.
		/// </summary>
		private void GameTimer_Tick(object sender, EventArgs e)
		{
			MovePaddle();
			MoveBall();
			CheckCollisions();
			CheckWinConditions();
			Invalidate();
		}

		/// <summary>
		/// Poruszanie się paletkami.
		/// </summary>
		private void MovePaddle()
		{
			if (GetKeyState(Keys.Up) < 0 && paddlePlayer.Y > 0)
			{
				paddlePlayer.Y -= paddleSpeed;
			}

			if (GetKeyState(Keys.Down) < 0 && paddlePlayer.Y < this.ClientSize.Height - PaddleHeight)
			{
				paddlePlayer.Y += paddleSpeed;
			}

			// Dodanie losowego błędu w poruszaniu się paletki komputera
			int computerError = random.Next(0, 101);
			// Dodanie zwiększania poziomu trudności co poziom gry
			int levelError = computerError / level;

			// Określenie maksymalnej szansy na błąd w zależności od poziomu
			int maxErrorChance = Math.Max(50 - level, 5);

			if (computerError < maxErrorChance)
			{
				int errorAmount = random.Next(1, 6);
				paddleComputer.Y += errorAmount;

				// Dodanie sporadycznego złego ruchu w złym kierunku
				int badMoveChance = Math.Max(10 - level, 1);
				int badMoveError = random.Next(0, 101);

				if (badMoveError < badMoveChance)
				{
					// Zły ruch - zmiana kierunku
					int badMoveAmount = random.Next(-3, 3);
					paddleComputer.Y += badMoveAmount;
				}
			}

			if (ballPosition.Y < paddleComputer.Y && paddleComputer.Y > 0)
			{
				paddleComputer.Y -= paddleSpeed;
			}
			else if (ballPosition.Y > paddleComputer.Y && paddleComputer.Y < this.ClientSize.Height - PaddleHeight)
			{
				paddleComputer.Y += paddleSpeed;
			}
		}

		/// <summary>
		/// Poruszanie się piłki.
		/// </summary>
		private void MoveBall()
		{
			ballPosition.X += ballDirection.X * initialBallSpeed;
			ballPosition.Y += ballDirection.Y * initialBallSpeed;

			if (ballPosition.Y < 0 || ballPosition.Y > this.ClientSize.Height - BallSize)
			{
				ballDirection.Y *= -1;
			}

			if (ballPosition.X < paddlePlayer.X + PaddleWidth &&
				ballPosition.Y > paddlePlayer.Y &&
				ballPosition.Y < paddlePlayer.Y + PaddleHeight)
			{
				ballDirection.X *= -1;
				IncreaseBallSpeed();
			}

			if (ballPosition.X > paddleComputer.X - BallSize &&
				ballPosition.Y > paddleComputer.Y &&
				ballPosition.Y < paddleComputer.Y + PaddleHeight)
			{
				ballDirection.X *= -1;
				IncreaseBallSpeed();
			}

			if (ballPosition.X < 0)
			{
				ComputerScores();
			}
			else if (ballPosition.X > this.ClientSize.Width - BallSize)
			{
				PlayerScores();
			}
		}

		/// <summary>
		/// Zwiększa prędkość piłki po odbiciu.
		/// </summary>
		private void IncreaseBallSpeed()
		{
			initialBallSpeed = 3 + level; // Prędkość zależy od poziomu
		}

		/// <summary>
		/// Sprawdza kolizje z krawędziami ekranu.
		/// </summary>
		private void CheckCollisions()
		{
			if (ballPosition.X < 0)
			{
				ballPosition.X = 0;
				ballDirection.X *= -1;
			}
			else if (ballPosition.X > this.ClientSize.Width - BallSize)
			{
				ballPosition.X = this.ClientSize.Width - BallSize;
				ballDirection.X *= -1;
			}
		}

		/// <summary>
		/// Sprawdza warunki zwycięstwa.
		/// </summary>
		private void CheckWinConditions()
		{
			if (playerScore == 5 || computerScore == 5)
			{
				if (playerScore == 5)
				{
					level++;
					ResetBall();
					playerScore = 0;
					computerScore = 0;
				}
				else
				{
					level = 1;
					playerScore = 0;
					computerScore = 0;
					DialogResult gameOverResult = ShowGameOverDialog();

					if (gameOverResult == DialogResult.Yes)
					{
						ResetGame();
					}
					else
					{
						this.Close();
					}
				}
			}
		}

		/// <summary>
		/// Obsługa zdobycia punktu przez gracza.
		/// </summary>
		private void PlayerScores()
		{
			playerScore++;
			UpdateScore();
			ResetBall();
		}

		/// <summary>
		/// Obsługa zdobycia punktu przez komputer.
		/// </summary>
		private void ComputerScores()
		{
			computerScore++;
			UpdateScore();
			ResetBall();
		}

		/// <summary>
		/// Aktualizuje wyświetlany wynik.
		/// </summary>
		private void UpdateScore()
		{
			this.Text = $"Pong Game - Poziom {level} | Gracz: {playerScore} | Komputer: {computerScore}";
		}

		/// <summary>
		/// Resetuje ustawienia gry.
		/// </summary>
		private void ResetGame()
		{
			level = 1;
			playerScore = 0;
			computerScore = 0;
			initialBallSpeed = 3;
			playerName = PromptForPlayerName();
			ResetBall();
			UpdateScore();
		}

		/// <summary>
		/// Resetuje pozycję piłki.
		/// </summary>
		private void ResetBall()
		{
			ballPosition = new Point(this.ClientSize.Width / 2 - BallSize / 2, this.ClientSize.Height / 2 - BallSize / 2);
			ballDirection = new Point(random.Next(0, 2) == 0 ? -1 : 1, random.Next(0, 2) == 0 ? -1 : 1);
		}

		/// <summary>
		/// Zapisuje wynik poziomu do pliku CSV.
		/// </summary>
		private void SaveLevelResult()
		{
			string fileName = "wyniki.csv";
			List<string> lines = new List<string>();

			if (File.Exists(fileName))
			{
				lines.AddRange(File.ReadAllLines(fileName));
			}

			lines.Add($"{level},{playerName},{playerScore},{computerScore}");

			File.WriteAllLines(fileName, lines);
		}

		/// <summary>
		/// Obsługuje zdarzenie rysowania na formie.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.FillRectangle(Brushes.Blue, new Rectangle(paddlePlayer, new Size(PaddleWidth, PaddleHeight)));
			e.Graphics.FillRectangle(Brushes.Red, new Rectangle(paddleComputer, new Size(PaddleWidth, PaddleHeight)));
			e.Graphics.FillEllipse(Brushes.Green, new Rectangle(ballPosition, new Size(BallSize, BallSize)));
		}

		/// <summary>
		/// Pobiera stan klawisza.
		/// </summary>
		private short GetKeyState(Keys key)
		{
			return (short)((GetAsyncKeyState((int)key) & 0x8001) >> 8);
		}

		/// <summary>
		/// Obsługuje zdarzenie naciśnięcia klawisza.
		/// </summary>
		public void Pong_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Up:
					if (paddlePlayer.Y > 0)
					{
						paddlePlayer.Y -= paddleSpeed;
					}
					break;

				case Keys.Down:
					if (paddlePlayer.Y < this.ClientSize.Height - PaddleHeight)
					{
						paddlePlayer.Y += paddleSpeed;
					}
					break;
				case Keys.Escape:
					ShowResultDialog();
					break;
			}
		}

		/// <summary>
		/// Pobiera asynchroniczny stan klawisza.
		/// </summary>
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int vKey);

		/// <summary>
		/// Wyświetla okno dialogowe z wynikiem gry.
		/// </summary>
		private void ShowResultDialog()
		{
			// Tworzymy nową instancję ResultsForm
			ResultsForm resultsForm = new ResultsForm();

			// Ustawiamy wyniki w oknie dialogowym
			resultsForm.SetResults($"{level},{playerName},{playerScore},{computerScore}");

			// Wyświetlamy okno dialogowe i sprawdzamy, czy użytkownik nacisnął OK
			if (resultsForm.ShowDialog() == DialogResult.OK)
			{
				// Jeśli OK, pobieramy nową nazwę gracza i resetujemy grę
				playerName = resultsForm.PlayerName;
				ResetGame();
			}
			else
			{
				// Jeśli użytkownik anuluje, zamykamy aplikację
				Application.Exit();
			}
		}

		/// <summary>
		/// Wyświetla MessageBox z polem do wpisania tekstu.
		/// </summary>
		private DialogResult InputBox(string title, string promptText, ref string value)
		{
			Form form = new Form();
			Label label = new Label();
			TextBox textBox = new TextBox();
			Button buttonOk = new Button();
			Button buttonCancel = new Button();

			form.Text = title;
			label.Text = promptText;
			textBox.Text = value;

			buttonOk.Text = "OK";
			buttonCancel.Text = "Cancel";
			buttonOk.DialogResult = DialogResult.OK;
			buttonCancel.DialogResult = DialogResult.Cancel;

			label.SetBounds(9, 20, 372, 13);
			textBox.SetBounds(12, 36, 372, 20);
			buttonOk.SetBounds(228, 72, 75, 23);
			buttonCancel.SetBounds(309, 72, 75, 23);

			label.AutoSize = true;
			textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
			buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			buttonOk.Click += (sender, e) => { form.Close(); };
			buttonCancel.Click += (sender, e) => { form.Close(); };

			form.ClientSize = new Size(396, 107);
			form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
			form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.AcceptButton = buttonOk;
			form.CancelButton = buttonCancel;

			DialogResult dialogResult = form.ShowDialog();
			value = textBox.Text;
			return dialogResult;
		}

		/// <summary>
		/// Deinicjalizacja gry.
		/// </summary>
		private void DeinitializeGame()
		{
			gameTimer.Stop();  // Zatrzymaj timer
			gameTimer.Dispose();  // Zwolnij timer
		}


		/// <summary>
		/// Zdarzenie wywoływane przed zamknięciem okna.
		/// </summary>
		private void Pong_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveLevelResult();
			DeinitializeGame();
		}

		/// <summary>
		/// Okno końca gry
		/// </summary>
		/// <returns>result DialogResult</returns>
		private DialogResult ShowGameOverDialog()
		{
			DialogResult result = MessageBox.Show("Przegrałeś. Zagrać jeszcze raz?", "Koniec gry", MessageBoxButtons.YesNo);
			return result;
		}

		/// <summary>
		/// Destruktor klasy Pong.
		/// </summary>
		~Pong()
		{
			SaveLevelResult();
			DeinitializeGame();
		}
	}
}
