using ILPatcher.Data.Finder;
using ILPatcher.Interface.General;
using System;

namespace ILPatcher.Interface.Finder
{
	public class EditorTargetFinder : EditorPanel<TargetFinder>
	{
		public override string PanelName { get { return "Default TargetFinder"; } }

		protected EditorTargetFinder() { /*Reserverd for VSDesigner*/ }
		protected EditorTargetFinder(Action<TargetFinder> pParentAddCallback)
			: base(pParentAddCallback)
		{ }
	}
}
