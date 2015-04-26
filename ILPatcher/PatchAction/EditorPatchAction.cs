using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILPatcher
{
	public abstract class EditorPatchAction : Control
	{
		public abstract string PanelName { get; }
		protected readonly Action<PatchAction> ParentAddCallback;

		public abstract void SetPatchAction(PatchAction pPatchAction);

		public EditorPatchAction(Action<PatchAction> pParentAddCallback)
		{
			ParentAddCallback = pParentAddCallback;
		}
	}
}
