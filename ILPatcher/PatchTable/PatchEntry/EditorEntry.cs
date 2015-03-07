using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ILPatcher
{
	public partial class EditorEntry : UserControl
	{
		private static EditorEntry _Instance;
		public static EditorEntry Instance
		{
			get { if (_Instance == null) _Instance = new EditorEntry(); return _Instance; }
			protected set { _Instance = value; }
		}

		private PatchEntry patchentry;
		public PatchEntry patchEntry
		{
			get
			{
				if (patchentry == null)
					patchentry = new PatchEntry();
				patchentry.EntryName = txtPatchEntryName.Text;
				return patchentry;
			}
			set { patchentry = value; }
		}

		private EditorEntry()
		{
			InitializeComponent();
			/*
			parent = _parent;
			//Owner = _parent;
			if (_pe == null)
			{
				pe = new PatchEntry();
				pe.EntryName = "testentry";
			}
			else
				pe = _pe; // init if != null
			 */
		}

		private void btnAddILPatch_Click(object sender, EventArgs e)
		{
			if (mEntryList.SelectedIndex >= 0) // TODO find correct inheritance | ADD Edit Function
				EditorILPattern.Instance.PatchAction = (PatchActionILMethodFixed)patchEntry.ActionList[mEntryList.SelectedIndex];
			((SwooshPanel)Parent).SwooshTo(EditorILPattern.Instance);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			MainPanel.Instance.Add(patchEntry);
			((SwooshPanel)Parent).SwooshTo(MainPanel.Instance);
		}

		public void Add(PatchAction pa)
		{
			if (!patchEntry.ActionList.Contains(pa))
				patchEntry.ActionList.Add(pa);
			LoadEntry(patchEntry);
		}

		public void LoadEntry(PatchEntry loadpe)
		{
			mEntryList.ClearItems();
			patchEntry = loadpe;
			if (loadpe == null)
			{
				txtPatchEntryName.Text = "DafaultName_" + new Random().Next(1000);
			}
			else
			{
				txtPatchEntryName.Text = loadpe.EntryName;
				foreach (PatchAction pa in loadpe.ActionList)
					mEntryList.AddItem(pa.DisplayName);
			}
			mEntryList.InvalidateChildren();
		}

		private void EditorEntry_Resize(object sender, EventArgs e)
		{
			const int space = 5;
			const int labelspace = 100 + 2 * space;

			txtPatchEntryName.Width = Width - (labelspace + space);
			mEntryList.Width = Width - (labelspace + space);
			mEntryList.Height = Height - (mEntryList.Top + space);

			btnOK.Top = Height - (btnOK.Height + space);
		}

		private void btnEditPatchAction_Click(object sender, EventArgs e)
		{
			if (mEntryList.SelectedIndex >= 0)
			{
				PatchAction pa = patchEntry.ActionList[mEntryList.SelectedIndex];
				switch (pa.PatchActionType)
				{
					case PatchActionType.ILMethodFixed:
						EditorILPattern.Instance.LoadPatchAction(pa as PatchActionILMethodFixed);
						break;
					case PatchActionType.ILMethodDynamic:
						Log.Write(Log.Level.Info, "ILMethodDynamic not implemented");
						return;
					case PatchActionType.ILDynamicScan:
						Log.Write(Log.Level.Info, "ILDynamicScan not implemented");
						return;
					case PatchActionType.AoBRawScan:
						Log.Write(Log.Level.Info, "AoBRawScan not implemented");
						return;
					default:
						return;
				}
				((SwooshPanel)Parent).SwooshTo(EditorILPattern.Instance);
			}
		}

		private void ReloadTable()
		{
			LoadEntry(patchEntry);
		}
	}
}
