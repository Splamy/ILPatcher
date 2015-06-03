using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Mono.Cecil;

namespace ILPatcher.PatchCluster
{
	public partial class PatchBuilder : UserControl
	{
		private Action<PatchCluster> callbackAdd;
		private AssemblyDefinition assemblyDefinition;

		public PatchBuilder(Action<PatchCluster> pCallbackAdd, AssemblyDefinition pAssemblyDefinition)
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			
		}

		private void PushToEditor(PatchActionType pat)
		{
			PatchAction pa = patchCluster.ActionList[mClusterList.SelectedIndex];
			EditorPatchAction editorform;
			switch (pa.PatchActionType)
			{
			case PatchActionType.ILMethodFixed:
				editorform = new EditorILPattern(Add);
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
				editorform = new EditorMethodCreator(Add, assemblyDefinition);
				break;
			default:
				return;
			}
			editorform.SetPatchAction((PatchActionILMethodFixed)pa);
			((SwooshPanel)Parent).PushPanel(editorform, editorform.PanelName);
		}
	}
}
