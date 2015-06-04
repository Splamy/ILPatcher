using ILPatcher.Utility;
using System.Windows.Forms;

namespace ILPatcher.Interface.Main
{
	public partial class MainForm : Form
	{
		private MainForm()
		{
			InitializeComponent();

			NameCompressor.Instance.CheckUnique();

			swooshPanel1.PushPanel(new MainPanel(), "Overview");
		}

		private static MainForm _Instance;
		public static MainForm Instance
		{
			get { if (_Instance == null) _Instance = new MainForm(); return _Instance; }
			protected set { _Instance = value; }
		}
	}
}
