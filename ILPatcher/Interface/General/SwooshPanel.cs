using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ILPatcher.Interface.General
{
	public class SwooshPanel : Control
	{
		//private List<LayerLevel> TabList;
		private Stack<LayerLevel> TabList;
		private LayerLevel WorkLayer;
		public const int PATHBOXHEIGHT = 30;

		Timer swooshTimer;
		bool swooshRight; // true if right, false is left
		int currentPos;

		public SwooshPanel()
		{
			TabList = new Stack<LayerLevel>();
			swooshTimer = new Timer();
			swooshTimer.Interval = 10;
			swooshTimer.Tick += swooshTimer_Tick;
		}

		void swooshTimer_Tick(object sender, EventArgs e)
		{
			int minspeed = Width / 50;
			float pos = currentPos * 2f / Width - 1;
			int speed = (int)Math.Round(Math.Pow(Width, (-.5 * pos * pos)) * Math.Log(Width, 1.5)) + minspeed;
			if (swooshRight) speed *= -1;
			WorkLayer.ctrl.Left += speed;
			TabList.Peek().ctrl.Left += speed;
			currentPos = Math.Abs(WorkLayer.ctrl.Left);

			//opt: if in next step
			if (currentPos >= Width)
			{
				if (!swooshRight)
				{
					WorkLayer.Dispose();
					WorkLayer = null;
				}
				ResizeAll(true);
			}
		}

		public void PushPanel(Control c, string name)
		{
			if (TabList.Count > 0)
			{
				WorkLayer = TabList.Peek();
				TabList.Push(new LayerLevel(this, c, name));
				SwooshTo(TabList.Count);
			}
			else
			{
				TabList.Push(new LayerLevel(this, c, name));
				ResizeAll(true);
			}
		}

		public void SwooshBack()
		{
			if (TabList.Count > 1)
			{
				SwooshTo(TabList.Count - 2);
			}
		}

		public void SwooshBackTo(int nwIndex)
		{
			int selectedIndex = TabList.Count - 1;
			if (nwIndex < selectedIndex && TabList.Count > 1)
			{
				SwooshTo(nwIndex);
			}
		}

		private void SwooshTo(int nwIndex)
		{
			int selectedIndex = TabList.Count - 1;
			if (selectedIndex == nwIndex) return;
			if (nwIndex > selectedIndex)
			{
				swooshRight = true; // controls move to the left
				TabList.Peek().ctrl.Width = Width;
				TabList.Peek().ctrl.Left = Width;
			}
			else // Layer is new
			{
				swooshRight = false; // controls move to the right
				WorkLayer = TabList.Pop();
				while (TabList.Count - 1 > nwIndex)
					TabList.Pop().Dispose();
				TabList.Peek().ctrl.Width = Width;
				TabList.Peek().ctrl.Left = -Width;
			}
			TabList.Peek().ctrl.Top = PATHBOXHEIGHT + 1;
			TabList.Peek().ctrl.Height = Height - PATHBOXHEIGHT - 1;
			TabList.Peek().ctrl.SuspendLayout();
			//TabList[nwIndex].ctrl.Enabled = false;

			WorkLayer.ctrl.Dock = DockStyle.None;
			WorkLayer.ctrl.Top = PATHBOXHEIGHT + 1;
			WorkLayer.ctrl.Left = 0;
			WorkLayer.ctrl.Width = Width;
			WorkLayer.ctrl.Height = Height - PATHBOXHEIGHT - 1;
			WorkLayer.ctrl.SuspendLayout();
			//TabList[selectedIndex].ctrl.Enabled = false;

			//for (int i = 0; i < TabList.Count; i++)
			//	TabList[i].btn.Enabled = i <= nwIndex;

			swooshTimer.Start();
		}

		protected override void OnResize(EventArgs e)
		{
			ResizeAll(false);
			base.OnResize(e);
		}

		private void ResizeAll(bool stopTimer)
		{
			if (stopTimer) swooshTimer.Stop();
			if (TabList.Count == 0) return;

			LayerLevel last = null;
			int index = 0;
			foreach (LayerLevel ll in TabList.Reverse())
			{
				ll.btn.Index = index++;
				if (last == null)
					ll.btn.Location = new Point(1, 1);
				else
					ll.btn.Location = new Point(last.lbl.Right - 1, 1);
				ll.lbl.Visible = true;
				ll.btn.Visible = true;
				ll.lbl.Location = new Point(ll.btn.Right - 1, 1);


				if (ll == TabList.Peek())
				{
					ll.ctrl.Dock = DockStyle.Bottom;
					ll.ctrl.Height = Height - PATHBOXHEIGHT - 1;
					ll.ctrl.ResumeLayout();
					ll.btn.BackColor = Color.LightBlue;
				}
				else
				{
					ll.ctrl.Dock = DockStyle.None;
					ll.ctrl.Left = -ll.ctrl.Width;
					ll.btn.BackColor = SystemColors.Control;
				}
				last = ll;
			}
		}

		private class LayerLevel
		{
			public Control ctrl;
			public CustomButton btn;
			public Label lbl;
			private SwooshPanel parent;

			public LayerLevel(SwooshPanel _parent, Control c, string name)
			{
				parent = _parent;
				ctrl = c;
				ctrl.Parent = _parent;
				btn = new CustomButton(name);
				btn.Parent = _parent;
				btn.Click += btn_Click;
				btn.Visible = false;
				lbl = new Label()
				{
					AutoSize = true,
					BorderStyle = BorderStyle.FixedSingle,
					TextAlign = ContentAlignment.MiddleCenter,
					Text = ">",
					Parent = parent,
					Width = 0,
					MinimumSize = new Size(0, SwooshPanel.PATHBOXHEIGHT),
					MaximumSize = new Size(0, SwooshPanel.PATHBOXHEIGHT),
					Visible = false,
				};
			}

			void btn_Click(object sender, EventArgs e)
			{
				parent.SwooshBackTo(((CustomButton)sender).Index);
			}

			public override string ToString()
			{
				return btn.Left + " " + lbl.Left + " " + btn.Text;
			}

			public void Dispose()
			{
				parent.Controls.Remove(btn);
				parent.Controls.Remove(lbl);
				parent.Controls.Remove(ctrl);
				btn.Dispose();
				lbl.Dispose();
				ctrl.Dispose();
			}
		}

		private class CustomButton : Button
		{
			public int Index;
			public CustomButton(string name)
			{
				AutoSize = true;
				FlatStyle = FlatStyle.Flat;
				Text = name;
				FlatAppearance.BorderColor = SystemColors.WindowFrame;
				SetStyle(ControlStyles.Selectable, false);
				MinimumSize = new Size(0, SwooshPanel.PATHBOXHEIGHT);
				MaximumSize = new Size(0, SwooshPanel.PATHBOXHEIGHT);
				Width = 0;
				Enabled = false;
			}
		}
	}
}
