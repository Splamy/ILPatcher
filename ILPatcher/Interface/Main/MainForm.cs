using ILPatcher.Utility;
using System.Windows.Forms;

namespace ILPatcher.Interface.Main
{
	public class MainForm : Form
	{
		#region Interface Elements
		SwooshPanel swooshPanel;
		#endregion

		public MainForm()
		{
			Text = "ILPatcher";

			NameCompressor.Instance.CheckUnique();

			swooshPanel = new SwooshPanel();
			this.Controls.Add(swooshPanel);
			swooshPanel.Dock = DockStyle.Fill;
			swooshPanel.PushPanel(new MainPanel(), "Overview");

			Size = new System.Drawing.Size(640, 480);
		}
	}
}
