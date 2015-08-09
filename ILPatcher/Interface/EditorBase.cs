using ILPatcher.Data;
using System;

namespace ILPatcher.Interface
{
	public abstract class EditorBase<Kind, Spec> : Swoosh.Control, IEditorPanel where Spec : class, Kind
	{
		protected readonly DataStruct dataStruct;
		protected Spec myData;

		public abstract string PanelName { get; }
		public abstract bool IsInline { get; }

		public abstract Kind CreateNewEntryPart();
		public void SetPatchData(Kind pPatchAction)
		{
			if (pPatchAction == null) throw new ArgumentNullException(nameof(pPatchAction));
			var tmp = pPatchAction as Spec;
			if (tmp == null) throw new InvalidOperationException("The passed parameter is not designed for this interface element");
			myData = tmp;
			OnPatchDataSet();
		}
		protected abstract void OnPatchDataSet();

		protected EditorBase(DataStruct dataAssociation) { dataStruct = dataAssociation; }
	}

	public interface IEditorPanel
	{
		string PanelName { get; }
		bool IsInline { get; }
	}
}
