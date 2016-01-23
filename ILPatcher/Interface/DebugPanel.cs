using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using ILPatcher.Utility;

namespace ILPatcher.Interface
{
	class DebugPanel : Swoosh.Panel
	{
		public DebugPanel()
		{
			this.AutoScroll = true;
			//FlowDirection = FlowDirection.TopDown;
			//WrapContents = false;

			var grid = new GridLineManager(this, false);
			grid.ElementDistance = 0;
			grid.AddElementFilling(grid.AddLineFixed(100), new Finder.EditorFinderClassByName(null), GlobalLayout.MinFill);
			grid.AddElementFilling(grid.AddLineFixed(100), new Finder.EditorFinderClassByName(null), GlobalLayout.MinFill);
			grid.AddElementFilling(grid.AddLineFixed(100), new Finder.EditorFinderClassByName(null), GlobalLayout.MinFill);
		}
	}
}