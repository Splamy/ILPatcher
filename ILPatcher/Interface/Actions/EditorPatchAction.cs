using ILPatcher.Data.Actions;
using System;
using System.Windows.Forms;

namespace ILPatcher.Interface.Actions
{
	public class EditorPatchAction : UserControl
	{
		public virtual string PanelName { get { return "Default Panelname"; } }
		protected readonly Action<PatchAction> ParentAddCallback;

		public virtual void SetPatchAction(PatchAction pPatchAction)
		{ throw new NotImplementedException(); }

		private EditorPatchAction() { /*Reserverd for VSDesigner*/ }

		protected EditorPatchAction(Action<PatchAction> pParentAddCallback)
		{
			ParentAddCallback = pParentAddCallback;
		}
	}
}
