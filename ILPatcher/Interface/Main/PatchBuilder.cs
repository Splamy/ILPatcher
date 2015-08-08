using ILPatcher.Data;
using ILPatcher.Data.Actions;
using ILPatcher.Interface.Actions;
using ILPatcher.Utility;
using Mono.Cecil;
using System;
using System.Windows.Forms;

namespace ILPatcher.Interface.Main
{
	public partial class PatchBuilder : UserControl
	{
		public PatchEntry PatchEntry { get; private set; }
		private bool editMode;
		private DataStruct dataStruct;

		// TODO: do patchfinder

		public PatchBuilder(DataStruct pDataStruct)
		{
			InitializeComponent();

			editMode = false;

			dataStruct = pDataStruct;
			LoadDropdownList();
		}

		public void LoadEntry(PatchEntry pPatchEntry)
		{
			editMode = true;
			PatchEntry = pPatchEntry;
		}

		private void LoadDropdownList()
		{
			comboBox2.Items.Clear();
			foreach (string eName in Enum.GetNames(typeof(PatchActionType)))
			{
				comboBox2.Items.Add(eName);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			SwooshToAction();
		}

		private void SwooshToAction()
		{
			PatchAction patchAction = null;
			PatchActionType patchActionType;
			if (PatchEntry == null || PatchEntry.PatchAction == null)
			{
				if (!Enum.TryParse(comboBox2.Text, out patchActionType))
				{
					throw new InvalidOperationException("PatchActionType not found");
				}
			}
			else
			{
				patchAction = PatchEntry.PatchAction;
				patchActionType = patchAction.PatchActionType;
			}

			IEditorPatchAction editorform;
			switch (patchActionType)
			{
			case PatchActionType.ILMethodFixed:
				editorform = new EditorILPattern(dataStruct);
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
			case PatchActionType.ILMethodCreator:
				editorform = new EditorMethodCreator(dataStruct);
				break;
			default:
				return;
			}

			editorform.SetPatchData(patchAction);
			((SwooshPanel)Parent).PushPanel((Control)editorform, "PatchAction: " + editorform.PanelName);
		}

		private void SetPatchAction(PatchAction patchAction)
		{
			if (PatchEntry == null)
				PatchEntry = new PatchEntry(dataStruct);
			PatchEntry.PatchAction = patchAction;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (editMode && PatchEntry != null)
				dataStruct.PatchEntryList.Add(PatchEntry);
			((SwooshPanel)Parent).SwooshBack();
		}
	}
}
