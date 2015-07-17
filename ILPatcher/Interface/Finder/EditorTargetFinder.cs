using ILPatcher.Data;
using ILPatcher.Data.Finder;
using ILPatcher.Interface.General;

namespace ILPatcher.Interface.Finder
{
	public class EditorTargetFinder : EditorPanel<TargetFinder>
	{
		public override string PanelName { get { return "Default TargetFinder"; } }

		protected EditorTargetFinder() { /*Reserverd for VSDesigner*/ }
		protected EditorTargetFinder(DataStruct dataStruct)
			: base(dataStruct)
		{ }
	}
}
