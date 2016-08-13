using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ILPatcher.Data;
using System.Windows.Forms;
using MetroObjects;
using ILPatcher.Data.Finder;
using ILPatcher.Data.Actions;

namespace ILPatcher.Interface.Main
{
	class EntryBlockHolder : Panel
	{
		private static readonly Brush dragBrush = new HatchBrush(HatchStyle.LightDownwardDiagonal, MetroGlobal.DefaultColorContent, SystemColors.ButtonFace);
		public EntryBase entryElement { get; protected set; }

		private Swoosh swooshManager;
		private EditorFactory editorFactory;

		//===
		private Control splitPanel;
		public Label lblResposition { get; private set; }
		//===
		public Label lblInput { get; private set; }
		public TextBox txtName { get; private set; }
		private Label lblTypeName;
		private Button btnEdit;
		//===
		private EditorPanel ownEditor;
		//===
		private Label lblResizeDrag;
		public Label lblOutput { get; private set; }

		public EntryBlockHolder(EntryBase entryElem, EditorFactory eFactory, Swoosh swooshMgr)
		{
			entryElement = entryElem;
			editorFactory = eFactory;
			swooshManager = swooshMgr;

			//===
			lblResposition = new Label();
			lblResposition.BorderStyle = BorderStyle.None;
			lblResposition.Dock = DockStyle.Right;
			lblResposition.Width = 15;
			lblResposition.Cursor = Cursors.SizeAll;
			lblResposition.Paint += (s, e) => e.Graphics.FillRectangle(dragBrush, lblResposition.ClientRectangle);

			splitPanel = new Control();
			splitPanel.Dock = DockStyle.Fill;
			Controls.Add(lblResposition);
			Controls.Add(splitPanel);
			splitPanel.BringToFront();

			var glm = new GridLineManager(splitPanel, false);
			glm.ElementDistance = 0;

			var line = glm.AddLineFixed(GlobalLayout.LineHeight);
			lblInput = GlobalLayout.GenMetroLabel("<Input>");
			glm.AddElementFilling(line, lblInput, GlobalLayout.MinFill);

			line = glm.AddLineFixed(GlobalLayout.LineHeight);
			lblTypeName = GlobalLayout.GenMetroLabel(GetTypeNameFromEntry(entryElem));
			glm.AddElementFixed(line, lblTypeName, GlobalLayout.LabelWidth);
			txtName = new TextBox();
			txtName.Enabled = false;
			txtName.Tag = entryElem;
			glm.AddElementFilling(line, txtName, GlobalLayout.MinFill);
			btnEdit = GlobalLayout.GenMetroButton("Edit", Edit_Click);
			glm.AddElementFixed(line, btnEdit, GlobalLayout.LabelWidth / 2);

			line = glm.AddLineFilling(GlobalLayout.LineHeight);
			ownEditor = editorFactory.CreateEditorForEntry(entryElem);
			ownEditor.Enabled = false;
			glm.AddElementFilling(line, ownEditor, GlobalLayout.MinFill);

			line = glm.AddLineFixed(5);
			lblResizeDrag = new Label();
			lblResizeDrag.BorderStyle = BorderStyle.None;
			lblResizeDrag.Cursor = Cursors.SizeNS;
			lblResizeDrag.Paint += (s, e) => e.Graphics.FillRectangle(dragBrush, lblResizeDrag.ClientRectangle);
			glm.AddElementFilling(line, lblResizeDrag, GlobalLayout.MinFill);

			line = glm.AddLineFixed(GlobalLayout.LineHeight);
			lblOutput = GlobalLayout.GenMetroLabel("<Output>");
			glm.AddElementFilling(line, lblOutput, GlobalLayout.MinFill);

			Height = ownEditor.DefaultHeight + GlobalLayout.LineHeight * 3 + 5;

			if (entryElem is TargetFinder)
			{
				var finder = (TargetFinder)entryElem;
				lblInput.Text = "Input: " + finder.TInput.Name;
				lblOutput.Text = "Output: " + finder.TOutput.Name;
			}
			else if (entryElem is PatchAction)
			{
				var action = (PatchAction)entryElem;
				lblInput.Text = "Input: " + action.TInput.Name;
				lblOutput.Text = "Output: <none>";
			}
		}

		protected void Edit_Click(object sender, EventArgs e)
		{
			if (!ownEditor.Enabled)
			{
				ownEditor.Enabled = txtName.Enabled = true;
				btnEdit.Text = "Done";
			}
			else
			{
				ownEditor.Enabled = txtName.Enabled = false;
				btnEdit.Text = "Edit";
			}
		}

		private static string GetTypeNameFromEntry(EntryBase element)
		{
			string typeName;
			switch (element.EntryKind)
			{
			case EntryKind.PatchAction: typeName = ((Data.Actions.PatchAction)element).PatchActionType.ToString(); break;
			case EntryKind.TargetFinder: typeName = ((Data.Finder.TargetFinder)element).TargetFinderType.ToString(); break;
			default:
			case EntryKind.Unknown:
				typeName = "<?>";
				break;
			}
			return typeName;
		}
	}
}
