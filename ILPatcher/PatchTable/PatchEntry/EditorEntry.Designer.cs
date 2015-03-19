namespace ILPatcher
{
	partial class EditorEntry
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
			this.txtPatchEntryName = new System.Windows.Forms.TextBox();
			this.lblName = new System.Windows.Forms.Label();
			this.mEntryList = new MetroObjects.MListBox();
			this.btnILMethodFixed = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnEditPatchAction = new System.Windows.Forms.Button();
			this.btnMethodCreator = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtPatchEntryName
			// 
			this.txtPatchEntryName.Location = new System.Drawing.Point(111, 10);
			this.txtPatchEntryName.Name = "txtPatchEntryName";
			this.txtPatchEntryName.Size = new System.Drawing.Size(317, 20);
			this.txtPatchEntryName.TabIndex = 13;
			this.txtPatchEntryName.Text = "DefaultEntryN";
			// 
			// lblName
			// 
			this.lblName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblName.Location = new System.Drawing.Point(5, 10);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(100, 20);
			this.lblName.TabIndex = 12;
			this.lblName.Text = "Name:";
			this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// mEntryList
			// 
			this.mEntryList.Location = new System.Drawing.Point(111, 35);
			this.mEntryList.MinimumSize = new System.Drawing.Size(50, 50);
			this.mEntryList.Name = "mEntryList";
			this.mEntryList.Size = new System.Drawing.Size(317, 170);
			this.mEntryList.TabIndex = 14;
			this.mEntryList.Text = "mListBox1";
			// 
			// btnILMethodFixed
			// 
			this.btnILMethodFixed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnILMethodFixed.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnILMethodFixed.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnILMethodFixed.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnILMethodFixed.Location = new System.Drawing.Point(5, 35);
			this.btnILMethodFixed.Name = "btnILMethodFixed";
			this.btnILMethodFixed.Size = new System.Drawing.Size(100, 25);
			this.btnILMethodFixed.TabIndex = 18;
			this.btnILMethodFixed.Text = "ILMethodFixed";
			this.btnILMethodFixed.UseVisualStyleBackColor = false;
			this.btnILMethodFixed.Click += new System.EventHandler(this.btnILMethodFixed_Click);
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnOK.Location = new System.Drawing.Point(5, 180);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 25);
			this.btnOK.TabIndex = 20;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnEditPatchAction
			// 
			this.btnEditPatchAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnEditPatchAction.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnEditPatchAction.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnEditPatchAction.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnEditPatchAction.Location = new System.Drawing.Point(5, 97);
			this.btnEditPatchAction.Name = "btnEditPatchAction";
			this.btnEditPatchAction.Size = new System.Drawing.Size(100, 25);
			this.btnEditPatchAction.TabIndex = 21;
			this.btnEditPatchAction.Text = "Edit Patch";
			this.btnEditPatchAction.UseVisualStyleBackColor = false;
			this.btnEditPatchAction.Click += new System.EventHandler(this.btnEditPatchAction_Click);
			// 
			// btnMethodCreator
			// 
			this.btnMethodCreator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnMethodCreator.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnMethodCreator.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnMethodCreator.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnMethodCreator.Location = new System.Drawing.Point(5, 66);
			this.btnMethodCreator.Name = "btnMethodCreator";
			this.btnMethodCreator.Size = new System.Drawing.Size(100, 25);
			this.btnMethodCreator.TabIndex = 22;
			this.btnMethodCreator.Text = "MethodCreator";
			this.btnMethodCreator.UseVisualStyleBackColor = false;
			this.btnMethodCreator.Click += new System.EventHandler(this.btnMethodCreator_Click);
			// 
			// EditorEntry
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnMethodCreator);
			this.Controls.Add(this.btnEditPatchAction);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnILMethodFixed);
			this.Controls.Add(this.mEntryList);
			this.Controls.Add(this.txtPatchEntryName);
			this.Controls.Add(this.lblName);
			this.Name = "EditorEntry";
			this.Size = new System.Drawing.Size(431, 214);
			this.Resize += new System.EventHandler(this.EditorEntry_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtPatchEntryName;
		private System.Windows.Forms.Label lblName;
		private MetroObjects.MListBox mEntryList;
		private System.Windows.Forms.Button btnILMethodFixed;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnEditPatchAction;
		private System.Windows.Forms.Button btnMethodCreator;
	}
}