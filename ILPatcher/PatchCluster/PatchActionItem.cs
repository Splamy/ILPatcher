using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MetroObjects;

namespace ILPatcher
{
	class PatchActionItem : DragItem // todo change metroobjects library to use bufferimages
	{
		PatchAction pa;
		int bufferedheight = 0;

		public PatchActionItem(PatchAction _pa)
		{
			pa = _pa;
		}

		public override void Draw(Graphics g, System.Drawing.RectangleF rec)
		{
			int split = 0;
			switch (pa.PatchActionType)
			{
			case PatchActionType.ILMethodFixed:
				split = (int)g.MeasureString(pa.ActionName, Font, Width).Height;
				g.DrawString(pa.ActionName, Font, Brushes.Black, rec.Location);
				break;
			case PatchActionType.ILMethodDynamic:
				break;
			case PatchActionType.ILDynamicScan:
				break;
			case PatchActionType.AoBRawScan:
				break;
			case PatchActionType.ILMethodCreator:
				PatchActionMethodCreator pamc = (PatchActionMethodCreator)pa;
				split = (int)g.MeasureString(pamc.ActionName, Font, Width).Height + 2;
				g.DrawString(pamc.ActionName, Font, Brushes.Black,
					new RectangleF(rec.Location.X + 10, rec.Location.Y + 1, Width, split));
				if (pamc.FillAction != null)
					g.DrawString(pamc.FillAction.ActionName, Font, Brushes.Black,
						new RectangleF(rec.Location.X + 10, rec.Location.Y + split, Width, Height));
				else ;
				g.FillRectangle(Brushes.DimGray, rec.Location.X + 4, rec.Location.Y + 7, 3, split);
				g.DrawLine(Pens.Gray, 15, rec.Location.Y + split, rec.Right, rec.Location.Y + split);
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.FillEllipse(Brushes.DimGray, rec.Location.X + 2, rec.Location.Y + 4, 6, 6);
				g.FillEllipse(Brushes.DimGray, rec.Location.X + 2, rec.Location.Y + 4 + split, 6, 6);
				// important: do NOT add this to split up before
				split += (int)g.MeasureString(pamc.FillAction.ActionName, Font, Width).Height + 1;
				break;
			default:
				break;
			}
			bufferedheight = split;

			RefreshHeight(g, (int)rec.Width);
		}

		public override void RefreshHeight(Graphics g, int nWidth)
		{
			if (this.Width != nWidth)
			{
				Width = nWidth;
				Height = bufferedheight;
			}
		}
	}
}
