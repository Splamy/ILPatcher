using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public partial class MultiPicker : Form
	{
		private EditorILPattern parent;
		public MultiPicker(EditorILPattern _parent)
		{
			InitializeComponent();
			parent = _parent;
			Owner = MainForm.Instance;

			ILManager.Instance.InitTree(MainPanel.AssemblyDef, 1); // TODO CHECK REFRESH
			structureViever1.Rebuild(true);
			structureViever1.AfterSelect += structureViever1_AfterSelect;
		}

		void structureViever1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			MethodDefinition MetDef = structureViever1.SelectedNode.Tag as MethodDefinition;
			if (MetDef == null)
				btn_Select.Enabled = false;
			else
				btn_Select.Enabled = true;
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
			MethodDefinition metDef = structureViever1.SelectedNode.Tag as MethodDefinition;
			if (metDef == null) return;
			parent.LoadMetDef(metDef);
			Hide();
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
}
