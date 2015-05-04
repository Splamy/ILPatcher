namespace ILPatcher
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateTypeForm));
			this.btnPickMethod = new System.Windows.Forms.Button();
			this.txtTypeName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.txtTypeCompile = new FastColoredTextBoxNS.FastColoredTextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.btnTypeCompiler = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.txtTypeCompile)).BeginInit();
			this.SuspendLayout();
			// 
			// btnPickMethod
			// 
			this.btnPickMethod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnPickMethod.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnPickMethod.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnPickMethod.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPickMethod.Location = new System.Drawing.Point(525, 36);
			this.btnPickMethod.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.btnPickMethod.Name = "btnPickMethod";
			this.btnPickMethod.Size = new System.Drawing.Size(70, 20);
			this.btnPickMethod.TabIndex = 21;
			this.btnPickMethod.Text = "Pick";
			this.btnPickMethod.UseVisualStyleBackColor = false;
			this.btnPickMethod.Click += new System.EventHandler(this.btnPickMethod_Click);
			// 
			// txtTypeName
			// 
			this.txtTypeName.Location = new System.Drawing.Point(95, 36);
			this.txtTypeName.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.txtTypeName.Name = "txtTypeName";
			this.txtTypeName.ReadOnly = true;
			this.txtTypeName.Size = new System.Drawing.Size(425, 20);
			this.txtTypeName.TabIndex = 20;
			// 
			// label4
			// 
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Location = new System.Drawing.Point(5, 35);
			this.label4.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(85, 20);
			this.label4.TabIndex = 22;
			this.label4.Text = "Type:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnOK.Location = new System.Drawing.Point(459, 59);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(136, 25);
			this.btnOK.TabIndex = 31;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// txtTypeCompile
			// 
			this.txtTypeCompile.AutoCompleteBracketsList = new char[] {
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
			this.txtTypeCompile.AutoIndentCharsPatterns = "\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;]+);\n^\\s*(case|default)\\s*[^:]*(" +
    "?<range>:)\\s*(?<range>[^;]+);\n";
			this.txtTypeCompile.AutoScrollMinSize = new System.Drawing.Size(99, 14);
			this.txtTypeCompile.BackBrush = null;
			this.txtTypeCompile.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy2;
			this.txtTypeCompile.CharHeight = 14;
			this.txtTypeCompile.CharWidth = 8;
			this.txtTypeCompile.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtTypeCompile.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.txtTypeCompile.IsReplaceMode = false;
			this.txtTypeCompile.Language = FastColoredTextBoxNS.Language.CSharp;
			this.txtTypeCompile.LeftBracket = '(';
			this.txtTypeCompile.LeftBracket2 = '{';
			this.txtTypeCompile.Location = new System.Drawing.Point(95, 5);
			this.txtTypeCompile.Multiline = false;
			this.txtTypeCompile.Name = "txtTypeCompile";
			this.txtTypeCompile.Paddings = new System.Windows.Forms.Padding(0);
			this.txtTypeCompile.RightBracket = ')';
			this.txtTypeCompile.RightBracket2 = '}';
			this.txtTypeCompile.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.txtTypeCompile.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("txtTypeCompile.ServiceColors")));
			this.txtTypeCompile.ShowScrollBars = false;
			this.txtTypeCompile.Size = new System.Drawing.Size(425, 25);
			this.txtTypeCompile.TabIndex = 32;
			this.txtTypeCompile.Text = "public...";
			this.txtTypeCompile.Zoom = 100;
			// 
			// label6
			// 
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label6.Location = new System.Drawing.Point(5, 5);
			this.label6.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(85, 25);
			this.label6.TabIndex = 33;
			this.label6.Text = "TypeCompiler:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnTypeCompiler
			// 
			this.btnTypeCompiler.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnTypeCompiler.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnTypeCompiler.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnTypeCompiler.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnTypeCompiler.Location = new System.Drawing.Point(525, 5);
			this.btnTypeCompiler.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.btnTypeCompiler.Name = "btnTypeCompiler";
			this.btnTypeCompiler.Size = new System.Drawing.Size(70, 25);
			this.btnTypeCompiler.TabIndex = 34;
			this.btnTypeCompiler.Text = "Compile";
			this.btnTypeCompiler.UseVisualStyleBackColor = false;
			this.btnTypeCompiler.Click += new System.EventHandler(this.btnTypeCompiler_Click);
			// 
			// CreateTypeForm
			// 
			this.ClientSize = new System.Drawing.Size(600, 90);
			this.Controls.Add(this.btnTypeCompiler);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.txtTypeCompile);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnPickMethod);
			this.Controls.Add(this.txtTypeName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "CreateTypeForm";
			this.Text = "TypeSelector";
			((System.ComponentModel.ISupportInitialize)(this.txtTypeCompile)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnPickMethod;
		public System.Windows.Forms.TextBox txtTypeName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnOK;
		private FastColoredTextBoxNS.FastColoredTextBox txtTypeCompile;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnTypeCompiler;
	}
}