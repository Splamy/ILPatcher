﻿using System;
using System.Collections.Generic;
using System.Text;
using MetroObjects;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace ILPatcher
{
	class InspectorHolder : MetroClass, IAcceptDropItems
	{
		private Brush _BackBrush;
		public override Color ColorTwo
		{
			set
			{
				_ColorTwo = value;
				_BackBrush = new SolidBrush(value);
				this.Invalidate();
			}
		}

		private Brush _GripBrush;
		public override Color ColorThree
		{
			set
			{
				_ColorThree = value;
				_GripBrush = new SolidBrush(value);
				this.Invalidate();
			}
		}

		private Brush _BackgrColor;

		private bool drawOnHover;
		private DragItem di;
		public DragItem DragItem
		{
			get { return di; }
			set { di = value; Invalidate(); }
		}

		private bool _AllowDrag = true;
		[DefaultValue(true)]
		public bool AllowDrag { get { return _AllowDrag; } set { _AllowDrag = value; } }

		private bool Drag;
		private bool TookElement;
		private Point offset;

		public event ItemDropEvent OnItemDropSuccess;
		public event ItemDropEvent OnItemDropFailed;

		public InspectorHolder()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		public bool DropToPreview(DragItem[] lbe)
		{
			drawOnHover = false;
			if (!AllowDrop) return false;

			di = lbe[0];
			if (OnItemDropSuccess != null)
				OnItemDropSuccess(lbe);
			Invalidate();
			return true;
		}

		public void SetPreview(Point pos)
		{
			drawOnHover = pos != DragItemHolder.NoPoint && AllowDrop;
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_BackgrColor == null)
				_BackgrColor = new SolidBrush(BackColor);
			if (_BackBrush == null)
				_BackBrush = new SolidBrush(_ColorTwo);
			if (_GripBrush == null)
				_GripBrush = new SolidBrush(_ColorThree);

			Graphics g = e.Graphics;

			g.FillRectangle(drawOnHover ? _GripBrush : _BackBrush, 0, 0, Width, Height); // draw the nice border
			RectangleF innerRect = new RectangleF(_Border, _Border, Width - (_Border * 2), Height - (_Border * 2));
			g.FillRectangle(_BackgrColor, innerRect); // fill in the inner ractangle with bg color

			if (di != null)
				di.Draw(g, innerRect);
			else
				g.DrawString("<<<empty>>>", Font, Brushes.Beige, _Border, _Border);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (AllowDrag && di != null)
			{
				Drag = true;
				TookElement = false;
				offset = new Point(e.X, e.Y);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!AllowDrag || !Drag || TookElement)
				return;
			if (e.X < offset.X + 10 && e.X > offset.X - 10 && e.Y < offset.Y + 10 && e.Y > offset.Y - 10)
				return;
			TookElement = true;
			Point location = FindForm().PointToClient(Parent.PointToScreen(Location));
			location.X = location.X + e.X - Width / 2;
			location.Y = location.Y + e.Y - Height / 2;
			DragItemHolder lbed = new DragItemHolder(new[] { di }, this, location);
			di = null;
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			Drag = false;
		}

		public void OnItemDropFailedReceiver(DragItem[] di)
		{
			if (OnItemDropFailed != null) // when special method should be called in case of fail
				OnItemDropFailed(di);
			else // otherwise just return the item where it was :)
				DragItem = di[0];
		}
	}
}
