using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

using MetroObjects;
using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public class ErrorLoggerItem : DragItem
	{
		private string error;
		private Log.Level level;

		public ErrorLoggerItem(Log.Level _level, string _error)
		{
			level = _level;
			error = _error;
		}

		public override void Draw(System.Drawing.Graphics g, System.Drawing.RectangleF rec)
		{
			RefreshHeight(g, (int)rec.Width);

			switch (level)
			{
			case Log.Level.Info: g.FillRectangle(Brushes.LightBlue, rec); break;
			case Log.Level.Careful: g.FillRectangle(Brushes.Yellow, rec); break;
			case Log.Level.Warning: g.FillRectangle(Brushes.Orange, rec); break;
			case Log.Level.Error: g.FillRectangle(Brushes.Coral, rec); break;
			default:
				break;
			}

			g.DrawString(error, Font, Brushes.Black, rec);
		}

		public override void RefreshHeight(System.Drawing.Graphics g, int nWidth)
		{
			if (this.Width != nWidth)
			{
				Width = nWidth;
				g.MeasureString(error, Font, Width);
				Height = (int)g.MeasureString(error, Font, Width).Height;
			}
		}
	}
}
