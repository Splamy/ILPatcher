namespace ILPatcher
{
	partial class PatchQuestionWindow
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
			this.label1 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnMerge = new System.Windows.Forms.Button();
			this.btnFresh = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(587, 37);
			this.label1.TabIndex = 0;
			this.label1.Text = "It seems your current binary file is already modded.\r\nDo you want to merge your c" +
    "urrent patches or apply them to a unmodified version (the backup file will be co" +
    "pied for you) ?";
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnCancel.Location = new System.Drawing.Point(467, 51);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(5);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(132, 21);
			this.btnCancel.TabIndex = 19;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnMerge
			// 
			this.btnMerge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnMerge.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnMerge.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnMerge.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnMerge.Location = new System.Drawing.Point(325, 51);
			this.btnMerge.Margin = new System.Windows.Forms.Padding(5);
			this.btnMerge.Name = "btnMerge";
			this.btnMerge.Size = new System.Drawing.Size(132, 21);
			this.btnMerge.TabIndex = 20;
			this.btnMerge.Text = "Merge -><-";
			this.btnMerge.UseVisualStyleBackColor = false;
			this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
			// 
			// btnFresh
			// 
			this.btnFresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnFresh.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnFresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnFresh.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnFresh.Location = new System.Drawing.Point(183, 51);
			this.btnFresh.Margin = new System.Windows.Forms.Padding(5);
			this.btnFresh.Name = "btnFresh";
			this.btnFresh.Size = new System.Drawing.Size(132, 21);
			this.btnFresh.TabIndex = 21;
			this.btnFresh.Text = "Fresh patch ->->";
			this.btnFresh.UseVisualStyleBackColor = false;
			this.btnFresh.Click += new System.EventHandler(this.btnFresh_Click);
			// 
			// PatchQuestionWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(611, 82);
			this.ControlBox = false;
			this.Controls.Add(this.btnFresh);
			this.Controls.Add(this.btnMerge);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PatchQuestionWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Execute Patch";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnMerge;
		private System.Windows.Forms.Button btnFresh;
	}
}