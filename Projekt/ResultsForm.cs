using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Projekt
{
	public partial class ResultsForm : Form
	{
		private DataGridView dataGridViewResults; // Dodana deklaracja DataGridView
		public string PlayerName { get; private set; }

		public ResultsForm()
		{
			InitializeComponent();
			InitializeDataGridView();
			LoadResults();
		}


		// Dodana metoda do inicjalizacji DataGridView
		private void InitializeDataGridView()
		{
			dataGridViewResults = new DataGridView();
			dataGridViewResults.Dock = DockStyle.Fill;
			dataGridViewResults.ReadOnly = true;
			dataGridViewResults.AllowUserToAddRows = false;
			dataGridViewResults.AllowUserToDeleteRows = false;
			dataGridViewResults.AllowUserToResizeRows = false;
			dataGridViewResults.RowHeadersVisible = false;

			// Dodaj kolumny do DataGridView
			dataGridViewResults.Columns.Add("Level", "Poziom");
			dataGridViewResults.Columns.Add("PlayerScore", "Gracz");
			dataGridViewResults.Columns.Add("ComputerScore", "Wynik Komputera");

			// Dodaj DataGridView do kontrolki formularza
			Controls.Add(dataGridViewResults);
		}

		private void LoadResults()
		{
			string fileName = "wyniki.csv";

			if (File.Exists(fileName))
			{
				List<string> lines = new List<string>(File.ReadAllLines(fileName));

				foreach (var line in lines)
				{
					string[] values = line.Split(',');
					dataGridViewResults.Rows.Add(values);
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			string fileName = "wyniki.csv";
			List<string> lines = new List<string>();

			foreach (DataGridViewRow row in dataGridViewResults.Rows)
			{
				List<string> values = new List<string>();

				foreach (DataGridViewCell cell in row.Cells)
				{
					values.Add(cell.Value.ToString());
				}

				lines.Add(string.Join(",", values));
			}

			File.WriteAllLines(fileName, lines);
			MessageBox.Show("Wyniki zostały zapisane.");
		}
		public void SetResults(string result)
		{
			PlayerName = result.Split(',')[1];
			dataGridViewResults.Rows.Add(result.Split(','));
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
