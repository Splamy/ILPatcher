using ILPatcher.Data;
using ILPatcher.Data.Actions;
using ILPatcher.Data.Finder;
using ILPatcher.Interface.Actions;
using ILPatcher.Interface.Finder;
using ILPatcher.Utility;
using System;
using System.Windows.Forms;
using System.Linq;

namespace ILPatcher.Interface.Main
{
	[EditorAttributes("Patchbuilder")]
	public class PatchBuilder : EditorBase<PatchEntry>
	{
		private EditorFactory editorFactory;

		#region Interface Elements
		GridLineManager glmFinder;
		Panel mTargetFinder;
		Panel mPatchAction;
		TextBox txtName;
		TreeView finderPoolView;
		#endregion

		public PatchBuilder(DataStruct dataStruct, EditorFactory editorFactory) : base(dataStruct)
		{
			InitializeGridLineManager();

			this.editorFactory = editorFactory;
		}

		private void InitializeGridLineManager()
		{
			mPatchAction = new Panel();
			mTargetFinder = new Panel();
			mTargetFinder.BorderStyle = BorderStyle.FixedSingle;
			glmFinder = new GridLineManager(mTargetFinder, false);
			glmFinder.ElementDistance = 0;
			txtName = new TextBox();
			txtName.TextChanged += TxtName_TextChanged;
			finderPoolView = new TreeView();
			finderPoolView.ShowPlusMinus = false;
			finderPoolView.ShowRootLines = false;
			finderPoolView.DoubleClick += PoolView_DoubleClick;

			var grid = new GridLineManager(this, true);
			int line = grid.AddLineFilling(GlobalLayout.LineHeight);
			//grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Add Finder", AddFinder_Click), GlobalLayout.LabelWidth);
			grid.AddElementFixed(line, finderPoolView, GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, mTargetFinder, GlobalLayout.MinFill);
			line = grid.AddLineStrechable(GlobalLayout.MinFill, 150);
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Set Action", SetAction_Click), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, mPatchAction, GlobalLayout.MinFill);
			line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroLabel("Name"), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, txtName, GlobalLayout.MinFill);
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Back", Save_Click), GlobalLayout.LabelWidth);
		}

		private void PoolView_DoubleClick(object sender, EventArgs e)
		{
			object tag = finderPoolView.SelectedNode?.Tag;
			if (tag == null)
				return;

			TargetFinder targetFinder = tag as TargetFinder;
			if (targetFinder != null)
			{
				AddFinder(targetFinder, true);
				return;
			}

			Type finderEditorType = tag as Type;
			if (finderEditorType != null)
			{
				var entryType = editorFactory.GetEntryTypeByEditorType(finderEditorType);
				var finder = (TargetFinder)dataStruct.EntryFactory.CreateEntryByType(entryType);
				AddNewFinder(finder);
				return;
			}

			throw new InvalidOperationException("PoolView node could not be processed");
		}

		private void LoadInterfaceContent()
		{
			foreach (var finder in myData.FinderChain)
				AddFinder(finder, false);

			if (myData.PatchAction != null)
				SetAction(myData.PatchAction);

			var nodeExist = finderPoolView.Nodes.Add("Add Existing:");
			foreach (var finder in dataStruct.TargetFinderList)
				nodeExist.Nodes.Add(new TreeNode(finder.Name) { Tag = finder, ToolTipText = finder.Description });
			var nodeNew = finderPoolView.Nodes.Add("Add New:");
			foreach (var edi in editorFactory.FinderEditors)
				nodeNew.Nodes.Add(new TreeNode(EditorFactory.GetEditorName(edi)) { Tag = edi });
			finderPoolView.ExpandAll();
		}

		private void RealoadInterfaceContent()
		{
			foreach (Control c in mTargetFinder.Controls) c.Dispose();
			mTargetFinder.Controls.Clear();

			finderPoolView.Nodes.Clear();

			LoadInterfaceContent();
		}

		// Interface tools

		private void AddFinder(TargetFinder finder, bool applyRefresh)
		{
			var fHolder = new EntryBlockHolder(finder, editorFactory, SwooshParent);
			int line = glmFinder.AddLineFixed(fHolder.Height);
			glmFinder.AddElementFilling(line, fHolder, GlobalLayout.MinFill);
			if (applyRefresh)
				glmFinder.InvokeResize();
		}

		private void AddNewFinder(TargetFinder finder)
		{
			myData.FinderChain.Add(finder);
			AddFinder(finder, true);
		}

		private void SetAction(PatchAction action)
		{
			foreach (Control c in mPatchAction.Controls) c.Dispose();
			mPatchAction.Controls.Clear();

			var aHolder = new EntryBlockHolder(action, editorFactory, SwooshParent);
			mPatchAction.Controls.Add(aHolder);
			aHolder.Dock = DockStyle.Fill;
		}

		// Events

		private void AddFinder_Click(object sender, EventArgs e)
		{
			// TODO: implement
		}

		private void SetAction_Click(object sender, EventArgs e)
		{
			// TODO: implement

		}

		private void Save_Click(object sender, EventArgs e)
		{
			SwooshBack();
		}

		private void TxtName_TextChanged(object sender, EventArgs e)
		{
			myData.Name = txtName.Text;
		}

		protected override void OnPatchDataSet()
		{
			LoadInterfaceContent();
		}

		// Functionality

		// TMP IDEA
		private class AddBar : Control
		{
			PatchBuilder builder;
			ComboBox eCmbx;

			public AddBar(PatchBuilder parent)
			{
				builder = parent;

				eCmbx = new ComboBox();
				Controls.Add(eCmbx);
				eCmbx.Location = new System.Drawing.Point(120, 10);
				eCmbx.Size = new System.Drawing.Size(100, 20);

				foreach (var finder in builder.editorFactory.FinderEditors)
					eCmbx.Items.Add(EditorFactory.GetEditorName(finder));

				var btnAdd = GlobalLayout.GenMetroButton("Add Finder", AddFinder_Click);
				Controls.Add(btnAdd);
				btnAdd.Location = new System.Drawing.Point(10, 10);
				btnAdd.Size = new System.Drawing.Size(100, 20);
			}

			private void AddFinder_Click(object sender, EventArgs e)
			{
				var edT = builder.editorFactory.FinderEditors.ToArray()[eCmbx.SelectedIndex];

			}
		}
	}
}
