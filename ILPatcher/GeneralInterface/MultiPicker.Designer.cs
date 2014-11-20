namespace ILPatcher
{
	partial class MultiPicker
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_Select = new System.Windows.Forms.Button();
			this.structureViever1 = new ILPatcher.StructureViewer();
			this.SuspendLayout();
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btn_Cancel.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btn_Cancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_Cancel.Location = new System.Drawing.Point(12, 422);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(87, 21);
			this.btn_Cancel.TabIndex = 20;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = false;
			this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
			// 
			// btn_Select
			// 
			this.btn_Select.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btn_Select.Enabled = false;
			this.btn_Select.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btn_Select.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btn_Select.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_Select.Location = new System.Drawing.Point(105, 422);
			this.btn_Select.Name = "btn_Select";
			this.btn_Select.Size = new System.Drawing.Size(87, 21);
			this.btn_Select.TabIndex = 21;
			this.btn_Select.Text = "Select";
			this.btn_Select.UseVisualStyleBackColor = false;
			this.btn_Select.Click += new System.EventHandler(this.btn_Select_Click);
			// 
			// structureViever1
			// 
			this.structureViever1.Dock = System.Windows.Forms.DockStyle.Top;
			this.structureViever1.ImageIndex = 0;
			this.structureViever1.Location = new System.Drawing.Point(0, 0);
			this.structureViever1.Name = "structureViever1";
			this.structureViever1.PathSeparator = ".";
			this.structureViever1.SelectedImageIndex = 0;
			this.structureViever1.Size = new System.Drawing.Size(372, 416);
			this.structureViever1.ViewElements = ILPatcher.StructureView.all;
			this.structureViever1.TabIndex = 22;
			// 
			// MultiPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(372, 451);
			this.Controls.Add(this.structureViever1);
			this.Controls.Add(this.btn_Select);
			this.Controls.Add(this.btn_Cancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "MultiPicker";
			this.Text = "MultiPicker";
			this.Resize += new System.EventHandler(this.MultiPicker_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btn_Cancel;
		private System.Windows.Forms.Button btn_Select;
		private StructureViewer structureViever1;
	}
}