using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public partial class MultiPicker : Form
	{
		private Func<object, bool> filterPredicate;
		private StructureView filterFlag;
		private Action<object> callback;

		private static MultiPicker _Instance;
		public static MultiPicker Instance
		{
			get { if (_Instance == null || _Instance.IsDisposed) _Instance = new MultiPicker(); return _Instance; }
			set { _Instance = value; }
		}

		private MultiPicker()
		{
			InitializeComponent();
			Owner = MainForm.Instance;

			ILManager.Instance.InitTree(MainPanel.AssemblyDef);
			structureViever1.AfterSelect += structureViever1_AfterSelect;
		}

		public void ShowStructure(StructureView fF, Func<object, bool> fP, Action<object> cb, bool mainmodonly = false)
		{
			filterFlag = fF;
			filterPredicate = fP;
			callback = cb;
			structureViever1.FilterElements = fF;
			this.Show();
			structureViever1.RebuildHalfAsync(mainmodonly);
		}

		public void AddToolBoxNode(ILNode extNode)
		{
			structureViever1.AddToolBoxNode(extNode);
		}

		private void structureViever1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			btn_Select.Enabled = filterPredicate(structureViever1.SelectedNode.Tag);
		}

		private void MultiPicker_Resize(object sender, EventArgs e)
		{
			btn_Select.Top = Height - (btn_Select.Height + 40);
			btn_Cancel.Top = Height - (btn_Cancel.Height + 40);
			structureViever1.Height = Height - (btn_Select.Height + 50);
		}

		private void btn_Select_Click(object sender, EventArgs e)
		{
			if (structureViever1.SelectedNode == null) return;
			Hide();
			callback(structureViever1.SelectedNode.Tag);
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Hide();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}
	}

	public delegate void OnItemSelected(object tag);
}
