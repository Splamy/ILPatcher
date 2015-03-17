using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Reflection;
using System.Diagnostics;
using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
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
