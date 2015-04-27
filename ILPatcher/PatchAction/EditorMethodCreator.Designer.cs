namespace ILPatcher
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
			this.components = new System.ComponentModel.Container();
			this.btnPickMethod = new System.Windows.Forms.Button();
			this.txtMethodFullName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtPatchActionName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.txtInjectCode = new FastColoredTextBoxNS.FastColoredTextBox();
			this.mListBox1 = new MetroObjects.MListBox();
			this.mLoadingCircle1 = new MetroObjects.MLoadingCircle();
			((System.ComponentModel.ISupportInitialize)(this.txtInjectCode)).BeginInit();
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
			this.btnOK.Location = new System.Drawing.Point(5, 53);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(85, 25);
			this.btnOK.TabIndex = 22;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button1.Location = new System.Drawing.Point(6, 120);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(84, 25);
			this.button1.TabIndex = 25;
			this.button1.Text = "Compile";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.button2.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button2.Location = new System.Drawing.Point(513, 289);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(100, 25);
			this.button2.TabIndex = 26;
			this.button2.Text = "Deref";
			this.button2.UseVisualStyleBackColor = false;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// txtInjectCode
			// 
			this.txtInjectCode.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
			this.txtInjectCode.AutoScrollMinSize = new System.Drawing.Size(179, 14);
			this.txtInjectCode.BackBrush = null;
			this.txtInjectCode.CharHeight = 14;
			this.txtInjectCode.CharWidth = 8;
			this.txtInjectCode.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtInjectCode.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.txtInjectCode.IsReplaceMode = false;
			this.txtInjectCode.Location = new System.Drawing.Point(95, 53);
			this.txtInjectCode.Name = "txtInjectCode";
			this.txtInjectCode.Paddings = new System.Windows.Forms.Padding(0);
			this.txtInjectCode.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.txtInjectCode.ServiceColors = null;
			this.txtInjectCode.Size = new System.Drawing.Size(412, 360);
			this.txtInjectCode.TabIndex = 27;
			this.txtInjectCode.Text = "fastColoredTextBox1";
			this.txtInjectCode.Zoom = 100;
			// 
			// mListBox1
			// 
			this.mListBox1.Location = new System.Drawing.Point(513, 53);
			this.mListBox1.MinimumSize = new System.Drawing.Size(50, 50);
			this.mListBox1.Name = "mListBox1";
			this.mListBox1.Size = new System.Drawing.Size(227, 230);
			this.mListBox1.TabIndex = 28;
			this.mListBox1.Text = "mListBox1";
			// 
			// mLoadingCircle1
			// 
			this.mLoadingCircle1.Location = new System.Drawing.Point(59, 84);
			this.mLoadingCircle1.MinimumSize = new System.Drawing.Size(30, 30);
			this.mLoadingCircle1.Name = "mLoadingCircle1";
			this.mLoadingCircle1.Size = new System.Drawing.Size(30, 30);
			this.mLoadingCircle1.TabIndex = 29;
			this.mLoadingCircle1.Text = "mLoadingCircle1";
			// 
			// EditorMethodCreator
			// 
			this.Controls.Add(this.mLoadingCircle1);
			this.Controls.Add(this.mListBox1);
			this.Controls.Add(this.txtInjectCode);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtPatchActionName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnPickMethod);
			this.Controls.Add(this.txtMethodFullName);
			this.Controls.Add(this.label4);
			this.Name = "EditorMethodCreator";
			this.Size = new System.Drawing.Size(743, 416);
			((System.ComponentModel.ISupportInitialize)(this.txtInjectCode)).EndInit();
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
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private MetroObjects.MListBox mListBox1;
		private MetroObjects.MLoadingCircle mLoadingCircle1;
		public FastColoredTextBoxNS.FastColoredTextBox txtInjectCode;
	}
}
