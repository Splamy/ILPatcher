using ILPatcher.Utility;
using System.Windows.Forms;
using System;

namespace ILPatcher.Interface.Main
{
	public class MainForm : Form
	{
		#region Interface Elements
		Swoosh swoosh;
		#endregion

		public MainForm()
		{
			Text = "ILPatcher";

			NameCompressor.Instance.CheckUnique();

			swoosh = new Swoosh();
			this.Controls.Add(swoosh);
			swoosh.Dock = DockStyle.Fill;
			swoosh.PushPanel(new MainPanel(), "Overview");

			Size = new System.Drawing.Size(640, 480);
		}
	}
}
