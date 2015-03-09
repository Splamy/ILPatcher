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

			swooshPanel1.AddPanel(MainPanel.Instance, "Overview");
			swooshPanel1.AddPanel(EditorEntry.Instance, "Patch Entry");
			swooshPanel1.AddPanel(EditorILPattern.Instance, "Patch Part");
			//swooshPanel1.AddPanel(new TextBox() { Multiline = true }, "Notes");
		}

		private static MainForm _Instance;
		public static MainForm Instance
		{
			get { if (_Instance == null) _Instance = new MainForm(); return _Instance; }
			protected set { _Instance = value; }
		}
	}
}
