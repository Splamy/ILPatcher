using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MetroObjects;

namespace ILPatcher
{
	class PatchActionItem : DragItem
	{
		string itemtext;

		public PatchActionItem(PatchAction pa)
		{

		}

		public override void Draw(System.Drawing.Graphics g, System.Drawing.RectangleF rec)
		{
			int split = (int)g.MeasureString(itemtext, Font).Width;
			RefreshHeight(g, (int)rec.Width);


		}

		public override void RefreshHeight(System.Drawing.Graphics g, int nWidth)
		{
			if (this.Width != nWidth)
			{
				Width = nWidth;
				Height = Font.Height + 1;
			}
		}

		private void GenerateText(PatchAction pa)
		{
			switch (pa.PatchActionType)
			{
			case PatchActionType.ILMethodFixed:
				PatchActionILMethodFixed pamf = (PatchActionILMethodFixed)pa;
				
				break;
			case PatchActionType.ILMethodDynamic:
				break;
			case PatchActionType.ILDynamicScan:
				break;
			case PatchActionType.AoBRawScan:
				break;
			case PatchActionType.ILMethodCreator:
				break;
			default:
				break;
			}
		}
	}
}
