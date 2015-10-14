using System;
using System.Drawing;
using ILPatcher.Data;
using System.Windows.Forms;

namespace ILPatcher.Interface.Main
{
	class EntryBlockHolder : Panel
	{
		public EntryBase entryElement { get; protected set; }

		private Swoosh swooshManager;
		private EditorFactory editorFactory;
		private Control displayControl;
		private bool isEditorInline;

		public Label lblName { get; protected set; }
		public Button btnEdit { get; protected set; }
		public Label lblLabel { get; protected set; }

		public EntryBlockHolder(EntryBase entryElem, EditorFactory eFactory, Swoosh swooshMgr)
		{
			entryElement = entryElem;
		}

		public EntryBlockHolder(Type editorType, EditorFactory eFactory, Swoosh swooshMgr)
		{



			entryElement = entryElem;
		}

		private void Initialize(IEditorPanel editor, DataStruct dataStruct, EditorFactory eFactory, Swoosh swooshMgr)
		{
			editorFactory = eFactory;
			swooshManager = swooshMgr;

			Height = 100;

			lblName = GlobalLayout.GenMetroLabel(GetNameFromEntry(entryElem));
			lblName.Location = new Point(0, 0);
			lblName.Height = GlobalLayout.LineHeight;
			btnEdit = GlobalLayout.GenMetroButton("Edit", Edit_Click);
			btnEdit.Size = new Size(GlobalLayout.LabelWidth, GlobalLayout.LineHeight);
			btnEdit.Top = 0;
			Controls.Add(lblName);
			Controls.Add(btnEdit);

			Type editorType = editorFactory.GetEditorType(entryElem);
			isEditorInline = EditorFactory.IsInline(editorType);
			if (isEditorInline)
			{
				displayControl = (Control)eFactory.CreateEditorByType(editorType, entryElem.dataStruct);
				displayControl.Location = new Point(0, GlobalLayout.LineHeight);
				displayControl.Enabled = false;
			}
			else
			{
				lblLabel = GlobalLayout.GenMetroLabel(entryElem.Label);
				displayControl = lblLabel;
			}
			Controls.Add(displayControl);
		}

		protected void Edit_Click(object sender, EventArgs e)
		{
			if (isEditorInline)
			{
				displayControl.Enabled = true;
			}
			else
			{
				var eType = editorFactory.GetEditorType(entryElement);
				string panelName = EditorFactory.GetEditorName(eType);
				swooshManager.PushPanel((Swoosh.ISwoosh)editorFactory.CreateEditorByEntry(entryElement), panelName);
			}
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			lblName.Width = Width - GlobalLayout.LabelWidth;
			btnEdit.Left = Width - GlobalLayout.LabelWidth;

			displayControl.Size = new Size(Width, Height - GlobalLayout.LineHeight);
		}

		private string GetNameFromEntry(EntryBase element)
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
			return $"({typeName}) {element.Name}";
		}
	}
}
