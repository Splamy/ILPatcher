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
		/// <summary>
		/// false if the current PatchEntry should be added to the parent list after saving
		/// true if it alreay is in the list, and just opened to edit.
		/// </summary>
		private bool editMode;
		private EditorFactory editorFactory;

		#region Interface Elements
		GridLineManager glmFinder;
		Panel mTargetFinder;
		Panel mPatchAction;
		TextBox txtName;
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

			var grid = new GridLineManager(this, true);
			int line = grid.AddLineFilling(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Add Finder", AddFinder_Click), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, mTargetFinder, GlobalLayout.MinFill);
			line = grid.AddLineStrechable(GlobalLayout.MinFill, 150);
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Set Action", SetAction_Click), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, mPatchAction, GlobalLayout.MinFill);
			line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroLabel("Name"), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, txtName, GlobalLayout.MinFill);
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Back", Save_Click), GlobalLayout.LabelWidth);
		}

		private void LoadInterfaceContent()
		{
			foreach (var finder in myData.FinderChain)
				AddFinder(finder, false);

			if (myData.PatchAction != null)
				SetAction(myData.PatchAction);

			int line = glmFinder.AddLineFixed(30);
			glmFinder.AddElementFilling(line, new AddBar(this), GlobalLayout.MinFill);
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
				var entryT = builder.editorFactory.GetEntryTypeByEditorType(edT);
				var entry = builder.dataStruct.EntryFactory.CreateEntryByType(entryT);
				var finder = (TargetFinder)entry;
				builder.myData.FinderChain.Add(finder);
				builder.AddFinder(finder, true);
			}
		}
	}
}
