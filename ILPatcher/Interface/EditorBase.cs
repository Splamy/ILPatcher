using ILPatcher.Data;
using System;

namespace ILPatcher.Interface
{
	[EditorAttributes(null)] // TODO: check ovverriding behaviour (eg. base (1:true, 2:true, 3:true) deriver (2:false) - meaning only overriding one attribute)
	public abstract class EditorBase<Kind, Spec> : Swoosh.Control, IEditorPanel where Spec : class, Kind
	{
		protected readonly DataStruct dataStruct;
		protected Spec myData;

		public string PanelName => EditorFactory.GetEditorName(GetType());
		public bool IsInline => EditorFactory.IsInline(GetType());

		public void CreateNewEntryPart() => SetPatchData(GetNewEntryPart());
		protected abstract Kind GetNewEntryPart();
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

		void CreateNewEntryPart();
    }
}
