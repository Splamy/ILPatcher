using ILPatcher.Utility;
using System.Windows.Forms;

namespace ILPatcher.Interface.Main
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			NameCompressor.Instance.CheckUnique();

			swooshPanel1.PushPanel(new MainPanel(), "Overview");
		}
	}
}
