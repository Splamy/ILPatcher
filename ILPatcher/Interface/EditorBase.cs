using ILPatcher.Data;
using System;

namespace ILPatcher.Interface
{
	[EditorAttributes(null)] // TODO: check ovverriding behaviour (eg. base (1:true, 2:true, 3:true) deriver (2:false) - meaning only overriding one attribute)
	public abstract class EditorBase<TSpec> : Swoosh.Control, IEditorPanel<TSpec> where TSpec : EntryBase
	{
		protected DataStruct dataStruct { get; }
		protected TSpec myData { get; set; }

		public string PanelName => EditorFactory.GetEditorName(GetType());
		public bool IsInline => EditorFactory.IsInline(GetType());

		public void SetPatchData(EntryBase pPatchAction)
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

	public interface IEditorPanel<out T>
	{
		string PanelName { get; }
		bool IsInline { get; }

		void SetPatchData(EntryBase pPatchAction);
	}
}
