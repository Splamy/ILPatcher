using ILPatcher.Data;
using ILPatcher.Data.Actions;
using ILPatcher.Data.Finder;
using ILPatcher.Interface.Actions;
using ILPatcher.Interface.Finder;
using ILPatcher.Utility;
using System;
using System.Windows.Forms;

namespace ILPatcher.Interface.Main
{
	public class PatchBuilder : EditorBase<PatchEntry, PatchEntry>, IEditorPanel
	{
		public override string PanelName { get { return "Patchbuilder"; } }
		public override bool IsInline { get { return false; } }

		private bool editMode;

		#region Interface Elements
		GridLineManager glmFinder;
		Panel mTargetFinder;
		Panel mPatchAction;
		TextBox txtName;
		#endregion

		public PatchBuilder(DataStruct dataStruct) : base(dataStruct)
		{
			InitializeGridLineManager();
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
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Save", Save_Click), GlobalLayout.LabelWidth);
		}

		private void LoadInterfaceContent()
		{
			foreach (var finder in myData.FinderChain)
				AddFinder(finder);

			if (myData.PatchAction != null)
				SetAction(myData.PatchAction);
		}

		// Interface tools

		private IEditorTargetFinder GetAssociatedFinderEditor(TargetFinder finder)
		{
			switch (finder.TargetFinderType)
			{
			case TargetFinderType.ClassByName: return new EditorFinderClassByName(dataStruct);
			default: return null;
			}
		}

		private IEditorPatchAction GetAssociatedActionEditor(PatchAction action)
		{
			switch (action.PatchActionType)
			{
			case PatchActionType.ILMethodFixed: return new EditorILPattern(dataStruct);
			case PatchActionType.ILMethodCreator: return new EditorMethodCreator(dataStruct);
			default: return null;
			}
		}

		private void AddFinder(TargetFinder finder)
		{
			IEditorTargetFinder fEditor = GetAssociatedFinderEditor(finder);
			if (fEditor == null)
			{
				Log.Write(Log.Level.Error, $"No editor found for {finder.TargetFinderType.ToString()}");
				return;
			}
			var fHolder = new EntryBlockHolder(finder, fEditor);
			int line = glmFinder.AddLineFixed(fHolder.Height);
			glmFinder.AddElementFilling(line, fHolder, GlobalLayout.MinFill);
		}

		private void SetAction(PatchAction action)
		{
			IEditorPatchAction aEditor = GetAssociatedActionEditor(action);
			if (aEditor == null)
			{
				Log.Write(Log.Level.Error, $"No editor found for {action.PatchActionType.ToString()}");
				return;
			}
			foreach (Control c in mPatchAction.Controls) c.Dispose();
			mPatchAction.Controls.Clear();

			var aHolder = new EntryBlockHolder(action, aEditor);
			mPatchAction.Controls.Add(aHolder);
			aHolder.Dock = DockStyle.Fill;
		}

		// Events

		private void AddFinder_Click(object sender, EventArgs e)
		{

		}

		private void SetAction_Click(object sender, EventArgs e)
		{

		}

		private void Save_Click(object sender, EventArgs e)
		{
			if (editMode && myData != null)
				dataStruct.PatchEntryList.Add(myData);
			SwooshBack();
		}

		private void TxtName_TextChanged(object sender, EventArgs e)
		{
			myData.Name = txtName.Text;
		}

		protected override void OnPatchDataSet()
		{
			editMode = true;
			LoadInterfaceContent();
		}

		// Functionality

		public override PatchEntry CreateNewEntryPart()
		{
			editMode = false;
			return new PatchEntry(dataStruct);
		}
	}
}
