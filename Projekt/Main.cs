using System;
using System.Windows.Forms;

namespace Projekt
{
	public partial class Main : Form
	{
		private ResultsForm resultsForm;
		private WeatherApp weatherForm;

		public Main()
		{
			InitializeComponent();
		}

		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			StartGame();
		}

		private void StartGame()
		{
			Pong gameForm = new Pong();
			gameForm.Show();
			gameForm.Activate();
		}

		private void resultsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowResults();
		}

		private void ShowResults()
		{
			if (resultsForm == null || resultsForm.IsDisposed)
			{
				resultsForm = new ResultsForm();
				resultsForm.MdiParent = this;
				resultsForm.Show();
			}
			else
			{
				resultsForm.Activate();
			}
		}

		private void weatherToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowWeatherForm();
		}

		private void ShowWeatherForm()
		{
			if (weatherForm == null || weatherForm.IsDisposed)
			{
				weatherForm = new WeatherApp();
				weatherForm.MdiParent = this;
				weatherForm.Show();
			}
			else
			{
				weatherForm.Activate();
			}
		}
	}
}
