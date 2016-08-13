using ILPatcher.Data;
using System;
using System.Windows.Forms;

namespace ILPatcher.Interface
{
	[EditorAttributes(null)]
	public abstract class EditorBase<TSpec> : EditorPanel where TSpec : EntryBase
	{
		protected DataStruct dataStruct { get; }
		protected TSpec myData { get; set; }

		public override sealed void SetPatchData(EntryBase pPatchAction)
		{
			if (pPatchAction == null) throw new ArgumentNullException(nameof(pPatchAction));
			var tmp = pPatchAction as TSpec;
			if (tmp == null) throw new InvalidOperationException("The passed parameter is not designed for this interface element");
			myData = tmp;
			OnPatchDataSet();
		}
		protected abstract void OnPatchDataSet();

		protected EditorBase(DataStruct dataAssociation) { dataStruct = dataAssociation; }
	}

	public abstract class EditorPanel : Swoosh.Control
	{
		public virtual string PanelName => EditorFactory.GetEditorName(GetType());
		public abstract bool FixedHeight { get; }
		public abstract int DefaultHeight { get; }

		public abstract void SetPatchData(EntryBase pPatchAction);
	}
}
