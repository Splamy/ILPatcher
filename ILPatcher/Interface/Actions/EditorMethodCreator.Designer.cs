namespace ILPatcher.Interface.Actions
{
	partial class EditorMethodCreator
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

		#region Component Designer generated code

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
			this.btnOK = new System.Windows.Forms.Button();
			this.txtMethodName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cbxModifier = new System.Windows.Forms.ComboBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.hoizontalTabControl1 = new ILPatcher.Interface.General.HoizontalTabControl();
			this.tabParameter = new System.Windows.Forms.TabPage();
			this.lbxParameter = new MetroObjects.MListBox();
			this.arcParameter = new ILPatcher.Interface.General.AddRemoveControl();
			this.tabVariables = new System.Windows.Forms.TabPage();
			this.tabAttributes = new System.Windows.Forms.TabPage();
			this.hoizontalTabControl1.SuspendLayout();
			this.tabParameter.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnPickMethod
			// 
			this.btnPickMethod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnPickMethod.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnPickMethod.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnPickMethod.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPickMethod.Location = new System.Drawing.Point(525, 30);
			this.btnPickMethod.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.btnPickMethod.Name = "btnPickMethod";
			this.btnPickMethod.Size = new System.Drawing.Size(70, 20);
			this.btnPickMethod.TabIndex = 19;
			this.btnPickMethod.Text = "Pick";
			this.btnPickMethod.UseVisualStyleBackColor = false;
			this.btnPickMethod.Click += new System.EventHandler(this.btnPickMethod_Click);
			// 
			// txtMethodFullName
			// 
			this.txtMethodFullName.Location = new System.Drawing.Point(95, 30);
			this.txtMethodFullName.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.txtMethodFullName.Name = "txtMethodFullName";
			this.txtMethodFullName.ReadOnly = true;
			this.txtMethodFullName.Size = new System.Drawing.Size(425, 20);
			this.txtMethodFullName.TabIndex = 18;
			// 
			// label4
			// 
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Location = new System.Drawing.Point(5, 30);
			this.label4.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(85, 20);
			this.label4.TabIndex = 17;
			this.label4.Text = "Class:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtPatchActionName
			// 
			this.txtPatchActionName.Location = new System.Drawing.Point(95, 5);
			this.txtPatchActionName.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.txtPatchActionName.Name = "txtPatchActionName";
			this.txtPatchActionName.Size = new System.Drawing.Size(500, 20);
			this.txtPatchActionName.TabIndex = 21;
			this.txtPatchActionName.Text = "DefaultName";
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.Location = new System.Drawing.Point(5, 5);
			this.label1.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 20);
			this.label1.TabIndex = 20;
			this.label1.Text = "Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnOK.Location = new System.Drawing.Point(510, 339);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(85, 25);
			this.btnOK.TabIndex = 22;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// txtMethodName
			// 
			this.txtMethodName.Location = new System.Drawing.Point(95, 55);
			this.txtMethodName.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.txtMethodName.Name = "txtMethodName";
			this.txtMethodName.Size = new System.Drawing.Size(500, 20);
			this.txtMethodName.TabIndex = 30;
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.Location = new System.Drawing.Point(5, 55);
			this.label2.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 20);
			this.label2.TabIndex = 29;
			this.label2.Text = "Methodname:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.Location = new System.Drawing.Point(5, 80);
			this.label3.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 20);
			this.label3.TabIndex = 31;
			this.label3.Text = "Modifier:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cbxModifier
			// 
			this.cbxModifier.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
			this.cbxModifier.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.cbxModifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxModifier.FormattingEnabled = true;
			this.cbxModifier.Items.AddRange(new object[] {
            "public",
            "protected",
            "protected internal",
            "internal",
            "private"});
			this.cbxModifier.Location = new System.Drawing.Point(95, 81);
			this.cbxModifier.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.cbxModifier.Name = "cbxModifier";
			this.cbxModifier.Size = new System.Drawing.Size(500, 21);
			this.cbxModifier.TabIndex = 32;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(95, 107);
			this.textBox1.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(425, 20);
			this.textBox1.TabIndex = 34;
			// 
			// label5
			// 
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.Location = new System.Drawing.Point(5, 107);
			this.label5.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(85, 20);
			this.label5.TabIndex = 33;
			this.label5.Text = "FillAction:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button1.Location = new System.Drawing.Point(525, 107);
			this.button1.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(70, 20);
			this.button1.TabIndex = 35;
			this.button1.Text = "Pick";
			this.button1.UseVisualStyleBackColor = false;
			// 
			// hoizontalTabControl1
			// 
			this.hoizontalTabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
			this.hoizontalTabControl1.Controls.Add(this.tabParameter);
			this.hoizontalTabControl1.Controls.Add(this.tabVariables);
			this.hoizontalTabControl1.Controls.Add(this.tabAttributes);
			this.hoizontalTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.hoizontalTabControl1.ItemSize = new System.Drawing.Size(20, 85);
			this.hoizontalTabControl1.Location = new System.Drawing.Point(5, 130);
			this.hoizontalTabControl1.Multiline = true;
			this.hoizontalTabControl1.Name = "hoizontalTabControl1";
			this.hoizontalTabControl1.SelectedIndex = 0;
			this.hoizontalTabControl1.Size = new System.Drawing.Size(590, 203);
			this.hoizontalTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.hoizontalTabControl1.TabIndex = 37;
			// 
			// tabParameter
			// 
			this.tabParameter.Controls.Add(this.lbxParameter);
			this.tabParameter.Controls.Add(this.arcParameter);
			this.tabParameter.Location = new System.Drawing.Point(89, 4);
			this.tabParameter.Name = "tabParameter";
			this.tabParameter.Padding = new System.Windows.Forms.Padding(3);
			this.tabParameter.Size = new System.Drawing.Size(497, 195);
			this.tabParameter.TabIndex = 1;
			this.tabParameter.Text = "Parameter";
			this.tabParameter.UseVisualStyleBackColor = true;
			// 
			// lbxParameter
			// 
			this.lbxParameter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbxParameter.Location = new System.Drawing.Point(3, 3);
			this.lbxParameter.MinimumSize = new System.Drawing.Size(50, 50);
			this.lbxParameter.Name = "lbxParameter";
			this.lbxParameter.Size = new System.Drawing.Size(491, 156);
			this.lbxParameter.TabIndex = 28;
			this.lbxParameter.Text = "mListBox1";
			// 
			// arcParameter
			// 
			this.arcParameter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.arcParameter.Location = new System.Drawing.Point(3, 159);
			this.arcParameter.Name = "arcParameter";
			this.arcParameter.Size = new System.Drawing.Size(491, 33);
			this.arcParameter.TabIndex = 29;
			this.arcParameter.Text = "addRemoveControl1";
			// 
			// tabVariables
			// 
			this.tabVariables.Location = new System.Drawing.Point(89, 4);
			this.tabVariables.Name = "tabVariables";
			this.tabVariables.Size = new System.Drawing.Size(497, 195);
			this.tabVariables.TabIndex = 2;
			this.tabVariables.Text = "Variables";
			this.tabVariables.UseVisualStyleBackColor = true;
			// 
			// tabAttributes
			// 
			this.tabAttributes.Location = new System.Drawing.Point(89, 4);
			this.tabAttributes.Name = "tabAttributes";
			this.tabAttributes.Size = new System.Drawing.Size(497, 195);
			this.tabAttributes.TabIndex = 3;
			this.tabAttributes.Text = "Attributes";
			this.tabAttributes.UseVisualStyleBackColor = true;
			// 
			// EditorMethodCreator
			// 
			this.Controls.Add(this.hoizontalTabControl1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cbxModifier);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtMethodName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtPatchActionName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnPickMethod);
			this.Controls.Add(this.txtMethodFullName);
			this.Controls.Add(this.label4);
			this.Name = "EditorMethodCreator";
			this.Size = new System.Drawing.Size(606, 377);
			this.hoizontalTabControl1.ResumeLayout(false);
			this.tabParameter.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnPickMethod;
		public System.Windows.Forms.TextBox txtMethodFullName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtPatchActionName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private MetroObjects.MListBox lbxParameter;
		private System.Windows.Forms.TextBox txtMethodName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cbxModifier;
		public System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button button1;
		private ILPatcher.Interface.General.HoizontalTabControl hoizontalTabControl1;
		private System.Windows.Forms.TabPage tabParameter;
		private System.Windows.Forms.TabPage tabVariables;
		private System.Windows.Forms.TabPage tabAttributes;
		private ILPatcher.Interface.General.AddRemoveControl arcParameter;
	}
}
