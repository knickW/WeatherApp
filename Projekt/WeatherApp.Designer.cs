using System.Windows.Forms;

namespace Projekt
{
	partial class WeatherApp
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private TextBox cityNameTextBox;
		private RichTextBox resultLabel;

		/// <summary>
		/// Inicjalizuje komponenty formularza.
		/// </summary>
		private void InitializeComponent()
		{
			this.cityNameTextBox = new System.Windows.Forms.TextBox();
			this.resultLabel = new System.Windows.Forms.RichTextBox();
			this.submitButton = new System.Windows.Forms.Button();
			this.Response = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// cityNameTextBox
			// 
			this.cityNameTextBox.Location = new System.Drawing.Point(16, 15);
			this.cityNameTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.cityNameTextBox.Name = "cityNameTextBox";
			this.cityNameTextBox.Size = new System.Drawing.Size(265, 22);
			this.cityNameTextBox.TabIndex = 0;
			// 
			// resultLabel
			// 
			this.resultLabel.AutoSize = true;
			this.resultLabel.Location = new System.Drawing.Point(16, 49);
			this.resultLabel.Margin = new System.Windows.Forms.Padding(4);
			this.resultLabel.Name = "resultLabel";
			this.resultLabel.Size = new System.Drawing.Size(0, 15);
			this.resultLabel.TabIndex = 2;
			this.resultLabel.Text = "";
			// 
			// submitButton
			// 
			this.submitButton.Location = new System.Drawing.Point(293, 12);
			this.submitButton.Margin = new System.Windows.Forms.Padding(4);
			this.submitButton.Name = "submitButton";
			this.submitButton.Size = new System.Drawing.Size(100, 28);
			this.submitButton.TabIndex = 1;
			this.submitButton.Text = "Submit";
			this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
			// 
			// Response
			// 
			this.Response.Location = new System.Drawing.Point(16, 44);
			this.Response.Name = "Response";
			this.Response.Size = new System.Drawing.Size(618, 347);
			this.Response.TabIndex = 3;
			this.Response.Text = "";
			// 
			// WeatherApp
			// 
			this.ClientSize = new System.Drawing.Size(646, 403);
			this.Controls.Add(this.Response);
			this.Controls.Add(this.resultLabel);
			this.Controls.Add(this.cityNameTextBox);
			this.Controls.Add(this.submitButton);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "WeatherApp";
			this.Text = "Weather Form";
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		#endregion

		private Button submitButton;
		private RichTextBox Response;
	}
}