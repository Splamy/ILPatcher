using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ILPatcher.Interface.General
{
	public partial class AddRemoveControl : Control
	{
		private IContainer components = null;
		private Button btnAdd;
		private Button btnRemove;
		private Button btnEdit;

		public Action OnAdd;
		public Action OnRemove;
		public Action OnEdit;

		private const int btnWidth = 80;
		private const int space = 5;

		public AddRemoveControl()
		{
			InitializeComponent();

			btnAdd.Click += (s, e) => OnAdd();
			btnRemove.Click += (s, e) => OnRemove();
			btnEdit.Click += (s, e) => OnEdit();
		}

		protected override void OnResize(EventArgs e)
		{
			int btnFinalWidth = (this.Width < 3 * btnWidth + 4 * space) ? ((this.Width - (4 * space)) / 3) : btnWidth;
			btnAdd.Left = (0 * btnFinalWidth) + (1 * space);
			btnRemove.Left = (1 * btnFinalWidth) + (2 * space);
			btnEdit.Left = (2 * btnFinalWidth) + (3 * space);
			foreach (Button btn in components.Components)
				btn.Width = btnFinalWidth;
			base.OnResize(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.SuspendLayout();

			components = new Container();
			components.Add(btnAdd);
			components.Add(btnRemove);
			components.Add(btnEdit);

			foreach (Button btn in components.Components)
			{
				btn.BackColor = Color.FromArgb(128, 255, 255);
				btn.FlatAppearance.BorderColor = Color.Blue;
				btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 255, 255);
				btn.FlatStyle = FlatStyle.Popup;
				btn.UseVisualStyleBackColor = false;
				btn.Top = space;
			}

			this.btnAdd.Text = "Add";
			this.btnRemove.Text = "Remove";
			this.btnEdit.Text = "Edit";

			this.ResumeLayout(false);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
		}
	}
}
