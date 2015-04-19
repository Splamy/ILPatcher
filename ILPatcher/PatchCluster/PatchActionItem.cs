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

		protected override void DrawBuffer(Graphics g)
		{
			int split = 0;
			switch (pa.PatchActionType)
			{
			case PatchActionType.ILMethodFixed:
				bufferedheight = (int)g.MeasureString(pa.ActionName, Font, Size.Width).Height;

				SetWidth(Size.Width);

				g.DrawString(pa.ActionName, Font, Brushes.Black, 0, 0);
				break;
			case PatchActionType.ILMethodDynamic:
				break;
			case PatchActionType.ILDynamicScan:
				break;
			case PatchActionType.AoBRawScan:
				break;
			case PatchActionType.ILMethodCreator:
				PatchActionMethodCreator pamc = (PatchActionMethodCreator)pa;
				int split1 = (int)g.MeasureString(pamc.ActionName, Font, Size.Width).Height + 2;
				if (pamc.FillAction != null)
					bufferedheight = split1 + (int)g.MeasureString(pamc.FillAction.ActionName, Font, Size.Width).Height + 1;
				else
					bufferedheight = split1;

				SetWidth(Size.Width);

				g.FillRectangle(Brushes.DimGray,  4,  7, 3, split1);
				g.DrawLine(Pens.Gray, 15, split1, Size.Width, split1);
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.FillEllipse(Brushes.DimGray,  2,  4, 6, 6);
				g.FillEllipse(Brushes.DimGray,  2,  4 + split1, 6, 6);
				g.DrawString(pamc.ActionName, Font, Brushes.Black, new RectangleF(10, 1, Size.Width, Size.Height));
				if (pamc.FillAction != null)
					g.DrawString(pamc.FillAction.ActionName, Font, Brushes.Black, new RectangleF(10, split1, Size.Width, Size.Height));
				break;
			default:
				break;
			}
			bufferedheight = split;
		}

		protected override int GetHeightFromWidth(int width)
		{
			return bufferedheight;
		}
	}
}
