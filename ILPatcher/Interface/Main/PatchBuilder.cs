using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ILPatcher.Data;
using ILPatcher.Data.Actions;
using ILPatcher.Data.Finder;
using MetroObjects;
using ILPatcher.Utility;

namespace ILPatcher.Interface.Main
{
	[EditorAttributes("Patchbuilder")]
	public class PatchBuilder : EditorBase<PatchEntry>
	{
		private EditorFactory editorFactory;
		private Timer uiMover;
		private List<EntryBlockHolder> holderList;

		public override bool FixedHeight => false;
		public override int DefaultHeight => 0;

		#region Interface Elements
		Panel holderPanel;
		TextBox txtName;
		TreeView entryPoolView;
		MVScrollBar buildScroll;
		#endregion

		public PatchBuilder(DataStruct dataStruct, EditorFactory editorFactory) : base(dataStruct)
		{
			InitializeGridLineManager();

			holderList = new List<EntryBlockHolder>();

			this.editorFactory = editorFactory;
			uiMover = new Timer();
			uiMover.Interval = 10;
			uiMover.Tick += UiMover_Tick;
		}


		private void InitializeGridLineManager()
		{
			holderPanel = new Panel();
			holderPanel.BorderStyle = BorderStyle.FixedSingle;
			txtName = new TextBox();
			txtName.TextChanged += TxtName_TextChanged;
			entryPoolView = new TreeView();
			entryPoolView.ShowPlusMinus = false;
			entryPoolView.ShowRootLines = false;
			entryPoolView.DoubleClick += PoolView_DoubleClick;
			buildScroll = new MVScrollBar();
			buildScroll.OnChange += BuildScroll_OnChange;

			var grid = new GridLineManager(this, true);
			int line = grid.AddLineFilling(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, entryPoolView, GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, holderPanel, GlobalLayout.MinFill);
			grid.AddElementFixed(line, buildScroll, GlobalLayout.LineHeight);
			line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroLabel("Name"), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, txtName, GlobalLayout.MinFill);
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Back", Save_Click), GlobalLayout.LabelWidth);
		}

		private void BuildScroll_OnChange(MVScrollBar source, int value)
		{
			ReorderHolder();
		}

		private void RealoadAll()
		{
			ReloadPool();
			ReloadEntryPanel();
			ReorderHolder();
		}

		private void ReloadEntryPanel()
		{
			foreach (var c in holderList)
				c.Dispose();
			holderPanel.Controls.Clear();
			holderList.Clear();
			foreach (var entry in myData.PatchChain)
				AddEntry(entry, false);
		}

		private void ReloadPool()
		{
			entryPoolView.Nodes.Clear();
			FillPoolTreeWithData(entryPoolView, dataStruct.TargetFinderList, editorFactory.FinderEditors);
			FillPoolTreeWithData(entryPoolView, dataStruct.PatchActionList, editorFactory.ActionEditors);
		}

		private void FillPoolTreeWithData<T>(TreeView tree, IList<T> existing, ICollection<Type> editors) where T : EntryBase
		{
			var nodeExist = tree.Nodes.Add("Add Existing:");
			foreach (var entry in existing)
				nodeExist.Nodes.Add(new TreeNode(entry.Name) { Tag = entry, ToolTipText = entry.Description });
			var nodeNew = tree.Nodes.Add("Add New:");
			foreach (var edi in editors)
				nodeNew.Nodes.Add(new TreeNode(EditorFactory.GetEditorName(edi)) { Tag = edi });
			tree.ExpandAll();
		}

		// EntryDrag

		private EntryBlockHolder dragHolder = null;
		private Point dragPoint = Point.Empty;
		int dragTakePos = -1;
		int dragInsertPos = -1;

		private void LblResposition_MouseDown(object sender, MouseEventArgs e)
		{
			var dragLbl = sender as Label;
			dragHolder = dragLbl.Parent as EntryBlockHolder;
			dragPoint = e.Location;
			dragTakePos = holderList.IndexOf(dragHolder);
			uiMover.Enabled = true;
		}

		private void LblResposition_MouseUp(object sender, MouseEventArgs e)
		{
			if (dragTakePos == -1 || dragInsertPos == -1) return;

			holderList.MoveItem(dragTakePos, dragInsertPos);
			myData.PatchChain.MoveItem(dragTakePos, dragInsertPos);

			dragHolder = null;
			dragTakePos = -1;
			dragInsertPos = -1;

			SimulateFlow();
		}

		private void LblResposition_MouseMove(object sender, MouseEventArgs e)
		{
			if (dragHolder != null)
			{
				uiMover.Enabled = true;
				dragHolder.Top += e.Y - dragPoint.Y;

				int pos = 0;
				for (int i = 0; i < holderList.Count; i++)
				{
					if (dragHolder.Top < pos + holderList[i].Height / 2)
					{
						dragInsertPos = i;
						break;
					}
					pos += holderList[i].Height;
				}
			}
		}

