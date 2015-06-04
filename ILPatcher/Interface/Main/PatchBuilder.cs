using ILPatcher.Data.Actions;
using ILPatcher.Data.General;
using ILPatcher.Interface.Actions;
using ILPatcher.Interface.General;
using ILPatcher.Utility;
using Mono.Cecil;
using System;
using System.Windows.Forms;

namespace ILPatcher.Interface.Main
{
	public partial class PatchBuilder : UserControl
	{
		public PatchCluster PatchCluster { get; private set; }
		private Action<PatchCluster> callbackAdd;
		private AssemblyDefinition assemblyDefinition;

		// TODO do patchfinder

		public PatchBuilder(Action<PatchCluster> pCallbackAdd, AssemblyDefinition pAssemblyDefinition)
		{
			InitializeComponent();

			callbackAdd = pCallbackAdd;
			assemblyDefinition = pAssemblyDefinition;

			LoadDropdownList();
		}

		public void LoadCluster(PatchCluster patchCluster)
		{
			PatchCluster = patchCluster;
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
			if (PatchCluster == null || PatchCluster.PatchAction == null)
				Enum.TryParse<PatchActionType>(comboBox2.Text, out patchActionType);
			else
			{
				patchAction = PatchCluster.PatchAction;
				patchActionType = patchAction.PatchActionType;
			}

			EditorPatchAction editorform;
			switch (patchActionType)
			{
			case PatchActionType.ILMethodFixed:
				editorform = new EditorILPattern(SetPatchAction);
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
				editorform = new EditorMethodCreator(SetPatchAction, assemblyDefinition);
				break;
			default:
				return;
			}

			editorform.SetPatchAction(patchAction);
			((SwooshPanel)Parent).PushPanel(editorform, "PatchAction: " + editorform.PanelName);
		}

		private void SetPatchAction(PatchAction patchAction)
		{
			if (PatchCluster == null)
				PatchCluster = new PatchCluster();
			PatchCluster.PatchAction = patchAction;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (PatchCluster != null)
				callbackAdd(PatchCluster);
			((SwooshPanel)Parent).SwooshBack();
		}
	}
}
