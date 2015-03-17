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

		Action<PatchEntry> callbackAdd;

		public EditorEntry(Action<PatchEntry> _cbAdd)
		{
			InitializeComponent();

			callbackAdd = _cbAdd;
		}

		private void btnAddILPatch_Click(object sender, EventArgs e)
		{
			((SwooshPanel)Parent).PushPanel(new EditorILPattern(Add), "PatchAction: ILMethodFixed");
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			callbackAdd(patchEntry);
			((SwooshPanel)Parent).SwooshBack();
		}

		public void Add(PatchAction pa)
		{
			patchEntry.Add(pa);
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
					EditorILPattern eilp = new EditorILPattern(Add);
					eilp.LoadPatchAction((PatchActionILMethodFixed)pa);
					((SwooshPanel)Parent).PushPanel(eilp, "PatchAction: ILMethodFixed");
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
			}
		}

		private void ReloadTable()
		{
			LoadEntry(patchEntry);
		}
	}
}
