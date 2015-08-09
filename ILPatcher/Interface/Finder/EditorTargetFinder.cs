using ILPatcher.Data;
using ILPatcher.Data.Finder;

namespace ILPatcher.Interface.Finder
{
	public abstract class EditorTargetFinder<T> : EditorBase<TargetFinder, T>, IEditorTargetFinder where T : TargetFinder
	{
		protected EditorTargetFinder(DataStruct dataStruct) : base(dataStruct) { }
	}

	public interface IEditorTargetFinder : IEditorPanel
	{
		void SetPatchData(TargetFinder pPatchAction);
	}
}
