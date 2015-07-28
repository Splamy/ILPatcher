using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILPatcher.Interface.Finder
{
	class EditorFinderByName : EditorTargetFinder
	{
		private void InitializeGridLineManager()
		{
			var grid = new GridLineManager(this, true);
		}
	}
}
