using ILPatcher.Data;
using System;
using System.Windows.Forms;

namespace ILPatcher.Interface.General
{
	public partial class MultiPicker<T> : Form where T : class
	{
		private DataStruct dataStruct;
		private Func<T, bool> filterPredicate;
		private Action<T> callback;

		private MultiPicker()
		{
			InitializeComponent();

			structureViever1.InitTree(dataStruct.AssemblyDefinition);
			structureViever1.AfterSelect += structureViever1_AfterSelect;
		}

		/// <summary>Shows the Memberpicker and automatically initializes it with the structureViever rebuild and the given specifications.</summary>
		/// <param name="fF">FilterElements: Enumeration to choose which Membertypes should be shown only.</param>
		/// <param name="fP">FilterPredicate: Method to check if the currently selected member could be chosen.</param>
		/// <param name="cb">Callback: Method that gets called when a member has been chosen.</param>
		/// <param name="mainmodonly">True if only the maindoule from the own assembly should be listed, false to show all.</param>
		public static MultiPicker<T> ShowStructure(DataStruct dataStruct, StructureView fE, Func<T, bool> fP, Action<T> cb, bool mainmodonly = false)
		{
			MultiPicker<T> mp = new MultiPicker<T>();
			mp.dataStruct = dataStruct;
			mp.structureViever1.ContextAssemblyLoad = !mainmodonly;
			mp.filterPredicate = fP;
			mp.callback = cb;
			mp.structureViever1.FilterElements = fE;
			mp.structureViever1.RebuildHalfAsync(mainmodonly);
			mp.Show();
			return mp;
		}

		/// <summary>Method to add Nodes in the StructureViewer to the seperated ToolBoxNode</summary>
		/// <param name="extNode">ILNode to be added</param>
		public void AddToolBoxNode(ILNode extNode)
		{
			structureViever1.AddToolBoxNode(extNode);
		}

		private void structureViever1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			T selectedValue = structureViever1.SelectedNode.Tag as T;
			if (selectedValue != null)
				btn_Select.Enabled = filterPredicate(selectedValue);
		}

		private void MultiPicker_Resize(object sender, EventArgs e)
		{
			btn_Select.Top = Height - (btn_Select.Height + 45);
			btn_Cancel.Top = Height - (btn_Cancel.Height + 45);
			structureViever1.Height = Height - (btn_Select.Height + 50);
		}

		private void btn_Select_Click(object sender, EventArgs e)
		{
			if (structureViever1.SelectedNode == null) return;
			Hide();
			T selectedValue = structureViever1.SelectedNode.Tag as T;
			if (selectedValue != null)
				callback(selectedValue);
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
