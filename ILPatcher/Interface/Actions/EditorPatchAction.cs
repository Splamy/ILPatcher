using ILPatcher.Data;
using ILPatcher.Data.Actions;

namespace ILPatcher.Interface.Actions
{
	public abstract class EditorPatchAction<T> : EditorBase<T> where T : PatchAction
	{
		protected EditorPatchAction(DataStruct dataStruct) : base(dataStruct) { }
	}
}
