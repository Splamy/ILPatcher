using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ILPatcher.Interface.General
{
	public abstract class EditorPanel<T> : UserControl
	{
		protected readonly Action<T> ParentAddCallback;
		public virtual string PanelName { get { return "Default EditorPanel"; } }
		public virtual bool IsInline { get { return false; } }

		public virtual void SetPatchData(T pPatchAction) { throw new NotImplementedException(); }

		protected EditorPanel() { /*Reserverd for VSDesigner*/ }
		protected EditorPanel(Action<T> pParentAddCallback) { ParentAddCallback = pParentAddCallback; }
	}
}
