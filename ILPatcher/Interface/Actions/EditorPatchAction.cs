using ILPatcher.Data;
using ILPatcher.Data.Actions;

namespace ILPatcher.Interface.Actions
{
	public abstract class EditorPatchAction<T> : EditorBase<PatchAction, T>, IEditorPatchAction where T : PatchAction
	{
		protected EditorPatchAction(DataStruct dataStruct) : base(dataStruct) { }
	}

	public interface IEditorPatchAction : IEditorPanel
	{
		void SetPatchData(PatchAction pPatchAction);
    }
}
