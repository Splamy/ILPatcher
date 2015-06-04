using ILPatcher.Data;
using ILPatcher.Interface.Main; // TODO: remove when static is removed
using System;
using System.Windows.Forms;

namespace ILPatcher.Interface.General
{
	public partial class MultiPicker : Form
	{
		private Func<object, bool> filterPredicate;
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

			ILManager.Instance.InitTree(MainPanel.MainAssemblyDefinition);
			structureViever1.AfterSelect += structureViever1_AfterSelect;
		}

		/// <summary>Shows the Memberpicker and automatically initializes it with the structureViever rebuild and the given specifications.</summary>
		/// <param name="fF">FilterElements: Enumeration to choose which Membertypes should be shown only.</param>
		/// <param name="fP">FilterPredicate: Method to check if the currently selected member could be chosen.</param>
		/// <param name="cb">Callback: Method that gets called when a member has been chosen.</param>
		/// <param name="mainmodonly">True if only the maindoule from the own assembly should be listed, false to show all.</param>
		public void ShowStructure(StructureView fE, Func<object, bool> fP, Action<object> cb, bool mainmodonly = false)
		{
			structureViever1.ContextAssemblyLoad = !mainmodonly;
			this.Show();

			if (filterPredicate != fP || callback != cb || structureViever1.FilterElements != fE)
			{
				filterPredicate = fP;
				callback = cb;
				structureViever1.FilterElements = fE;
				structureViever1.RebuildHalfAsync(mainmodonly);
			}
		}

		/// <summary>Method to add Nodes in the StructureViewer to the seperated ToolBoxNode</summary>
		/// <param name="extNode">ILNode to be added</param>
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
			btn_Select.Top = Height - (btn_Select.Height + 45);
			btn_Cancel.Top = Height - (btn_Cancel.Height + 45);
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
}
