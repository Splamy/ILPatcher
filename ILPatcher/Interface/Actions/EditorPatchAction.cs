using ILPatcher.Data.Actions;
using System;
using System.Windows.Forms;
using ILPatcher.Interface.General;

namespace ILPatcher.Interface.Actions
{
	public class EditorPatchAction : EditorPanel<PatchAction>
	{
		public override string PanelName { get { return "Default PatchAction"; } }

		protected EditorPatchAction() { /*Reserverd for VSDesigner*/ }
		protected EditorPatchAction(Action<PatchAction> pParentAddCallback)
			: base(pParentAddCallback)
		{ }
	}
}
