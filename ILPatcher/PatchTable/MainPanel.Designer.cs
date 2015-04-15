namespace ILPatcher
{
	partial class MainPanel
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Komponenten-Designer generierter Code

		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("actionxxx");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("actionxxy");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Patch1", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Patch2");
			this.btnCreatePatch = new System.Windows.Forms.Button();
			this.clbPatchList = new System.Windows.Forms.CheckedListBox();
			this.tabInfoControl = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.structureViever1 = new ILPatcher.StructureViewer();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.rtbInfo = new System.Windows.Forms.TextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.lbxErrors = new MetroObjects.MListBox();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.btnSavePatchList = new System.Windows.Forms.Button();
			this.btnTestPatch = new System.Windows.Forms.Button();
			this.btnLoadila = new System.Windows.Forms.Button();
			this.btnLoadilp = new System.Windows.Forms.Button();
			this.txtilpFile = new System.Windows.Forms.Label();
			this.txtilaFile = new System.Windows.Forms.Label();
			this.btnExecutePatches = new System.Windows.Forms.Button();
			this.mLoading = new MetroObjects.MLoadingCircle();
			this.btnEditPatch = new System.Windows.Forms.Button();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.mPatchListTest = new MetroObjects.MListBox();
			this.tabInfoControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCreatePatch
			// 
			this.btnCreatePatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnCreatePatch.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnCreatePatch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnCreatePatch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnCreatePatch.Location = new System.Drawing.Point(332, 444);
			this.btnCreatePatch.Margin = new System.Windows.Forms.Padding(5);
			this.btnCreatePatch.Name = "btnCreatePatch";
			this.btnCreatePatch.Size = new System.Drawing.Size(132, 22);
			this.btnCreatePatch.TabIndex = 18;
			this.btnCreatePatch.Text = "create ILPatch";
			this.btnCreatePatch.UseVisualStyleBackColor = false;
			this.btnCreatePatch.Click += new System.EventHandler(this.btnNewPatch_Click);
			// 
			// clbPatchList
			// 
			this.clbPatchList.FormattingEnabled = true;
			this.clbPatchList.Location = new System.Drawing.Point(5, 70);
			this.clbPatchList.Margin = new System.Windows.Forms.Padding(5);
			this.clbPatchList.Name = "clbPatchList";
			this.clbPatchList.Size = new System.Drawing.Size(317, 379);
			this.clbPatchList.TabIndex = 17;
			// 
			// tabInfoControl
			// 
			this.tabInfoControl.Controls.Add(this.tabPage1);
			this.tabInfoControl.Controls.Add(this.tabPage2);
			this.tabInfoControl.Controls.Add(this.tabPage3);
			this.tabInfoControl.Controls.Add(this.tabPage4);
			this.tabInfoControl.Controls.Add(this.tabPage5);
			this.tabInfoControl.Location = new System.Drawing.Point(332, 70);
			this.tabInfoControl.Margin = new System.Windows.Forms.Padding(5);
			this.tabInfoControl.Name = "tabInfoControl";
			this.tabInfoControl.SelectedIndex = 0;
			this.tabInfoControl.Size = new System.Drawing.Size(297, 362);
			this.tabInfoControl.TabIndex = 16;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.structureViever1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(289, 336);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Assemblylist";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// structureViever1
			// 
			this.structureViever1.ContextAssemblyLoad = true;
			this.structureViever1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.structureViever1.ImageIndex = 0;
			this.structureViever1.Location = new System.Drawing.Point(3, 3);
			this.structureViever1.Name = "structureViever1";
			this.structureViever1.PathSeparator = ".";
			this.structureViever1.SelectedImageIndex = 0;
			this.structureViever1.Size = new System.Drawing.Size(283, 330);
			this.structureViever1.TabIndex = 0;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.rtbInfo);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(289, 336);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Fileinfo";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// rtbInfo
			// 
			this.rtbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbInfo.Location = new System.Drawing.Point(3, 3);
			this.rtbInfo.Multiline = true;
			this.rtbInfo.Name = "rtbInfo";
			this.rtbInfo.Size = new System.Drawing.Size(283, 330);
			this.rtbInfo.TabIndex = 2;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.lbxErrors);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(289, 336);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Error List";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// lbxErrors
			// 
			this.lbxErrors.AllowDrag = false;
			this.lbxErrors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbxErrors.Location = new System.Drawing.Point(0, 0);
			this.lbxErrors.MinimumSize = new System.Drawing.Size(50, 50);
			this.lbxErrors.Name = "lbxErrors";
			this.lbxErrors.Size = new System.Drawing.Size(289, 336);
			this.lbxErrors.TabIndex = 0;
			this.lbxErrors.Text = "mListBox1";
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.treeView1);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Size = new System.Drawing.Size(289, 336);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "testTab";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// treeView1
			// 
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeView1.CheckBoxes = true;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.FullRowSelect = true;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			treeNode1.Name = "Knoten1";
			treeNode1.Text = "actionxxx";
			treeNode2.Name = "Knoten2";
			treeNode2.Text = "actionxxy";
			treeNode3.Name = "Knoten0";
			treeNode3.Text = "Patch1";
			treeNode4.Name = "Knoten3";
			treeNode4.Text = "Patch2";
			this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4});
			this.treeView1.Size = new System.Drawing.Size(289, 336);
			this.treeView1.TabIndex = 2;
			this.treeView1.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCheck);
			this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
			// 
			// btnSavePatchList
			// 
			this.btnSavePatchList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnSavePatchList.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnSavePatchList.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnSavePatchList.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSavePatchList.Location = new System.Drawing.Point(474, 476);
			this.btnSavePatchList.Margin = new System.Windows.Forms.Padding(5);
			this.btnSavePatchList.Name = "btnSavePatchList";
			this.btnSavePatchList.Size = new System.Drawing.Size(155, 22);
			this.btnSavePatchList.TabIndex = 15;
			this.btnSavePatchList.Text = "safe current Patchlist";
			this.btnSavePatchList.UseVisualStyleBackColor = false;
			this.btnSavePatchList.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnTestPatch
			// 
			this.btnTestPatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnTestPatch.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnTestPatch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnTestPatch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnTestPatch.Location = new System.Drawing.Point(332, 476);
			this.btnTestPatch.Margin = new System.Windows.Forms.Padding(5);
			this.btnTestPatch.Name = "btnTestPatch";
			this.btnTestPatch.Size = new System.Drawing.Size(132, 22);
			this.btnTestPatch.TabIndex = 14;
			this.btnTestPatch.Text = "Debug [TestPatch]";
			this.btnTestPatch.UseVisualStyleBackColor = false;
			this.btnTestPatch.Click += new System.EventHandler(this.btnTestpatch_Click);
			// 
			// btnLoadila
			// 
			this.btnLoadila.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnLoadila.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnLoadila.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnLoadila.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnLoadila.Location = new System.Drawing.Point(5, 35);
			this.btnLoadila.Margin = new System.Windows.Forms.Padding(5);
			this.btnLoadila.Name = "btnLoadila";
			this.btnLoadila.Size = new System.Drawing.Size(130, 25);
			this.btnLoadila.TabIndex = 12;
			this.btnLoadila.Text = "open IL Assembly";
			this.btnLoadila.UseVisualStyleBackColor = false;
			this.btnLoadila.Click += new System.EventHandler(this.btnOpenILFile_Click);
			// 
			// btnLoadilp
			// 
			this.btnLoadilp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnLoadilp.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnLoadilp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnLoadilp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnLoadilp.Location = new System.Drawing.Point(5, 5);
			this.btnLoadilp.Margin = new System.Windows.Forms.Padding(5);
			this.btnLoadilp.Name = "btnLoadilp";
			this.btnLoadilp.Size = new System.Drawing.Size(130, 25);
			this.btnLoadilp.TabIndex = 19;
			this.btnLoadilp.Text = "open Patch File (*.ilp)";
			this.btnLoadilp.UseVisualStyleBackColor = false;
			this.btnLoadilp.Click += new System.EventHandler(this.btnLoadILPatch_Click);
			// 
			// txtilpFile
			// 
			this.txtilpFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtilpFile.Location = new System.Drawing.Point(140, 5);
			this.txtilpFile.Margin = new System.Windows.Forms.Padding(5);
			this.txtilpFile.Name = "txtilpFile";
			this.txtilpFile.Size = new System.Drawing.Size(489, 25);
			this.txtilpFile.TabIndex = 21;
			this.txtilpFile.Text = "<<Patch File>>";
			this.txtilpFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtilaFile
			// 
			this.txtilaFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtilaFile.Location = new System.Drawing.Point(140, 35);
			this.txtilaFile.Margin = new System.Windows.Forms.Padding(5);
			this.txtilaFile.Name = "txtilaFile";
			this.txtilaFile.Size = new System.Drawing.Size(456, 25);
			this.txtilaFile.TabIndex = 22;
			this.txtilaFile.Text = "<< IL File >>";
			this.txtilaFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnExecutePatches
			// 
			this.btnExecutePatches.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnExecutePatches.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnExecutePatches.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnExecutePatches.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnExecutePatches.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnExecutePatches.Location = new System.Drawing.Point(5, 459);
			this.btnExecutePatches.Margin = new System.Windows.Forms.Padding(5);
			this.btnExecutePatches.Name = "btnExecutePatches";
			this.btnExecutePatches.Size = new System.Drawing.Size(317, 35);
			this.btnExecutePatches.TabIndex = 23;
			this.btnExecutePatches.Text = "Execute Selected Patches";
			this.btnExecutePatches.UseVisualStyleBackColor = false;
			this.btnExecutePatches.Click += new System.EventHandler(this.btnExecutePatches_Click);
			// 
			// mLoading
			// 
			this.mLoading.Location = new System.Drawing.Point(604, 35);
			this.mLoading.MinimumSize = new System.Drawing.Size(25, 25);
			this.mLoading.Name = "mLoading";
			this.mLoading.Size = new System.Drawing.Size(25, 25);
			this.mLoading.TabIndex = 24;
			this.mLoading.Text = "mLoadingCircle1";
			// 
			// btnEditPatch
			// 
			this.btnEditPatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnEditPatch.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			this.btnEditPatch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.btnEditPatch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnEditPatch.Location = new System.Drawing.Point(474, 444);
			this.btnEditPatch.Margin = new System.Windows.Forms.Padding(5);
			this.btnEditPatch.Name = "btnEditPatch";
			this.btnEditPatch.Size = new System.Drawing.Size(155, 22);
			this.btnEditPatch.TabIndex = 25;
			this.btnEditPatch.Text = "edit ILPatch";
			this.btnEditPatch.UseVisualStyleBackColor = false;
			this.btnEditPatch.Click += new System.EventHandler(this.btnEditPatch_Click);
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.mPatchListTest);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Size = new System.Drawing.Size(289, 336);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "testList";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// mPatchListTest
			// 
			this.mPatchListTest.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mPatchListTest.Location = new System.Drawing.Point(0, 0);
			this.mPatchListTest.MinimumSize = new System.Drawing.Size(50, 50);
			this.mPatchListTest.Name = "mPatchListTest";
			this.mPatchListTest.Size = new System.Drawing.Size(289, 336);
			this.mPatchListTest.TabIndex = 0;
			this.mPatchListTest.Text = "mListBox1";
			// 
			// MainPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnEditPatch);
			this.Controls.Add(this.mLoading);
			this.Controls.Add(this.clbPatchList);
			this.Controls.Add(this.btnSavePatchList);
			this.Controls.Add(this.btnLoadilp);
			this.Controls.Add(this.btnExecutePatches);
			this.Controls.Add(this.btnLoadila);
			this.Controls.Add(this.btnCreatePatch);
			this.Controls.Add(this.txtilaFile);
			this.Controls.Add(this.txtilpFile);
			this.Controls.Add(this.tabInfoControl);
			this.Controls.Add(this.btnTestPatch);
			this.Name = "MainPanel";
			this.Size = new System.Drawing.Size(634, 506);
			this.Resize += new System.EventHandler(this.MainPanel_Resize);
			this.tabInfoControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.tabPage5.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnCreatePatch;
		private System.Windows.Forms.CheckedListBox clbPatchList;
		private System.Windows.Forms.TabControl tabInfoControl;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TextBox rtbInfo;
		private System.Windows.Forms.Button btnSavePatchList;
		private System.Windows.Forms.Button btnTestPatch;
		private System.Windows.Forms.Button btnLoadila;
		public ILPatcher.StructureViewer structureViever1;
		private System.Windows.Forms.Button btnLoadilp;
		private System.Windows.Forms.Label txtilpFile;
		private System.Windows.Forms.Label txtilaFile;
		private System.Windows.Forms.Button btnExecutePatches;
		private System.Windows.Forms.TabPage tabPage3;
		private MetroObjects.MLoadingCircle mLoading;
		private System.Windows.Forms.Button btnEditPatch;
		public MetroObjects.MListBox lbxErrors;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.TabPage tabPage5;
		private MetroObjects.MListBox mPatchListTest;
	}
}
