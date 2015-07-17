using ILPatcher.Data;
using System;
using System.Windows.Forms;

namespace ILPatcher.Interface.General
{
	public abstract class EditorPanel<T> : UserControl
	{
		protected readonly DataStruct dataStruct;
		public virtual string PanelName { get { return "Default EditorPanel"; } }
		public virtual bool IsInline { get { return false; } }

		public virtual void SetPatchData(T pPatchAction) { throw new NotImplementedException(); }

		protected EditorPanel() { /*Reserverd for VSDesigner*/ }
		protected EditorPanel(DataStruct dataAssociation) { dataStruct = dataAssociation; }
	}
}