		private void UiMover_Tick(object sender, EventArgs e)
		{
			IList<EntryBlockHolder> tmp;
			if (dragInsertPos != -1 && dragTakePos != -1)
			{
				tmp = new EntryBlockHolder[holderList.Count];
				if (dragTakePos <= dragInsertPos)
				{
					for (int i = 0; i < holderList.Count; i++)
					{
						if (i < dragTakePos)
							tmp[i] = holderList[i];
						else if (i < dragInsertPos)
							tmp[i] = holderList[i + 1];
						else if (i == dragInsertPos)
							tmp[i] = holderList[dragTakePos];
						else
							tmp[i] = holderList[i];
					}
				}
				else if (dragInsertPos < dragTakePos)
				{
					for (int i = 0; i < holderList.Count; i++)
					{
						if (i < dragInsertPos)
							tmp[i] = holderList[i];
						else if (i == dragInsertPos)
							tmp[i] = holderList[dragTakePos];
						else if (i <= dragTakePos)
							tmp[i] = holderList[i - 1];
						else
							tmp[i] = holderList[i];
					}
				}
			}
			else
				tmp = holderList;

			int pos = -buildScroll.Value;
			bool movedSomething = false;
			for (int i = 0; i < tmp.Count; i++)
			{
				while (i != dragInsertPos || dragHolder == null)
				{
					if (tmp[i].Top == pos)
						break;

					movedSomething = true;

					const int speed = 10;
					int varspeed = Math.Max(speed, Math.Abs(tmp[i].Top - pos) / 5);
					if (tmp[i].Top > pos && tmp[i].Top - speed > pos)
						tmp[i].Top -= varspeed;
					else if (tmp[i].Top < pos && tmp[i].Top + speed < pos)
						tmp[i].Top += varspeed;
					else
						tmp[i].Top = pos;

					break;
				}
				pos += tmp[i].Height;
			}

			if (!movedSomething && dragHolder == null)
			{
				uiMover.Enabled = false;
			}
		}

		// Interface tools

		private void AddNewEntry(EntryBase entry)
		{
			myData.PatchChain.Add(entry);
			AddEntry(entry, true);
		}

		private void AddEntry(EntryBase finder, bool applyRefresh)
		{
			var entryHolder = new EntryBlockHolder(finder, editorFactory, SwooshParent);
			entryHolder.lblResposition.MouseDown += LblResposition_MouseDown;
			entryHolder.lblResposition.MouseMove += LblResposition_MouseMove;
			entryHolder.lblResposition.MouseUp += LblResposition_MouseUp;
			entryHolder.txtName.TextChanged += TxtName_TextChanged1;
			holderPanel.Controls.Add(entryHolder);
			holderList.Add(entryHolder);

			if (applyRefresh)
			{
				CalculateListSizes();
				ReloadPool();
				ReorderHolder();
			}
		}

		private void TxtName_TextChanged1(object sender, EventArgs e)
		{
			var box = (TextBox)sender;
			var entry = box.Tag as EntryBase;
			entry.Name = box.Text;
			ReloadPool();
		}

		private void CalculateListSizes()
		{
			int height = 0;
			for (int i = 0; i < holderList.Count; i++)
			{
				height += holderList[i].Height;
				holderList[i].Width = holderPanel.Width;
			}
			buildScroll.Max = Math.Max(0, height - holderPanel.Height);
		}

		private void ReorderHolder()
		{
			int pos = 0;
			foreach (var holder in holderList)
			{
				holder.Top = pos - buildScroll.Value;
				pos += holder.Height;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			CalculateListSizes();
		}

		// Events

		private void PoolView_DoubleClick(object sender, EventArgs e)
		{
			object tag = ((TreeView)sender).SelectedNode?.Tag;
			if (tag == null)
				return;

			// We clicked on a existing EntryBase
			if (tag is PatchAction || tag is TargetFinder)
			{
				if (tag is PatchAction)
					AddEntry((PatchAction)tag, true);
				else if (tag is TargetFinder)
					AddEntry((TargetFinder)tag, true);
				return;
			}

			// We clicked to create a new EntryBase
			Type patchActionType = tag as Type;
			if (patchActionType != null)
			{
				var entryType = editorFactory.GetEntryTypeByEditorType(patchActionType);
				var finder = dataStruct.EntryFactory.CreateEntryByType(entryType);
				if (finder is PatchAction)
					AddNewEntry((PatchAction)finder);
				else if (finder is TargetFinder)
					AddNewEntry((TargetFinder)finder);
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

		// ILFlow

		private void SimulateFlow()
		{
			// TODO: move the testing logic to a sepereate class with something like a continue/next semantic
			// each elem can return ok/mismatch/unreachable
			bool outofreach = false;

			object currentInput = dataStruct.AssemblyDefinition;
			foreach (var holder in holderList)
			{
				if (currentInput == null) return;

				if (outofreach)
				{
					holder.lblInput.BackColor = Color.DarkGray;
					continue;
				}

				if (holder.entryElement is TargetFinder)
				{
					var targetFinder = (TargetFinder)holder.entryElement;

					if (currentInput.GetType() != targetFinder.TInput)
					{
						holder.lblInput.BackColor = Color.Red;
						return;
					}
					else
						holder.lblInput.BackColor = Color.Green;

					try
					{
						currentInput = targetFinder.FilterInput(currentInput);
						holder.lblOutput.Text = "Output: " + CecilFormatter.TryFormat(currentInput);
						holder.lblOutput.BackColor = Color.Green;
					}
					catch (TargetNotFoundException)
					{
						holder.lblOutput.Text = "Output: Nothing found!";
						holder.lblOutput.BackColor = Color.Red;
					}
				}
				else if (holder.entryElement is PatchAction)
				{
					var patchAction = (PatchAction)holder.entryElement;

					if (currentInput.GetType() != patchAction.TInput)
					{
						holder.lblInput.BackColor = Color.Red;
						return;
					}
					else
						holder.lblInput.BackColor = Color.Green;

					outofreach = true;
				}
			}
		}
	}
}
