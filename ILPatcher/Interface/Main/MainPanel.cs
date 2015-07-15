using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ILPatcher.Data;
using ILPatcher.Interface.General;
using ILPatcher.Utility;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace ILPatcher.Interface.Main
{
	public partial class MainPanel : UserControl
	{
		public static AssemblyStatus status = AssemblyStatus.Uninitialized;
		public static AssemblyDefinition MainAssemblyDefinition { get; private set; } // TODO: get rid of static
		public static string AssemblyPath { get; private set; }
		public DataStruct dataStruct;
		bool awaitingAssemblySelect = false;
		string ilpFiletmp;


		public MainPanel()
		{
			InitializeComponent();

			Log.callback = VisualLog;
			dataStruct = new DataStruct();
		}

		private void btnOpenILFile_Click(object sender, EventArgs e)
		{
			openAssembly();
		}

		private void btnLoadILPatch_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openIlp = new OpenFileDialog())
			{
				openIlp.Filter = "ILPatch File | *.ilp";
				if (openIlp.ShowDialog() == DialogResult.OK)
				{
					ilpFiletmp = openIlp.FileName;
					txtilpFile.Text = ilpFiletmp;
					if (status == AssemblyStatus.RawAssemblyLoaded || status == AssemblyStatus.AssemblyAndDataLoaded)
						LoadIlpFile(openIlp.FileName);
					else
						awaitingAssemblySelect = true;
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				sfd.AddExtension = true;
				sfd.OverwritePrompt = true;
				sfd.CheckPathExists = true;
				sfd.Filter = "ILPatcher File | *.ilp";
				if (sfd.ShowDialog() != DialogResult.OK) return;
				NameCompressor.Compress = false;
				XmlDocument xDoc = new XmlDocument();
				dataStruct.Save(xDoc);
				XMLUtility.SaveToFile(xDoc, sfd.FileName);
			}
		}

		private void btnNewPatch_Click(object sender, EventArgs e)
		{
			EditCluster(null);
		}

		private void btnEditPatch_Click(object sender, EventArgs e)
		{
			TreeNode treeNode = treeView1.SelectedNode;
			if (treeNode == null)
				return;
			PatchEntry patchCluster = treeNode.Tag as PatchEntry;
			if (patchCluster == null)
				return;
			EditCluster(patchCluster);
		}

		private void btnTestpatch_Click(object sender, EventArgs e)
		{
			((SwooshPanel)Parent).PushPanel(new ILPatcher.Interface.Actions.EditorMethodCreator(x => { }, MainAssemblyDefinition), "Debug Disassemble");
			//TestMet1();
			//TestMet2();

		}

		private void TestMet1()
		{
			if (structureViever1.SelectedNode == null) return;
			TypeDefinition TypDef = structureViever1.SelectedNode.Tag as TypeDefinition;
			if (TypDef == null) return;

			string test = @"using System.Windows.Forms;
							namespace DefNamSp
							{
							  public class DefClass
							  {
								public void DefFunc()
								{
									MessageBox.Show(""Message Successfuly Injected"");
								}
							  }
							}";
			CSCompiler csc = new CSCompiler(MainAssemblyDefinition);
			csc.Code = test;
			Mono.Cecil.MethodDefinition md = csc.GetMethodDefinition(string.Empty, string.Empty);
			if (md == null) return;
			TypeDefinition tdret = (TypeDefinition)ILManager.Instance.FindTypeByName("-.System.Void");
			if (tdret == null) return;
			MethodDefinition md2 = new MethodDefinition("blub", Mono.Cecil.MethodAttributes.Public, tdret);
			TypDef.Methods.Add(md2);

			CecilHelper.CloneMethodBody(md, md2);

			using (SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Title = "Save to :",
				Filter = "Executables | *.exe;*.dll"
			})
			{
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					MainAssemblyDefinition.MainModule.Runtime = TargetRuntime.Net_4_0;
					MainAssemblyDefinition.Write(saveFileDialog.FileName);
					MessageBox.Show("Message Successfuly Injected");
				}
			}
		}

		private void TestMet2()
		{
			if (structureViever1.SelectedNode == null) return;
			MethodDefinition MetDef = structureViever1.SelectedNode.Tag as MethodDefinition;
			if (MetDef == null) return;

			DecompilerContext decon = new DecompilerContext(MainAssemblyDefinition.MainModule);
			decon.CancellationToken = new System.Threading.CancellationToken();
			decon.CurrentType = MetDef.DeclaringType;
			DecompilerSettings decoset = new DecompilerSettings();
			decon.Settings = decoset;
			AstBuilder ast = new AstBuilder(decon);

			ast.AddMethod(MetDef);
			PlainTextOutput pto = new PlainTextOutput();
			ast.GenerateCode(pto);

			var emc = new ILPatcher.Interface.Actions.EditorMethodCreator(x => { }, MainAssemblyDefinition);
			//emc.txtInjectCode.Text = pto.ToString();
			((SwooshPanel)Parent).PushPanel(emc, "Debug Disassemble");
		}

		private void btnExecutePatches_Click(object sender, EventArgs e)
		{
			try
			{
				string backupPath = AssemblyPath + "_ilpbackup";
				if (!File.Exists(backupPath))
					File.Copy(AssemblyPath, backupPath);
				tabInfoControl.SelectedIndex = 2;
				for (int i = 0; i < dataStruct.ClusterList.Count; i++)
					if (clbPatchList.GetItemChecked(i))
						dataStruct.ClusterList[i].Execute();
				MainAssemblyDefinition.Write(AssemblyPath);
			}
			catch (Exception ex) { MessageBox.Show(ex.Message); }
		}


		private void openAssembly()
		{
			status = AssemblyStatus.Uninitialized;
			using (OpenFileDialog openAsm = new OpenFileDialog())
			{
				openAsm.Filter = ".NET Managed Code | *.exe;*.dll";
				if (openAsm.ShowDialog() == DialogResult.OK)
				{
					AssemblyPath = openAsm.FileName;
					txtilaFile.Text = AssemblyPath;
					string backupPath = AssemblyPath + "_ilpbackup";
					if (File.Exists(backupPath))
					{
						using (PatchQuestionWindow pqw = new PatchQuestionWindow())
						{
							DialogResult dr = pqw.ShowDialog(this);
							if (dr == DialogResult.Cancel) return;
							else if (dr == DialogResult.Yes) File.Copy(backupPath, AssemblyPath, true); // check for rights
						}
					}
					ILManager.Instance.ClearAll();

					mLoading.ON = true;
					LoadAsmOrigin();
					if (status == AssemblyStatus.RawAssemblyLoaded || status == AssemblyStatus.AssemblyAndDataLoaded)
					{
						ILManager.Instance.InitTreeHalfAsync(MainAssemblyDefinition);
						structureViever1.RebuildHalfAsync();
					}
					if (awaitingAssemblySelect)
					{
						LoadIlpFile(ilpFiletmp);
					}
					mLoading.ON = false;
				}
			}
		}

		private void LoadAsmOrigin()
		{
			try
			{
				MainAssemblyDefinition = AssemblyDefinition.ReadAssembly(AssemblyPath);
				status = AssemblyStatus.RawAssemblyLoaded;

				rtbInfo.Clear();
				rtbInfo.AppendText("[Name]::" + MainAssemblyDefinition.MainModule.Name.ToString() + Environment.NewLine);
				rtbInfo.AppendText("[CLR Runtime]::" + MainAssemblyDefinition.MainModule.Runtime.ToString() + Environment.NewLine);
				rtbInfo.AppendText("[Full Name]::" + MainAssemblyDefinition.MainModule.FullyQualifiedName.ToString() + Environment.NewLine);
				rtbInfo.AppendText("[Metadata Token]::" + MainAssemblyDefinition.MainModule.MetadataToken.ToString() + Environment.NewLine);
				rtbInfo.AppendText("[Architecture]::" + MainAssemblyDefinition.MainModule.Architecture.ToString() + Environment.NewLine);
				if (MainAssemblyDefinition.MainModule.EntryPoint != null)
					rtbInfo.AppendText("[EntryPoint]::" + MainAssemblyDefinition.MainModule.EntryPoint.ToString() + Environment.NewLine);
				rtbInfo.AppendText("[Mvid]::" + MainAssemblyDefinition.MainModule.Mvid.ToString() + Environment.NewLine);
				status = AssemblyStatus.AssemblyAndDataLoaded;
			}
			catch (Exception ex)
			{
				if (status == AssemblyStatus.Uninitialized)
				{
					status = AssemblyStatus.LoadFailed;
					MessageBox.Show("Couldn't read assembly, it is either unmanaged or obfuscated");
				}
				else if (status == AssemblyStatus.RawAssemblyLoaded)
				{
					status = AssemblyStatus.LoadFailed;
					MessageBox.Show("The Assembly was loaded, yet some errors occoured. Patching might work... ;)\n" +
						ex.Message);
				}
			}
		}

		public void LoadIlpFile(string filename)
		{
			//mLoading.ON = true;
			XmlDocument xDoc = XMLUtility.ReadFromFile(filename); // TODO Rework too
			dataStruct.Clear();
			XmlNode BaseNode = null;
			bool Match = false;
			NameCompressor nc = NameCompressor.Instance;
			foreach (XmlNode xnode in xDoc.ChildNodes)
			{
				if (xnode.Name == nc.GetValComp(SST.ILPTable)) { Match = true; NameCompressor.Compress = true; }
				else if (xnode.Name == nc.GetValUnComp(SST.ILPTable)) { Match = true; NameCompressor.Compress = false; }
				if (Match) { BaseNode = xnode; break; }
			}
			if (Match)
			{
				dataStruct.Load(BaseNode);
				RebuildTable();
			}
			else
				Log.Write(Log.Level.Error, "No PatchTable found!");
			//mLoading.ON = false;
		}

		public void Add(PatchEntry pe)
		{
			// TODO: Rework
			dataStruct.Add(pe);
			TreeNode matchNode = treeView1.Nodes.OfType<TreeNode>().FirstOrDefault(node => node.Tag == pe);
			if (matchNode == null)
				treeView1.Nodes.Add(GenClusterNode(pe));
		}

		public void EditCluster(PatchEntry patchcluster)
		{
			PatchBuilder patchBuilder = new PatchBuilder(Add, MainAssemblyDefinition);
			patchBuilder.LoadCluster(patchcluster);
			((SwooshPanel)Parent).PushPanel(patchBuilder, "PatchBuilder");
		}

		private void RebuildTable()
		{
			var labels = new Dictionary<string, TreeNode>();
			foreach (var patchCluster in dataStruct.ClusterList)
			{
				TreeNode treeNode = GenClusterNode(patchCluster);

				if (patchCluster.Label == null)
					treeView1.Nodes.Add(treeNode);
				if (!labels.ContainsKey(patchCluster.Label))
				{
					TreeNode labelNode = new TreeNode(patchCluster.Label);
					labels.Add(patchCluster.Label, labelNode);
				}
				labels[patchCluster.Label].Nodes.Add(treeNode);
			}
		}

		private static TreeNode GenClusterNode(PatchEntry patchCluster)
		{
			TreeNode treeNode = new TreeNode(patchCluster.Name);
			treeNode.Tag = patchCluster;
			return treeNode;
		}

		private void MainPanel_Resize(object sender, EventArgs e)
		{
			const int space = 5;

			txtilpFile.Width = Width - (btnLoadilp.Width + 3 * space);
			txtilaFile.Width = Width - (btnLoadila.Width + mLoading.Width + 4 * space);
			mLoading.Left = Width - (mLoading.Width + space);

			clbPatchList.Width = Width / 2;
			clbPatchList.Height = Height - (clbPatchList.Top + btnExecutePatches.Height + 2 * space);

			btnExecutePatches.Top = clbPatchList.Bottom + space;
			btnExecutePatches.Width = clbPatchList.Width;

			tabInfoControl.Left = clbPatchList.Right + space;
			tabInfoControl.Width = Width - (clbPatchList.Width + 3 * space);
			tabInfoControl.Height = Height - (tabInfoControl.Top + btnTestPatch.Height + btnCreatePatch.Height + 3 * space);

			btnCreatePatch.Top = Height - (btnCreatePatch.Height + btnTestPatch.Height + 2 * space);
			btnCreatePatch.Left = tabInfoControl.Left;
			btnCreatePatch.Width = tabInfoControl.Width / 2;

			btnEditPatch.Top = btnCreatePatch.Top;
			btnEditPatch.Left = btnCreatePatch.Right + space;
			btnEditPatch.Width = tabInfoControl.Width / 2 - space;

			btnTestPatch.Top = Height - (btnTestPatch.Height + space);
			btnTestPatch.Left = tabInfoControl.Left;
			btnTestPatch.Width = btnCreatePatch.Width;

			btnSavePatchList.Top = btnTestPatch.Top;
			btnSavePatchList.Left = btnEditPatch.Left;
			btnSavePatchList.Width = btnEditPatch.Width;
		}


		private void VisualLog(ErrorLoggerItem eli)
		{
			lbxErrors.AddItem(eli);
			lbxErrors.InvalidateBuffer();
		}

		private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Action != TreeViewAction.Unknown)
			{
				if (e.Node.Level > 0)
				{
					e.Cancel = true;
					return;
				}
				if (e.Node.Nodes.Count > 0)
				{
					CheckAllChildNodes(e.Node, !e.Node.Checked);
				}
			}
		}

		private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
		{
			foreach (TreeNode node in treeNode.Nodes)
			{
				node.Checked = nodeChecked;
				if (node.Nodes.Count > 0)
				{
					this.CheckAllChildNodes(node, nodeChecked);
				}
			}
		}

		bool drag = false;
		TreeNode dragObj = null;

		private void treeView1_MouseUp(object sender, MouseEventArgs e)
		{

			var node = ((TreeView)sender).GetNodeAt(new Point(e.X, e.Y));
			if (node != null && node.Level > 0 && node.Checked)
				node.Checked = false;
		}

		private void treeView1_MouseDown(object sender, MouseEventArgs e)
		{
			TreeNode tn = treeView1.SelectedNode;
			dragObj = tn;
			drag = tn != null;
		}

		private void treeView1_MouseMove(object sender, MouseEventArgs e)
		{
			if (drag)
				treeView1.DoDragDrop(dragObj, DragDropEffects.Move);
		}

		private void treeView1_DragDrop(object sender, DragEventArgs e)
		{
			Point point = treeView1.PointToClient(new Point(e.X, e.Y));
			TreeNode tn = treeView1.GetNodeAt(point);
			TreeNode tnData = (TreeNode)e.Data.GetData(typeof(TreeNode));
			if (tn.Level == 0 && (int)tnData.Tag == 0)
			{

			}
		}
	}

	public enum AssemblyStatus
	{
		Uninitialized,
		RawAssemblyLoaded,
		AssemblyAndDataLoaded,
		LoadFailed,
	}
}
