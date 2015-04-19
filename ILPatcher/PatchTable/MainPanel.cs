using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using System.Reflection;
using System.Diagnostics;
using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public partial class MainPanel : UserControl
	{
		public static AssemblyStatus status = AssemblyStatus.Uninitialized;
		public static AssemblyDefinition AssemblyDef { get; private set; }
		public static string AssemblyPath { get; private set; }
		public TableManager tablemgr;
		bool AwaitingAssemblySelect = false;
		string ilpFiletmp;


		public MainPanel()
		{
			InitializeComponent();

			Log.callback = VisualLog;
			tablemgr = new TableManager();
		}

		private void btnOpenILFile_Click(object sender, EventArgs e)
		{
			openAssembly();
		}

		private void btnLoadILPatch_Click(object sender, EventArgs e)
		{
			OpenFileDialog openIlp = new OpenFileDialog();
			openIlp.Filter = "ILPatch File | *.ilp";
			if (openIlp.ShowDialog() == DialogResult.OK)
			{
				ilpFiletmp = openIlp.FileName;
				txtilpFile.Text = ilpFiletmp;
				if (status == AssemblyStatus.RawAssemblyLoaded || status == AssemblyStatus.AssemblyAndDataLoaded)
					LoadIlpFile(openIlp.FileName);
				else
					AwaitingAssemblySelect = true;
			}
		}


		private void btnSave_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.AddExtension = true;
			sfd.OverwritePrompt = true;
			sfd.CheckPathExists = true;
			sfd.Filter = "ILPatcher File|*.ilp";
			if (sfd.ShowDialog() != DialogResult.OK) return;
			NameCompressor.Compress = false;
			XmlDocument xDoc = new XmlDocument();
			tablemgr.Save(xDoc);
			XMLUtility.SaveToFile(xDoc, sfd.FileName);
		}

		private void btnNewPatch_Click(object sender, EventArgs e)
		{
			OpenCluster(null);
		}

		private void btnEditPatch_Click(object sender, EventArgs e)
		{
			if (clbPatchList.SelectedIndex >= 0)
				OpenCluster(tablemgr.ClusterList[clbPatchList.SelectedIndex]);
		}

		private void btnTestpatch_Click(object sender, EventArgs e)
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
			CSCompiler csc = new CSCompiler(AssemblyDef);
			Mono.Cecil.MethodDefinition md = csc.InjectCode(test);
			if (md == null) return;
			TypeDefinition tdret = (TypeDefinition)ILManager.Instance.FindTypeByName("-.System.Void");
			if (tdret == null) return;
			MethodDefinition md2 = new MethodDefinition("blub", Mono.Cecil.MethodAttributes.Public, tdret);
			TypDef.Methods.Add(md2);

			CecilHelper.CloneMethodBody(md, md2);

			//foreach (Instruction i in md.Body.Instructions)
			//	md2.Body.Instructions.Add(i);

			/*if (structureViever1.SelectedNode == null) return;
			MethodDefinition MetDef = structureViever1.SelectedNode.Tag as MethodDefinition;
			if (MetDef == null) return;

			//ILProcessor cilProcess = MetDef.Body.GetILProcessor();
			//MessageBox.Show("Test",);
			MethodInfo method = typeof(MessageBox).GetMethod("Show",
			 new Type[] { typeof(string), typeof(string), typeof(MessageBoxButtons), typeof(MessageBoxIcon) });
			MethodReference method2 = AssemblyDef.MainModule.Import(method);*/

			using (SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Title = "Save to :",
				Filter = "Executables | *.exe;*.dll"
			})
			{
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					AssemblyDef.MainModule.Runtime = TargetRuntime.Net_4_0;
					AssemblyDef.Write(saveFileDialog.FileName);
					MessageBox.Show("Message Successfuly Injected");
				}
			}
		}

		private void btnExecutePatches_Click(object sender, EventArgs e)
		{
			try
			{
				string backupPath = AssemblyPath + "_ilpbackup";
				if (!File.Exists(backupPath))
					File.Copy(AssemblyPath, backupPath);
				tabInfoControl.SelectedIndex = 2;
				for (int i = 0; i < tablemgr.ClusterList.Count; i++)
					if (clbPatchList.GetItemChecked(i))
						tablemgr.ClusterList[i].Execute();
				AssemblyDef.Write(AssemblyPath);
			}
			catch (Exception ex) { MessageBox.Show(ex.Message); }
		}


		private void openAssembly()
		{
			status = AssemblyStatus.Uninitialized;
			OpenFileDialog openAsm = new OpenFileDialog();
			openAsm.Filter = ".NET Managed Code | *.exe;*.dll";
			if (openAsm.ShowDialog() == DialogResult.OK)
			{
				AssemblyPath = openAsm.FileName;
				txtilaFile.Text = AssemblyPath;
				string backupPath = AssemblyPath + "_ilpbackup";
				if (File.Exists(backupPath))
				{
					PatchQuestionWindow pqw = new PatchQuestionWindow();
					DialogResult dr = pqw.ShowDialog(this);
					if (dr == DialogResult.Cancel) return;
					else if (dr == DialogResult.Yes) File.Copy(backupPath, AssemblyPath, true); // check for rights
				}
				ILManager.Instance.ClearAll();

				mLoading.ON = true;
				LoadAsmOrigin();
				if (status == AssemblyStatus.RawAssemblyLoaded || status == AssemblyStatus.AssemblyAndDataLoaded)
				{
					ILManager.Instance.InitTreeHalfAsync(AssemblyDef);
					structureViever1.RebuildHalfAsync();
				}
				if (AwaitingAssemblySelect)
				{
					LoadIlpFile(ilpFiletmp);
				}
				mLoading.ON = false;
			}
		}

		private void LoadAsmOrigin()
		{
			try
			{
				AssemblyDef = AssemblyDefinition.ReadAssembly(AssemblyPath);
				status = AssemblyStatus.RawAssemblyLoaded;

				rtbInfo.Clear();
				rtbInfo.AppendText("[Name]::" + AssemblyDef.MainModule.Name.ToString() + Environment.NewLine);
				rtbInfo.AppendText("[CLR Runtime]::" + AssemblyDef.MainModule.Runtime.ToString() + Environment.NewLine);
				rtbInfo.AppendText("[Full Name]::" + AssemblyDef.MainModule.FullyQualifiedName.ToString() + Environment.NewLine);
				rtbInfo.AppendText("[Metadata Token]::" + AssemblyDef.MainModule.MetadataToken.ToString() + Environment.NewLine);
				rtbInfo.AppendText("[Architecture]::" + AssemblyDef.MainModule.Architecture.ToString() + Environment.NewLine);
				if (AssemblyDef.MainModule.EntryPoint != null)
					rtbInfo.AppendText("[EntryPoint]::" + AssemblyDef.MainModule.EntryPoint.ToString() + Environment.NewLine);
				rtbInfo.AppendText("[Mvid]::" + AssemblyDef.MainModule.Mvid.ToString() + Environment.NewLine);
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
			XmlDocument xDoc = XMLUtility.ReadFromFile(filename);
			if (tablemgr == null) // TODO tableMgr clear
				tablemgr = new TableManager();
			XmlNode BaseNode = null;
			bool Match = false;
			NameCompressor nc = NameCompressor.Instance;
			foreach (XmlNode xnode in xDoc.ChildNodes)
			{
				if (xnode.Name == nc.GetValComp(SST.PatchTable)) { Match = true; NameCompressor.Compress = true; }
				else if (xnode.Name == nc.GetValUnComp(SST.PatchTable)) { Match = true; NameCompressor.Compress = false; }
				if (Match) { BaseNode = xnode; break; }
			}
			if (Match)
			{
				tablemgr.Load(BaseNode);
				RebuildTable();
			}
			else
				Log.Write(Log.Level.Error, "No PatchTable found!");
			//mLoading.ON = false;
		}

		public void Add(PatchCluster pe)
		{
			if (tablemgr == null)
				tablemgr = new TableManager();
			tablemgr.Add(pe);
			RebuildTable();
		}

		public void OpenCluster(PatchCluster patchcluster)
		{
			EditorCluster ee = new EditorCluster(Add);
			ee.LoadCluster(null);
			((SwooshPanel)Parent).PushPanel(ee, "Patch Cluster");
		}

		private void RebuildTable()
		{
			bool[] checkState = new bool[clbPatchList.Items.Count];
			for (int i = 0; i < clbPatchList.Items.Count; i++)
				checkState[i] = clbPatchList.GetItemChecked(i);
			clbPatchList.Items.Clear();
			for (int i = 0; i < tablemgr.ClusterList.Count; i++)
				clbPatchList.Items.Add(tablemgr.ClusterList[i].ClusterName, i < checkState.Length ? checkState[i] : false);
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

		private void treeView1_MouseUp(object sender, MouseEventArgs e)
		{
			var node = ((TreeView)sender).GetNodeAt(new Point(e.X, e.Y));
			if (node != null && node.Level > 0 && node.Checked)
				node.Checked = false;
		}
	}

	public enum AssemblyStatus
	{
		Uninitialized,
		RawAssemblyLoaded,
		AssemblyAndDataLoaded,
		//Operatable = RawAssemblyLoaded | AssemblyAndDataLoaded,
		LoadFailed,
	}
}
