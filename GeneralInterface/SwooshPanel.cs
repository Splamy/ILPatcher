using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace ILPatcher
{
	public class SwooshPanel : Control
	{
		List<LayerLevel> TabList;
		LayerLevel selectedLayer
		{
			get { return TabList[selectedIndex]; }
			set { TabList[selectedIndex] = value; }
		}
		int selectedIndex;
		public const int PATHBOXHEIGHT = 30;

		Timer swooshTimer;
		bool swooshRight; // true if right, false is left
		int oldindex;
		int currentPos;

		public SwooshPanel()
		{
			TabList = new List<LayerLevel>();
			swooshTimer = new Timer();
			swooshTimer.Interval = 10;
			swooshTimer.Tick += swooshTimer_Tick;
		}

		void swooshTimer_Tick(object sender, EventArgs e)
		{
			int minspeed = Width / 50;
			float pos = currentPos * 2f / Width - 1;
			int speed = (int)Math.Round(Math.Pow(Width, (-.5 * pos * pos)) * Math.Log(Width, 1.5)) + minspeed;
			Parent.Text = speed.ToString();
			if (swooshRight) speed *= -1;
			TabList[oldindex].ctrl.Left += speed;
			TabList[selectedIndex].ctrl.Left += speed;
			currentPos = Math.Abs(TabList[oldindex].ctrl.Left);

			//opt: if in next step
			if (currentPos >= Width)
				ResizeAll();
		}

		public void AddPanel(Control c, string name)
		{
			TabList.Add(new LayerLevel(this, c, name));
			ResizeAll();
		}

		public void SwooshTo(int nwIndex)
		{
			if (selectedIndex == nwIndex) return;
			if (selectedIndex < nwIndex)
			{
				swooshRight = true;
				TabList[nwIndex].ctrl.Width = Width;
				TabList[nwIndex].ctrl.Left = Width;
			}
			else
			{
				swooshRight = false;
				TabList[nwIndex].ctrl.Width = Width;
				TabList[nwIndex].ctrl.Left = -Width;
			}
			TabList[nwIndex].ctrl.Top = PATHBOXHEIGHT + 1;
			TabList[nwIndex].ctrl.Height = Height - PATHBOXHEIGHT - 1;
			TabList[nwIndex].ctrl.SuspendLayout();
			//TabList[nwIndex].ctrl.Enabled = false;

			TabList[selectedIndex].ctrl.Dock = DockStyle.None;
			TabList[selectedIndex].ctrl.Top = PATHBOXHEIGHT + 1;
			TabList[selectedIndex].ctrl.Left = 0;
			TabList[selectedIndex].ctrl.Width = Width;
			TabList[selectedIndex].ctrl.Height = Height - PATHBOXHEIGHT - 1;
			TabList[selectedIndex].ctrl.SuspendLayout();
			//TabList[selectedIndex].ctrl.Enabled = false;

			for (int i = 0; i < TabList.Count; i++)
				TabList[i].btn.Enabled = i <= nwIndex;

			oldindex = selectedIndex;
			selectedIndex = nwIndex;
			swooshTimer.Start();
		}

		public void SwooshTo(Control nwControl)
		{
			for (int i = 0; i < TabList.Count; i++)
				if (TabList[i].ctrl == nwControl)
				{
					SwooshTo(i);
					return;
				}
		}

		protected override void OnResize(EventArgs e)
		{
			ResizeAll();
			base.OnResize(e);
		}

		private void ResizeAll()
		{
			swooshTimer.Stop();
			if (TabList.Count == 0) return;

			LayerLevel last = null;
			foreach (LayerLevel ll in TabList)
			{
				ll.btn.Index = TabList.IndexOf(ll);
				if (last == null)
					ll.btn.Location = new Point(1, 1);
				else
					ll.btn.Location = new Point(last.lbl.Right - 1, 1);
				ll.lbl.Location = new Point(ll.btn.Right - 1, 1);

				if (ll == selectedLayer)
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
	}

	class LayerLevel
	{
		public Control ctrl;
		public CustomButton btn;
		public Label lbl;
		private SwooshPanel parent;

		public LayerLevel(SwooshPanel parent, Control c, string name)
		{
			this.parent = parent;
			ctrl = c;
			ctrl.Parent = parent;
			btn = new CustomButton(name);
			btn.Parent = parent;
			btn.Click += btn_Click;
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
			};
		}

		void btn_Click(object sender, EventArgs e)
		{
			parent.SwooshTo(((CustomButton)sender).Index);
		}

		public override string ToString()
		{
			return btn.Left + " " + lbl.Left + " " + btn.Text;
		}
	}

	class CustomButton : Button
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
