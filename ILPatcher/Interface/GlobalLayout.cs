using System;
using System.Windows.Forms;
using System.Drawing;

namespace ILPatcher.Interface
{
	static class GlobalLayout
	{
		public const int LineHeight = 25;
		public const int LabelWidth = 120;
		public const int MinFill = 50;

		public static Button GenMetroButton(string text, EventHandler clickEvent)
		{
			Button btn = new Button();
			btn.FlatStyle = FlatStyle.Popup;
			btn.BackColor = Color.PaleTurquoise;
			btn.Text = text;
			btn.Click += clickEvent;
			return btn;
		}

		public static Label GenMetroLabel(string text)
		{
			Label lbl = new Label();
			lbl.BorderStyle = BorderStyle.FixedSingle;
			lbl.Text = text;
			lbl.TextAlign = ContentAlignment.MiddleLeft;
			return lbl;
		}
	}
}
