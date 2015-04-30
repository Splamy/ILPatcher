namespace ILPatcher.GeneralInterface
{
	partial class CreateTypeForm
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
			this.btnPickMethod = new System.Windows.Forms.Button();
			this.txtMethodFullName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtPatchActionName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.chbDelete = new MetroObjects.MCheckBox();
			this.mCheckBox1 = new MetroObjects.MCheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.mCheckBox2 = new MetroObjects.MCheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnPickMethod
			// 
			this.btnPickMethod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnPickMethod.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnPickMethod.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnPickMethod.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPickMethod.Location = new System.Drawing.Point(525, 6);
			this.btnPickMethod.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.btnPickMethod.Name = "btnPickMethod";
			this.btnPickMethod.Size = new System.Drawing.Size(70, 20);
			this.btnPickMethod.TabIndex = 21;
			this.btnPickMethod.Text = "Pick";
			this.btnPickMethod.UseVisualStyleBackColor = false;
			this.btnPickMethod.Click += new System.EventHandler(this.btnPickMethod_Click);
			// 
			// txtMethodFullName
			// 
			this.txtMethodFullName.Location = new System.Drawing.Point(95, 6);
			this.txtMethodFullName.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.txtMethodFullName.Name = "txtMethodFullName";
			this.txtMethodFullName.ReadOnly = true;
			this.txtMethodFullName.Size = new System.Drawing.Size(425, 20);
			this.txtMethodFullName.TabIndex = 20;
			// 
			// label4
			// 
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Location = new System.Drawing.Point(5, 5);
			this.label4.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(85, 20);
			this.label4.TabIndex = 22;
			this.label4.Text = "Type:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtPatchActionName
			// 
			this.txtPatchActionName.Location = new System.Drawing.Point(95, 30);
			this.txtPatchActionName.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.txtPatchActionName.Name = "txtPatchActionName";
			this.txtPatchActionName.Size = new System.Drawing.Size(500, 20);
			this.txtPatchActionName.TabIndex = 24;
			this.txtPatchActionName.Text = "DefaultName";
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.Location = new System.Drawing.Point(5, 30);
			this.label1.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 20);
			this.label1.TabIndex = 23;
			this.label1.Text = "Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.Location = new System.Drawing.Point(5, 55);
			this.label2.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 25);
			this.label2.TabIndex = 25;
			this.label2.Text = "IsArray:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// chbDelete
			// 
			this.chbDelete.Location = new System.Drawing.Point(95, 55);
			this.chbDelete.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.chbDelete.MinimumSize = new System.Drawing.Size(10, 10);
			this.chbDelete.Name = "chbDelete";
			this.chbDelete.Size = new System.Drawing.Size(25, 25);
			this.chbDelete.TabIndex = 26;
			this.chbDelete.Text = "mCheckBox1";
			// 
			// mCheckBox1
			// 
			this.mCheckBox1.Location = new System.Drawing.Point(236, 55);
			this.mCheckBox1.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.mCheckBox1.MinimumSize = new System.Drawing.Size(10, 10);
			this.mCheckBox1.Name = "mCheckBox1";
			this.mCheckBox1.Size = new System.Drawing.Size(25, 25);
			this.mCheckBox1.TabIndex = 28;
			this.mCheckBox1.Text = "mCheckBox1";
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.Location = new System.Drawing.Point(146, 55);
			this.label3.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 25);
			this.label3.TabIndex = 27;
			this.label3.Text = "IsPointer:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// mCheckBox2
			// 
			this.mCheckBox2.Location = new System.Drawing.Point(389, 55);
			this.mCheckBox2.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.mCheckBox2.MinimumSize = new System.Drawing.Size(10, 10);
			this.mCheckBox2.Name = "mCheckBox2";
			this.mCheckBox2.Size = new System.Drawing.Size(25, 25);
			this.mCheckBox2.TabIndex = 30;
			this.mCheckBox2.Text = "mCheckBox2";
			// 
			// label5
			// 
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.Location = new System.Drawing.Point(299, 55);
			this.label5.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(85, 25);
			this.label5.TabIndex = 29;
			this.label5.Text = "IsReference:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// CreateTypeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mCheckBox2);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.mCheckBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.chbDelete);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtPatchActionName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnPickMethod);
			this.Controls.Add(this.txtMethodFullName);
			this.Name = "CreateTypeForm";
			this.Size = new System.Drawing.Size(603, 393);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnPickMethod;
		public System.Windows.Forms.TextBox txtMethodFullName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtPatchActionName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private MetroObjects.MCheckBox chbDelete;
		private MetroObjects.MCheckBox mCheckBox1;
		private System.Windows.Forms.Label label3;
		private MetroObjects.MCheckBox mCheckBox2;
		private System.Windows.Forms.Label label5;
	}
}