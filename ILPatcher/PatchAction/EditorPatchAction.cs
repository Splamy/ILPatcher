using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILPatcher
{
	abstract class EditorPatchAction : Control
	{
		public abstract void SetPatchAction(PatchAction pa);

		protected Action<PatchAction> ParentAddCallback;
	}
}
