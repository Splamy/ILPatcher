namespace ILPatcher.Interface.Actions
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
			this.cbxOperand = new System.Windows.Forms.ComboBox();
			this.lblOperand = new System.Windows.Forms.Label();
			this.lblOperandType = new System.Windows.Forms.Label();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmUnDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.lblDnD = new System.Windows.Forms.Label();
			this.cbxOpcode = new System.Windows.Forms.ComboBox();
			this.txtOperand = new System.Windows.Forms.TextBox();
			this.chbDelete = new MetroObjects.MCheckBox();
			this.lblDelete = new System.Windows.Forms.Label();
			this.btnNewOpCode = new System.Windows.Forms.Button();
			this.panTMFPicker = new System.Windows.Forms.Panel();
			this.lblTMFPicker = new System.Windows.Forms.Label();
			this.btnTMFPicker = new System.Windows.Forms.Button();
			this.instructionEditor = new ILPatcher.Interface.General.InspectorHolder();
			this.contextMenuStrip1.SuspendLayout();
			this.panTMFPicker.SuspendLayout();
			this.SuspendLayout();
			// 
			// mInstructBox
			// 
			this.mInstructBox.Location = new System.Drawing.Point(95, 55);
			this.mInstructBox.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.mInstructBox.MinimumSize = new System.Drawing.Size(50, 50);
			this.mInstructBox.MultiSelect = true;
			this.mInstructBox.Name = "mInstructBox";
			this.mInstructBox.Size = new System.Drawing.Size(500, 190);
			this.mInstructBox.TabIndex = 8;
			this.mInstructBox.Text = "mListBox1";
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.Location = new System.Drawing.Point(5, 5);
			this.label1.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 20);
			this.label1.TabIndex = 10;
			this.label1.Text = "Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtPatchActionName
			// 
			this.txtPatchActionName.Location = new System.Drawing.Point(95, 5);
			this.txtPatchActionName.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.txtPatchActionName.Name = "txtPatchActionName";
			this.txtPatchActionName.Size = new System.Drawing.Size(500, 20);
			this.txtPatchActionName.TabIndex = 11;
			this.txtPatchActionName.Text = "DefaultName";
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.Location = new System.Drawing.Point(5, 55);
			this.label2.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 20);
			this.label2.TabIndex = 12;
			this.label2.Text = "Patch Table:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtMethodFullName
			// 
			this.txtMethodFullName.Location = new System.Drawing.Point(95, 30);
			this.txtMethodFullName.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.txtMethodFullName.Name = "txtMethodFullName";
			this.txtMethodFullName.ReadOnly = true;
			this.txtMethodFullName.Size = new System.Drawing.Size(425, 20);
			this.txtMethodFullName.TabIndex = 15;
			// 
			// label4
			// 
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Location = new System.Drawing.Point(5, 30);
			this.label4.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
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
			this.btnPickMethod.Location = new System.Drawing.Point(525, 30);
			this.btnPickMethod.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
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
			this.btnDone.Location = new System.Drawing.Point(505, 250);
			this.btnDone.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.btnDone.Name = "btnDone";
			this.btnDone.Size = new System.Drawing.Size(90, 25);
			this.btnDone.TabIndex = 19;
			this.btnDone.Text = "Done!";
			this.btnDone.UseVisualStyleBackColor = false;
			this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
			// 
			// cbxOperand
			// 
			this.cbxOperand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOperand.FormattingEnabled = true;
			this.cbxOperand.Location = new System.Drawing.Point(159, 277);
			this.cbxOperand.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.cbxOperand.Name = "cbxOperand";
			this.cbxOperand.Size = new System.Drawing.Size(80, 21);
			this.cbxOperand.TabIndex = 18;
			this.cbxOperand.Visible = false;
			this.cbxOperand.SelectedIndexChanged += new System.EventHandler(this.cbxOperand_SelectedIndexChanged);
			// 
			// lblOperand
			// 
			this.lblOperand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblOperand.Location = new System.Drawing.Point(5, 276);
			this.lblOperand.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.lblOperand.Name = "lblOperand";
			this.lblOperand.Size = new System.Drawing.Size(85, 21);
			this.lblOperand.TabIndex = 17;
			this.lblOperand.Text = "Operand:";
			this.lblOperand.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblOperandType
			// 
			this.lblOperandType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblOperandType.Location = new System.Drawing.Point(288, 250);
			this.lblOperandType.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.lblOperandType.Name = "lblOperandType";
			this.lblOperandType.Size = new System.Drawing.Size(212, 21);
			this.lblOperandType.TabIndex = 15;
			this.lblOperandType.Text = "< Operand Type >";
			this.lblOperandType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmDelete,
            this.tsmUnDelete,
            this.tsmRemove});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(168, 70);
			// 
			// tsmDelete
			// 
			this.tsmDelete.Name = "tsmDelete";
			this.tsmDelete.Size = new System.Drawing.Size(167, 22);
			this.tsmDelete.Text = "Set Delete-Flag";
			this.tsmDelete.Click += new System.EventHandler(this.tsmDelete_Click);
			// 
			// tsmUnDelete
			// 
			this.tsmUnDelete.Name = "tsmUnDelete";
			this.tsmUnDelete.Size = new System.Drawing.Size(167, 22);
			this.tsmUnDelete.Text = "Unset Delete-Flag";
			this.tsmUnDelete.Click += new System.EventHandler(this.tsmUnDelete_Click);
			// 
			// tsmRemove
			// 
			this.tsmRemove.Name = "tsmRemove";
			this.tsmRemove.Size = new System.Drawing.Size(167, 22);
			this.tsmRemove.Text = "Remove Selected";
			this.tsmRemove.Click += new System.EventHandler(this.tsmRemove_Click);
			// 
			// lblDnD
			// 
			this.lblDnD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblDnD.Location = new System.Drawing.Point(5, 302);
			this.lblDnD.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
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
			this.cbxOpcode.Location = new System.Drawing.Point(95, 250);
			this.cbxOpcode.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.cbxOpcode.Name = "cbxOpcode";
			this.cbxOpcode.Size = new System.Drawing.Size(188, 21);
			this.cbxOpcode.TabIndex = 22;
			this.cbxOpcode.SelectedIndexChanged += new System.EventHandler(this.cbxOpcode_ValueChanged);
			this.cbxOpcode.TextChanged += new System.EventHandler(this.cbxOpcode_ValueChanged);
			// 
			// txtOperand
			// 
			this.txtOperand.Location = new System.Drawing.Point(95, 276);
			this.txtOperand.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.txtOperand.Name = "txtOperand";
			this.txtOperand.Size = new System.Drawing.Size(59, 20);
			this.txtOperand.TabIndex = 24;
			this.txtOperand.Visible = false;
			this.txtOperand.TextChanged += new System.EventHandler(this.txtOperand_TextChanged);
			// 
			// chbDelete
			// 
			this.chbDelete.Location = new System.Drawing.Point(475, 302);
			this.chbDelete.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.chbDelete.MinimumSize = new System.Drawing.Size(10, 10);
			this.chbDelete.Name = "chbDelete";
			this.chbDelete.Size = new System.Drawing.Size(25, 25);
			this.chbDelete.TabIndex = 25;
			this.chbDelete.Text = "mCheckBox1";
			// 
			// lblDelete
			// 
			this.lblDelete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblDelete.Location = new System.Drawing.Point(423, 302);
			this.lblDelete.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
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
			this.btnNewOpCode.Location = new System.Drawing.Point(5, 250);
			this.btnNewOpCode.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.btnNewOpCode.Name = "btnNewOpCode";
			this.btnNewOpCode.Size = new System.Drawing.Size(85, 21);
			this.btnNewOpCode.TabIndex = 27;
			this.btnNewOpCode.Text = "New OpCode:";
			this.btnNewOpCode.UseVisualStyleBackColor = false;
			this.btnNewOpCode.Click += new System.EventHandler(this.btnNewOpCode_Click);
			// 
			// panTMFPicker
			// 
			this.panTMFPicker.Controls.Add(this.lblTMFPicker);
			this.panTMFPicker.Controls.Add(this.btnTMFPicker);
			this.panTMFPicker.Location = new System.Drawing.Point(242, 277);
			this.panTMFPicker.Name = "panTMFPicker";
			this.panTMFPicker.Size = new System.Drawing.Size(125, 21);
			this.panTMFPicker.TabIndex = 29;
			this.panTMFPicker.Visible = false;
			// 
			// lblTMFPicker
			// 
			this.lblTMFPicker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblTMFPicker.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblTMFPicker.Location = new System.Drawing.Point(0, 0);
			this.lblTMFPicker.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.lblTMFPicker.Name = "lblTMFPicker";
			this.lblTMFPicker.Size = new System.Drawing.Size(50, 21);
			this.lblTMFPicker.TabIndex = 30;
			this.lblTMFPicker.Text = "TMF";
			this.lblTMFPicker.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnTMFPicker
			// 
			this.btnTMFPicker.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnTMFPicker.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnTMFPicker.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnTMFPicker.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnTMFPicker.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnTMFPicker.Location = new System.Drawing.Point(50, 0);
			this.btnTMFPicker.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.btnTMFPicker.Name = "btnTMFPicker";
			this.btnTMFPicker.Size = new System.Drawing.Size(75, 21);
			this.btnTMFPicker.TabIndex = 30;
			this.btnTMFPicker.Text = "Pick";
			this.btnTMFPicker.UseVisualStyleBackColor = false;
			this.btnTMFPicker.Click += new System.EventHandler(this.btnTMFPicker_Click);
			// 
			// instructionEditor
			// 
			this.instructionEditor.AllowDrag = false;
			this.instructionEditor.DragItem = null;
			this.instructionEditor.Location = new System.Drawing.Point(95, 302);
			this.instructionEditor.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.instructionEditor.Name = "instructionEditor";
			this.instructionEditor.Size = new System.Drawing.Size(323, 25);
			this.instructionEditor.TabIndex = 0;
			this.instructionEditor.Text = "inspectorHolder1";
			// 
			// EditorILPattern
			// 
			this.Controls.Add(this.panTMFPicker);
			this.Controls.Add(this.btnNewOpCode);
			this.Controls.Add(this.lblDelete);
			this.Controls.Add(this.chbDelete);
			this.Controls.Add(this.txtOperand);
			this.Controls.Add(this.cbxOpcode);
			this.Controls.Add(this.lblDnD);
			this.Controls.Add(this.cbxOperand);
			this.Controls.Add(this.lblOperand);
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
			this.Size = new System.Drawing.Size(600, 334);
			this.Resize += new System.EventHandler(this.EditorILPattern_Resize);
			this.contextMenuStrip1.ResumeLayout(false);
			this.panTMFPicker.ResumeLayout(false);
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
		private System.Windows.Forms.Label lblOperandType;
		private ILPatcher.Interface.General.InspectorHolder instructionEditor;
		private System.Windows.Forms.ComboBox cbxOperand;
		private System.Windows.Forms.Label lblOperand;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.Label lblDnD;
		private System.Windows.Forms.ComboBox cbxOpcode;
		private System.Windows.Forms.ToolStripMenuItem tsmDelete;
		private System.Windows.Forms.TextBox txtOperand;
		private MetroObjects.MCheckBox chbDelete;
		private System.Windows.Forms.Label lblDelete;
		private System.Windows.Forms.Button btnNewOpCode;
		public System.Windows.Forms.TextBox txtMethodFullName;
		private System.Windows.Forms.Panel panTMFPicker;
		private System.Windows.Forms.Label lblTMFPicker;
		private System.Windows.Forms.Button btnTMFPicker;
		private System.Windows.Forms.ToolStripMenuItem tsmRemove;
		private System.Windows.Forms.ToolStripMenuItem tsmUnDelete;
	}
}