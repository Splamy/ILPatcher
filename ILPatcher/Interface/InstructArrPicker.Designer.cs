namespace ILPatcher.Interface
{
	partial class InstructArrPicker
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
			this.lbxAllInstruct = new MetroObjects.MListBox();
			this.lbxSwitchInstruct = new MetroObjects.MListBox();
			this.btn_Select = new System.Windows.Forms.Button();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.lblAllInstruct = new System.Windows.Forms.Label();
			this.lblSwitchInstruct = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lbxAllInstruct
			// 
			this.lbxAllInstruct.AllowDrag = false;
			this.lbxAllInstruct.Location = new System.Drawing.Point(5, 30);
			this.lbxAllInstruct.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.lbxAllInstruct.MinimumSize = new System.Drawing.Size(50, 50);
			this.lbxAllInstruct.MultiSelect = true;
			this.lbxAllInstruct.Name = "lbxAllInstruct";
			this.lbxAllInstruct.Size = new System.Drawing.Size(250, 200);
			this.lbxAllInstruct.TabIndex = 0;
			this.lbxAllInstruct.Text = "mListBox1";
			// 
			// lbxSwitchInstruct
			// 
			this.lbxSwitchInstruct.Location = new System.Drawing.Point(260, 30);
			this.lbxSwitchInstruct.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.lbxSwitchInstruct.MinimumSize = new System.Drawing.Size(50, 50);
			this.lbxSwitchInstruct.MultiSelect = true;
			this.lbxSwitchInstruct.Name = "lbxSwitchInstruct";
			this.lbxSwitchInstruct.Size = new System.Drawing.Size(250, 200);
			this.lbxSwitchInstruct.TabIndex = 1;
			this.lbxSwitchInstruct.Text = "mListBox2";
			// 
			// btn_Select
			// 
			this.btn_Select.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btn_Select.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btn_Select.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btn_Select.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_Select.Location = new System.Drawing.Point(98, 235);
			this.btn_Select.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.btn_Select.Name = "btn_Select";
			this.btn_Select.Size = new System.Drawing.Size(87, 21);
			this.btn_Select.TabIndex = 23;
			this.btn_Select.Text = "OK";
			this.btn_Select.UseVisualStyleBackColor = false;
			this.btn_Select.Click += new System.EventHandler(this.btn_Select_Click);
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btn_Cancel.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btn_Cancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_Cancel.Location = new System.Drawing.Point(5, 235);
			this.btn_Cancel.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(87, 21);
			this.btn_Cancel.TabIndex = 22;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = false;
			this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
			// 
			// lblAllInstruct
			// 
			this.lblAllInstruct.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblAllInstruct.Location = new System.Drawing.Point(5, 5);
			this.lblAllInstruct.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.lblAllInstruct.Name = "lblAllInstruct";
			this.lblAllInstruct.Size = new System.Drawing.Size(250, 20);
			this.lblAllInstruct.TabIndex = 24;
			this.lblAllInstruct.Text = "All Instructions";
			this.lblAllInstruct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblSwitchInstruct
			// 
			this.lblSwitchInstruct.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblSwitchInstruct.Location = new System.Drawing.Point(260, 5);
			this.lblSwitchInstruct.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.lblSwitchInstruct.Name = "lblSwitchInstruct";
			this.lblSwitchInstruct.Size = new System.Drawing.Size(250, 20);
			this.lblSwitchInstruct.TabIndex = 25;
			this.lblSwitchInstruct.Text = "Instructions for switch";
			this.lblSwitchInstruct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// InstructArrPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(517, 263);
			this.Controls.Add(this.lblSwitchInstruct);
			this.Controls.Add(this.lblAllInstruct);
			this.Controls.Add(this.btn_Select);
			this.Controls.Add(this.btn_Cancel);
			this.Controls.Add(this.lbxSwitchInstruct);
			this.Controls.Add(this.lbxAllInstruct);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "InstructArrPicker";
			this.Text = "InstructArrPicker";
			this.Resize += new System.EventHandler(this.InstructArrPicker_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private MetroObjects.MListBox lbxAllInstruct;
		private MetroObjects.MListBox lbxSwitchInstruct;
		private System.Windows.Forms.Button btn_Select;
		private System.Windows.Forms.Button btn_Cancel;
		private System.Windows.Forms.Label lblAllInstruct;
		private System.Windows.Forms.Label lblSwitchInstruct;
	}
}