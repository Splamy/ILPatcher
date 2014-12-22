namespace ILPatcher
{
	partial class EditorILPattern
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
			this.mInstructBox = new MetroObjects.MListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtPatchActionName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtMethodFullName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btnPickMethod = new System.Windows.Forms.Button();
			this.btnDone = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.cbxOperand = new System.Windows.Forms.ComboBox();
			this.lblOperand = new System.Windows.Forms.Label();
			this.cbxOperandType = new System.Windows.Forms.ComboBox();
			this.lblOperandType = new System.Windows.Forms.Label();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.editIntructionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lblDnD = new System.Windows.Forms.Label();
			this.cbxOpcode = new System.Windows.Forms.ComboBox();
			this.btnDebug = new System.Windows.Forms.Button();
			this.txtOperand = new System.Windows.Forms.TextBox();
			this.chbDelete = new MetroObjects.MCheckBox();
			this.lblDelete = new System.Windows.Forms.Label();
			this.btnNewOpCode = new System.Windows.Forms.Button();
			this.lblwip = new System.Windows.Forms.Label();
			this.instructionEditor = new ILPatcher.InspectorHolder();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// mInstructBox
			// 
			this.mInstructBox.Location = new System.Drawing.Point(95, 60);
			this.mInstructBox.Margin = new System.Windows.Forms.Padding(5);
			this.mInstructBox.MinimumSize = new System.Drawing.Size(50, 50);
			this.mInstructBox.Name = "mInstructBox";
			this.mInstructBox.Size = new System.Drawing.Size(503, 190);
			this.mInstructBox.TabIndex = 8;
			this.mInstructBox.Text = "mListBox1";
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.Location = new System.Drawing.Point(5, 10);
			this.label1.Margin = new System.Windows.Forms.Padding(5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 20);
			this.label1.TabIndex = 10;
			this.label1.Text = "Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtPatchActionName
			// 
			this.txtPatchActionName.Location = new System.Drawing.Point(95, 10);
			this.txtPatchActionName.Margin = new System.Windows.Forms.Padding(5);
			this.txtPatchActionName.Name = "txtPatchActionName";
			this.txtPatchActionName.Size = new System.Drawing.Size(503, 20);
			this.txtPatchActionName.TabIndex = 11;
			this.txtPatchActionName.Text = "DefaultName";
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.Location = new System.Drawing.Point(5, 60);
			this.label2.Margin = new System.Windows.Forms.Padding(5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 20);
			this.label2.TabIndex = 12;
			this.label2.Text = "Patch Table:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtMethodFullName
			// 
			this.txtMethodFullName.Location = new System.Drawing.Point(95, 35);
			this.txtMethodFullName.Margin = new System.Windows.Forms.Padding(5);
			this.txtMethodFullName.Name = "txtMethodFullName";
			this.txtMethodFullName.ReadOnly = true;
			this.txtMethodFullName.Size = new System.Drawing.Size(425, 20);
			this.txtMethodFullName.TabIndex = 15;
			// 
			// label4
			// 
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Location = new System.Drawing.Point(5, 35);
			this.label4.Margin = new System.Windows.Forms.Padding(5);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(85, 20);
			this.label4.TabIndex = 14;
			this.label4.Text = "Methode:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnPickMethod
			// 
			this.btnPickMethod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnPickMethod.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnPickMethod.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnPickMethod.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPickMethod.Location = new System.Drawing.Point(528, 35);
			this.btnPickMethod.Margin = new System.Windows.Forms.Padding(5);
			this.btnPickMethod.Name = "btnPickMethod";
			this.btnPickMethod.Size = new System.Drawing.Size(70, 20);
			this.btnPickMethod.TabIndex = 16;
			this.btnPickMethod.Text = "Pick";
			this.btnPickMethod.UseVisualStyleBackColor = false;
			this.btnPickMethod.Click += new System.EventHandler(this.btnPickMethod_Click);
			// 
			// btnDone
			// 
			this.btnDone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnDone.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnDone.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnDone.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnDone.Location = new System.Drawing.Point(508, 284);
			this.btnDone.Margin = new System.Windows.Forms.Padding(5);
			this.btnDone.Name = "btnDone";
			this.btnDone.Size = new System.Drawing.Size(90, 25);
			this.btnDone.TabIndex = 19;
			this.btnDone.Text = "Done!";
			this.btnDone.UseVisualStyleBackColor = false;
			this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnCancel.Location = new System.Drawing.Point(508, 256);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(5);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(90, 25);
			this.btnCancel.TabIndex = 20;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// cbxOperand
			// 
			this.cbxOperand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOperand.FormattingEnabled = true;
			this.cbxOperand.Items.AddRange(new object[] {
            "[None]",
            "Byte",
            "SByte",
            "Int32",
            "Int64",
            "Single",
            "Double",
            "String",
            "-> Instruction reference",
            "-> Variable reference",
            "-> Parameter reference",
            "-> Field reference",
            "-> Method reference",
            "-> Type reference"});
			this.cbxOperand.Location = new System.Drawing.Point(137, 309);
			this.cbxOperand.Margin = new System.Windows.Forms.Padding(5);
			this.cbxOperand.Name = "cbxOperand";
			this.cbxOperand.Size = new System.Drawing.Size(33, 21);
			this.cbxOperand.TabIndex = 18;
			this.cbxOperand.Visible = false;
			// 
			// lblOperand
			// 
			this.lblOperand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblOperand.Location = new System.Drawing.Point(5, 310);
			this.lblOperand.Margin = new System.Windows.Forms.Padding(5);
			this.lblOperand.Name = "lblOperand";
			this.lblOperand.Size = new System.Drawing.Size(85, 21);
			this.lblOperand.TabIndex = 17;
			this.lblOperand.Text = "Operand:";
			this.lblOperand.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cbxOperandType
			// 
			this.cbxOperandType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOperandType.FormattingEnabled = true;
			this.cbxOperandType.Items.AddRange(new object[] {
            "[None]",
            "Byte",
            "SByte",
            "Int32",
            "Int64",
            "Single",
            "Double",
            "String",
            "-> Instruction reference",
            "-> Variable reference",
            "-> Parameter reference",
            "-> Field reference",
            "-> Method reference",
            "-> Type reference"});
			this.cbxOperandType.Location = new System.Drawing.Point(93, 283);
			this.cbxOperandType.Margin = new System.Windows.Forms.Padding(5);
			this.cbxOperandType.Name = "cbxOperandType";
			this.cbxOperandType.Size = new System.Drawing.Size(388, 21);
			this.cbxOperandType.TabIndex = 16;
			this.cbxOperandType.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// lblOperandType
			// 
			this.lblOperandType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblOperandType.Location = new System.Drawing.Point(5, 284);
			this.lblOperandType.Margin = new System.Windows.Forms.Padding(5);
			this.lblOperandType.Name = "lblOperandType";
			this.lblOperandType.Size = new System.Drawing.Size(85, 21);
			this.lblOperandType.TabIndex = 15;
			this.lblOperandType.Text = "Operand Type:";
			this.lblOperandType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editIntructionToolStripMenuItem,
            this.newToolStripMenuItem,
            this.deleteToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(142, 70);
			// 
			// editIntructionToolStripMenuItem
			// 
			this.editIntructionToolStripMenuItem.Name = "editIntructionToolStripMenuItem";
			this.editIntructionToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.editIntructionToolStripMenuItem.Text = "Edit Selected";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.newToolStripMenuItem.Text = "New";
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			// 
			// lblDnD
			// 
			this.lblDnD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblDnD.Location = new System.Drawing.Point(5, 337);
			this.lblDnD.Margin = new System.Windows.Forms.Padding(5);
			this.lblDnD.Name = "lblDnD";
			this.lblDnD.Size = new System.Drawing.Size(85, 25);
			this.lblDnD.TabIndex = 21;
			this.lblDnD.Text = "Drag n Drop:";
			this.lblDnD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cbxOpcode
			// 
			this.cbxOpcode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
			this.cbxOpcode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.cbxOpcode.FormattingEnabled = true;
			this.cbxOpcode.Location = new System.Drawing.Point(93, 256);
			this.cbxOpcode.Margin = new System.Windows.Forms.Padding(5);
			this.cbxOpcode.Name = "cbxOpcode";
			this.cbxOpcode.Size = new System.Drawing.Size(388, 21);
			this.cbxOpcode.TabIndex = 22;
			this.cbxOpcode.SelectedIndexChanged += new System.EventHandler(this.comboBox3_ValueChanged);
			this.cbxOpcode.TextChanged += new System.EventHandler(this.comboBox3_ValueChanged);
			// 
			// btnDebug
			// 
			this.btnDebug.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnDebug.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnDebug.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnDebug.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnDebug.Location = new System.Drawing.Point(508, 313);
			this.btnDebug.Margin = new System.Windows.Forms.Padding(5);
			this.btnDebug.Name = "btnDebug";
			this.btnDebug.Size = new System.Drawing.Size(90, 25);
			this.btnDebug.TabIndex = 23;
			this.btnDebug.Text = "Debug_Load";
			this.btnDebug.UseVisualStyleBackColor = false;
			this.btnDebug.Click += new System.EventHandler(this.btnDebug_Click);
			// 
			// txtOperand
			// 
			this.txtOperand.Location = new System.Drawing.Point(93, 310);
			this.txtOperand.Margin = new System.Windows.Forms.Padding(5);
			this.txtOperand.Name = "txtOperand";
			this.txtOperand.Size = new System.Drawing.Size(34, 20);
			this.txtOperand.TabIndex = 24;
			this.txtOperand.Visible = false;
			// 
			// chbDelete
			// 
			this.chbDelete.Location = new System.Drawing.Point(457, 337);
			this.chbDelete.Margin = new System.Windows.Forms.Padding(5);
			this.chbDelete.MinimumSize = new System.Drawing.Size(10, 10);
			this.chbDelete.Name = "chbDelete";
			this.chbDelete.Size = new System.Drawing.Size(25, 25);
			this.chbDelete.TabIndex = 25;
			this.chbDelete.Text = "mCheckBox1";
			// 
			// lblDelete
			// 
			this.lblDelete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblDelete.Location = new System.Drawing.Point(405, 337);
			this.lblDelete.Margin = new System.Windows.Forms.Padding(5);
			this.lblDelete.Name = "lblDelete";
			this.lblDelete.Size = new System.Drawing.Size(45, 25);
			this.lblDelete.TabIndex = 26;
			this.lblDelete.Text = "Delete";
			this.lblDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnNewOpCode
			// 
			this.btnNewOpCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnNewOpCode.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnNewOpCode.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnNewOpCode.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnNewOpCode.Location = new System.Drawing.Point(5, 256);
			this.btnNewOpCode.Margin = new System.Windows.Forms.Padding(5);
			this.btnNewOpCode.Name = "btnNewOpCode";
			this.btnNewOpCode.Size = new System.Drawing.Size(85, 21);
			this.btnNewOpCode.TabIndex = 27;
			this.btnNewOpCode.Text = "New OpCode:";
			this.btnNewOpCode.UseVisualStyleBackColor = false;
			this.btnNewOpCode.Click += new System.EventHandler(this.btnNewOpCode_Click);
			// 
			// lblwip
			// 
			this.lblwip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblwip.Location = new System.Drawing.Point(178, 309);
			this.lblwip.Name = "lblwip";
			this.lblwip.Size = new System.Drawing.Size(50, 21);
			this.lblwip.TabIndex = 28;
			this.lblwip.Text = "<<Work in Progress>>";
			this.lblwip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// instructionEditor
			// 
			this.instructionEditor.AllowDrag = false;
			this.instructionEditor.DragItem = null;
			this.instructionEditor.Location = new System.Drawing.Point(93, 337);
			this.instructionEditor.Margin = new System.Windows.Forms.Padding(5);
			this.instructionEditor.Name = "instructionEditor";
			this.instructionEditor.Size = new System.Drawing.Size(306, 25);
			this.instructionEditor.TabIndex = 0;
			this.instructionEditor.Text = "inspectorHolder1";
			// 
			// EditorILPattern
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblwip);
			this.Controls.Add(this.btnNewOpCode);
			this.Controls.Add(this.lblDelete);
			this.Controls.Add(this.chbDelete);
			this.Controls.Add(this.txtOperand);
			this.Controls.Add(this.btnDebug);
			this.Controls.Add(this.cbxOpcode);
			this.Controls.Add(this.lblDnD);
			this.Controls.Add(this.cbxOperand);
			this.Controls.Add(this.lblOperand);
			this.Controls.Add(this.cbxOperandType);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblOperandType);
			this.Controls.Add(this.btnDone);
			this.Controls.Add(this.instructionEditor);
			this.Controls.Add(this.btnPickMethod);
			this.Controls.Add(this.txtMethodFullName);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtPatchActionName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.mInstructBox);
			this.Name = "EditorILPattern";
			this.Size = new System.Drawing.Size(605, 368);
			this.Resize += new System.EventHandler(this.EditorILPattern_Resize);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MetroObjects.MListBox mInstructBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtPatchActionName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnPickMethod;
		private System.Windows.Forms.Button btnDone;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblOperandType;
		private InspectorHolder instructionEditor;
		private System.Windows.Forms.ComboBox cbxOperand;
		private System.Windows.Forms.Label lblOperand;
		private System.Windows.Forms.ComboBox cbxOperandType;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem editIntructionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.Label lblDnD;
		private System.Windows.Forms.ComboBox cbxOpcode;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.Button btnDebug;
		private System.Windows.Forms.TextBox txtOperand;
		private MetroObjects.MCheckBox chbDelete;
		private System.Windows.Forms.Label lblDelete;
		private System.Windows.Forms.Button btnNewOpCode;
		private System.Windows.Forms.Label lblwip;
		public System.Windows.Forms.TextBox txtMethodFullName;
	}
}