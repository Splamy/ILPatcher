using ILPatcher.Data;
using ILPatcher.Data.Actions;
using ILPatcher.Data.Finder;
using ILPatcher.Interface.Actions;
using ILPatcher.Interface.Finder;
using ILPatcher.Utility;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
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
		TreeView actionPoolView;
		#endregion

		//TODO:
		// move textbox for name of entryBase to the Entryblockholder so the clumsy editors dont need to bother.
		// add Tinput/Toutput to the EntryBlockholders

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
			actionPoolView = new TreeView();
			finderPoolView = new TreeView();
			finderPoolView.ShowPlusMinus = actionPoolView.ShowPlusMinus = false;
			finderPoolView.ShowRootLines = actionPoolView.ShowRootLines = false;
			actionPoolView.DoubleClick += PoolView_DoubleClick;
			finderPoolView.DoubleClick += PoolView_DoubleClick;

			var grid = new GridLineManager(this, true);
			int line = grid.AddLineFilling(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, finderPoolView, GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, mTargetFinder, GlobalLayout.MinFill);
			line = grid.AddLineStrechable(GlobalLayout.MinFill, 150);
			grid.AddElementFixed(line, actionPoolView, GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, mPatchAction, GlobalLayout.MinFill);
			line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroLabel("Name"), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, txtName, GlobalLayout.MinFill);
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Back", Save_Click), GlobalLayout.LabelWidth);
		}

		private void RealoadAll()
		{
			ReloadPools();
			ReloadAction();
			ReloadFinder();
		}

		private void ReloadAction()
		{
			foreach (Control c in mPatchAction.Controls)
				c.Dispose();
			mPatchAction.Controls.Clear();
			if (myData.PatchAction != null)
				SetAction(myData.PatchAction, false);
		}

		private void ReloadFinder()
		{
			foreach (Control c in mTargetFinder.Controls)
				c.Dispose();
			mTargetFinder.Controls.Clear();
			foreach (var finder in myData.FinderChain)
				AddFinder(finder, false);
		}

		private void ReloadPools()
		{
			actionPoolView.Nodes.Clear();
			finderPoolView.Nodes.Clear();
			FillPoolTreeWithData(finderPoolView, dataStruct.TargetFinderList, editorFactory.FinderEditors);
			FillPoolTreeWithData(actionPoolView, dataStruct.PatchActionList, editorFactory.ActionEditors);
		}

		private void FillPoolTreeWithData<T>(TreeView tree, IList<T> existing, ICollection<Type> editors) where T : EntryBase
		{
			var nodeExist = tree.Nodes.Add("Add Existing:");
			foreach (var finder in existing)
				nodeExist.Nodes.Add(new TreeNode(finder.Name) { Tag = finder, ToolTipText = finder.Description });
			var nodeNew = tree.Nodes.Add("Add New:");
			foreach (var edi in editors)
				nodeNew.Nodes.Add(new TreeNode(EditorFactory.GetEditorName(edi)) { Tag = edi });
			tree.ExpandAll();
		}

		// Interface tools

		private void AddNewFinder(TargetFinder finder)
		{
			myData.FinderChain.Add(finder);
			AddFinder(finder, true);
		}

		private void AddFinder(TargetFinder finder, bool applyRefresh)
		{
			var fHolder = new EntryBlockHolder(finder, editorFactory, SwooshParent);
			int line = glmFinder.AddLineFixed(fHolder.Height);
			glmFinder.AddElementFilling(line, fHolder, GlobalLayout.MinFill);
			if (applyRefresh)
			{
				ReloadPools();
				glmFinder.InvokeResize();
			}
		}

		private void SetAction(PatchAction action, bool applyRefresh)
		{
			foreach (Control c in mPatchAction.Controls) c.Dispose();
			mPatchAction.Controls.Clear();

			var aHolder = new EntryBlockHolder(action, editorFactory, SwooshParent);
			mPatchAction.Controls.Add(aHolder);
			aHolder.Dock = DockStyle.Fill;
			if (applyRefresh)
			{
				ReloadPools();
				glmFinder.InvokeResize();
			}
		}

		// Events

		enum InvokePool
		{
			Undefined,
			Action,
			Finder,
		}

		private void PoolView_DoubleClick(object sender, EventArgs e)
		{
			InvokePool invoker = InvokePool.Undefined;
			if (sender == actionPoolView) invoker = InvokePool.Action;
			else if (sender == finderPoolView) invoker = InvokePool.Finder;

			if (invoker == InvokePool.Undefined) throw new InvalidOperationException();

			object tag = ((TreeView)sender).SelectedNode?.Tag;
			if (tag == null)
				return;

			if ((invoker == InvokePool.Action && tag is PatchAction)
				|| (invoker == InvokePool.Finder && tag is TargetFinder))
			{
				if (invoker == InvokePool.Action)
					SetAction((PatchAction)tag, true);
				else if (invoker == InvokePool.Finder)
					AddFinder((TargetFinder)tag, true);
				return;
			}

			Type patchActionType = tag as Type;
			if (patchActionType != null)
			{
				var entryType = editorFactory.GetEntryTypeByEditorType(patchActionType);
				var finder = dataStruct.EntryFactory.CreateEntryByType(entryType);
				if (invoker == InvokePool.Action)
					SetAction((PatchAction)finder, true);
				else if (invoker == InvokePool.Finder)
					AddNewFinder((TargetFinder)finder);
				return;
			}
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
			RealoadAll();
		}
	}
}
