using MetroObjects;
using System.Drawing;

namespace ILPatcher.Utility
{
	public class ErrorLoggerItem : DragItem
	{
		public string error; // HACK
		private Log.Level level;

		public ErrorLoggerItem(Log.Level _level, string _error)
		{
			level = _level;
			error = _error;
		}

		protected override void DrawBuffer(Graphics g)
		{
			RectangleF layout = new RectangleF(PointF.Empty, Size);
			switch (level)
			{
			case Log.Level.Info: g.FillRectangle(Brushes.LightBlue, layout); break;
			case Log.Level.Careful: g.FillRectangle(Brushes.Yellow, layout); break;
			case Log.Level.Warning: g.FillRectangle(Brushes.Orange, layout); break;
			case Log.Level.Error: g.FillRectangle(Brushes.Coral, layout); break;
			default: break;
			}

			g.DrawString(error, Font, Brushes.Black, layout);
		}

		protected override int GetHeightFromWidth(int width)
		{
			Graphics g = GetBufferGraphics();
			g.MeasureString(error, Font, width);
			return (int)g.MeasureString(error, Font, width).Height;
		}
	}
}
