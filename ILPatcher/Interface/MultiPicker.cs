using ILPatcher.Data;
using System;
using System.Windows.Forms;

namespace ILPatcher.Interface
{
	public partial class MultiPicker<T> : Form where T : class
	{
		private DataStruct dataStruct;
		private Predicate<T> filterPredicate;
		private Action<T> callback;

		#region Interface Elements
		StructureViewer structureViever;
		Button btnSelect;
		#endregion

		private MultiPicker(DataStruct dataStruct)
		{
			this.dataStruct = dataStruct;

			InitializeGridLineManager();
			Size = new System.Drawing.Size(300, 600);
		}

		private void InitializeGridLineManager()
		{
			structureViever = new StructureViewer(dataStruct);
			//structureViever.RebuildHalfAsync();
			structureViever.AfterSelect += structureViever1_AfterSelect;
			btnSelect = GlobalLayout.GenMetroButton("Select", Select_Click);

			var grid = new GridLineManager(this, true);
			int line = grid.AddLineFilling(GlobalLayout.LineHeight);
			grid.AddElementFilling(line, structureViever, GlobalLayout.MinFill);
			line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementStretchable(line, btnSelect, GlobalLayout.MinFill, GlobalLayout.LabelWidth);
			grid.AddElementStretchable(line, GlobalLayout.GenMetroButton("Cancel", Cancel_Click), GlobalLayout.MinFill, GlobalLayout.LabelWidth);
		}

		/// <summary>Shows the Memberpicker and automatically initializes it with the structureViever rebuild and the given specifications.</summary>
		/// <param name="fF">FilterElements: Enumeration to choose which Membertypes should be shown only.</param>
		/// <param name="fP">FilterPredicate: Method to check if the currently selected member could be chosen.</param>
		/// <param name="cb">Callback: Method that gets called when a member has been chosen.</param>
		/// <param name="mainmodonly">True if only the maindoule from the own assembly should be listed, false to show all.</param>
		public static MultiPicker<T> ShowStructure(DataStruct dataStruct, StructureView fE, Predicate<T> fP, Action<T> cb, bool mainmodonly = false)
		{
			MultiPicker<T> mp = new MultiPicker<T>(dataStruct);
			mp.structureViever.ContextAssemblyLoad = !mainmodonly;
			mp.filterPredicate = fP;
			mp.callback = cb;
			mp.structureViever.FilterElements = fE;
			mp.structureViever.RebuildAsync(mainmodonly);
			mp.Show();
			return mp;
		}

		/// <summary>Method to add Nodes in the StructureViewer to the seperated ToolBoxNode</summary>
		/// <param name="extNode">ILNode to be added</param>
		public void AddToolBoxNode(ILNode extNode)
		{
			structureViever.AddToolBoxNode(extNode);
		}

		private void structureViever1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			T selectedValue = structureViever.SelectedNode.Tag as T;
			if (selectedValue != null)
				btnSelect.Enabled = filterPredicate(selectedValue);
		}

		private void Select_Click(object sender, EventArgs e)
		{
			if (structureViever.SelectedNode == null) return;
			ClosePicker();
			T selectedValue = structureViever.SelectedNode.Tag as T;
			if (selectedValue != null)
				callback(selectedValue);
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			ClosePicker();
		}

		private void ClosePicker()
		{
			structureViever.Dispose();
			Close();
		}
	}
}
