using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILPatcher
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
