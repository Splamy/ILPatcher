using ILPatcher.Data;
using ILPatcher.Data.Finder;

namespace ILPatcher.Interface.Finder
{
	public abstract class EditorTargetFinder<T> : EditorBase<T> where T : TargetFinder
	{
		protected EditorTargetFinder(DataStruct dataStruct) : base(dataStruct) { }
	}
}
