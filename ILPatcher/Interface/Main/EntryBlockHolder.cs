using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MetroObjects;
using ILPatcher.Data;
using System.Windows.Forms;

namespace ILPatcher.Interface.Main
{
	class EntryBlockHolder : Panel
	{
		public EntryBase entryElement { get; protected set; }
		public IEditorPanel associatedEditor { get; protected set; }
		private Control editorAsControl;

		public Label lblName { get; protected set; }
		public Button btnEdit { get; protected set; }
		public Label lblLabel { get; protected set; }

		public EntryBlockHolder(EntryBase entryElem, IEditorPanel editor)
		{
			entryElement = entryElem;
			associatedEditor = editor;

			Height = 100;

			lblName = GlobalLayout.GenMetroLabel(GetNameFromEntry(entryElem));
			lblName.Location = new Point(0, 0);
			lblName.Height = GlobalLayout.LineHeight;
			btnEdit = GlobalLayout.GenMetroButton("Edit", Edit_Click);
			btnEdit.Size = new Size(GlobalLayout.LabelWidth, GlobalLayout.LineHeight);
			btnEdit.Top = 0;
			Controls.Add(lblName);
			Controls.Add(btnEdit);

			if (associatedEditor.IsInline)
			{
				editorAsControl = (Control)editor;
				Controls.Add(editorAsControl);
				editorAsControl.Location = new Point(0, GlobalLayout.LineHeight);
				editorAsControl.Enabled = false;
			}
			else
			{
				lblLabel = GlobalLayout.GenMetroLabel(entryElem.Label);
				Controls.Add(lblLabel);
			}
		}

		protected void Edit_Click(object sender, EventArgs e)
		{
			editorAsControl.Enabled = true;
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			lblName.Width = Width - GlobalLayout.LabelWidth;
			btnEdit.Left = Width - GlobalLayout.LabelWidth;
			
			(associatedEditor.IsInline ? editorAsControl : lblLabel).Size = new Size(Width, Height - GlobalLayout.LineHeight);
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
