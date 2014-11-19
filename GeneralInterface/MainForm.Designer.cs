namespace ILPatcher
{
	partial class MainForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.swooshPanel1 = new ILPatcher.SwooshPanel();
			this.SuspendLayout();
			// 
			// swooshPanel1
			// 
			this.swooshPanel1.BackColor = System.Drawing.SystemColors.Control;
			this.swooshPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.swooshPanel1.Location = new System.Drawing.Point(0, 0);
			this.swooshPanel1.Name = "swooshPanel1";
			this.swooshPanel1.Size = new System.Drawing.Size(644, 483);
			this.swooshPanel1.TabIndex = 14;
			this.swooshPanel1.Text = "swooshPanel1";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(644, 483);
			this.Controls.Add(this.swooshPanel1);
			this.Name = "Form1";
			this.Text = "C# Patcher";
			this.ResumeLayout(false);

		}

		#endregion

		private SwooshPanel swooshPanel1;
	}
}

