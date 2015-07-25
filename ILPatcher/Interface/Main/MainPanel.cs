using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ILPatcher.Data;
using ILPatcher.Interface.General;
using ILPatcher.Utility;
using MetroObjects;
using Mono.Cecil;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ILPatcher.Interface.Main
{
	public partial class MainPanel : Control
	{
		private DataStruct dataStruct;
		bool awaitingAssemblySelect = false;
		string ilpFiletmp;

		#region Interface Elements
		Label lblPathNETFile;
		Label lblPathILPFile;
		MListBox lbxPatchEntryListBox;
		StructureViewer structureViever;
		MLoadingCircle loadingBox;
		MListBox lbxErrors; // TODO: TMP (move into own error manager/ tab manager extra tab)
		#endregion

		public MainPanel()
		{
			Log.callback = VisualLog;

			dataStruct = new DataStruct();
			dataStruct.OnILPFileLoadedDelegate += RebuildTable;

			InitializeGridLineManager();
		}

		private void InitializeGridLineManager()
		{
			this.SuspendLayout();
			lblPathILPFile = GlobalLayout.GenMetroLabel("<<Patch File>>");
			lblPathNETFile = GlobalLayout.GenMetroLabel("<< .NET Assembly >>");
			lbxPatchEntryListBox = new MListBox();
			structureViever = new StructureViewer(dataStruct);
			structureViever.PathSeparator = ".";
			loadingBox = new MLoadingCircle();
			lbxErrors = new MListBox();

			var grid = new GridLineManager(this, true);
			int line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Open patch file (*.ilp)", OpenILPatchFile_Click), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, lblPathILPFile, GlobalLayout.MinFill);
			grid.AddElementStrechable(line, GlobalLayout.GenMetroButton("Save", Save_Click), GlobalLayout.MinFill, GlobalLayout.LabelWidth);
			line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("Open .NET assembly", OpenNETAssembly_Click), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, lblPathNETFile, GlobalLayout.MinFill);
			grid.AddElementFixed(line, loadingBox, GlobalLayout.LineHeight);
			line = grid.AddLineFilling(GlobalLayout.LineHeight);
			grid.AddElementFilling(line, structureViever, GlobalLayout.MinFill);
			grid.AddElementFilling(line, lbxPatchEntryListBox, GlobalLayout.MinFill);
			line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFilling(line, GlobalLayout.GenMetroButton("[Execute patches]", ExecutePatches_Click), GlobalLayout.MinFill);
			grid.AddElementFilling(line, GlobalLayout.GenMetroButton("(Testpatch)", Testpatch_Click), GlobalLayout.MinFill);
			grid.AddElementFilling(line, GlobalLayout.GenMetroButton("[New patch]", NewPatch_Click), GlobalLayout.MinFill);
			grid.AddElementFilling(line, GlobalLayout.GenMetroButton("[Edit patch]", EditPatch_Click), GlobalLayout.MinFill);
			this.ResumeLayout(false);
		}

		// Interface events

		private void OpenNETAssembly_Click(object sender, EventArgs e)
		{
			openAssembly();
		}

		private void OpenILPatchFile_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openIlp = new OpenFileDialog())
			{
				openIlp.Filter = "ILPatch File | *.ilp";
				if (openIlp.ShowDialog() == DialogResult.OK)
				{
					ilpFiletmp = openIlp.FileName;
					lblPathILPFile.Text = ilpFiletmp;
					if (dataStruct.AssemblyStatus == AssemblyStatus.RawAssemblyLoaded)
					{
						dataStruct.OpenILP(openIlp.FileName);
					}
					else
						awaitingAssemblySelect = true;
				}
			}
		}

		private void Save_Click(object sender, EventArgs e)
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

		// Graphical break

		private void ExecutePatches_Click(object sender, EventArgs e)
		{
			// XXX: Maby move into dataStruct or make own backup manager (incl. readonly stuff after writing)
			try
			{
				string backupPath = dataStruct.AssemblyLocation + "_ilpbackup";
				if (!File.Exists(backupPath))
					File.Copy(dataStruct.AssemblyLocation, backupPath);
				foreach (var pe in dataStruct.PatchEntryList) // TODO: check if item checked
				{
					pe.Execute();
				}
				dataStruct.AssemblyDefinition.Write(dataStruct.AssemblyLocation);
			}
			catch (Exception ex) { MessageBox.Show(ex.Message); }
		}

		private void Testpatch_Click(object sender, EventArgs e)
		{
			//((SwooshPanel)Parent).PushPanel(new ILPatcher.Interface.Actions.EditorMethodCreator(dataStruct), "Debug Disassemble"); // HACK: Change to dataStruct param
			//TestMet1();
			//TestMet2();
		}

		private void NewPatch_Click(object sender, EventArgs e)
		{
			EditCluster(null);
		}

		private void EditPatch_Click(object sender, EventArgs e)
		{
			DragItem diElemet = lbxPatchEntryListBox.SelectedElement as DragItem;
			if (diElemet == null)
				return;
			PatchEntry patchCluster = null as PatchEntry; // TODO get stuff
			if (patchCluster == null)
				return;
			EditCluster(patchCluster);
		}

		// Functionality methods

		private void TestMet1()
		{
			if (structureViever.SelectedNode == null) return;
			TypeDefinition TypDef = structureViever.SelectedNode.Tag as TypeDefinition;
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
			CSCompiler csc = new CSCompiler(dataStruct.AssemblyDefinition);
			csc.Code = test;
			MethodDefinition md = csc.GetMethodDefinition(string.Empty, string.Empty);
			if (md == null) return;
			TypeDefinition tdret = (TypeDefinition)structureViever.FindTypeByName("-.System.Void");
			if (tdret == null) return;
			MethodDefinition md2 = new MethodDefinition("blub", MethodAttributes.Public, tdret);
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
					dataStruct.AssemblyDefinition.MainModule.Runtime = TargetRuntime.Net_4_0;
					dataStruct.AssemblyDefinition.Write(saveFileDialog.FileName);
					MessageBox.Show("Message Successfuly Injected");
				}
			}
		}

		private void TestMet2()
		{
			if (structureViever.SelectedNode == null) return;
			MethodDefinition MetDef = structureViever.SelectedNode.Tag as MethodDefinition;
			if (MetDef == null) return;

			DecompilerContext decon = new DecompilerContext(dataStruct.AssemblyDefinition.MainModule);
			decon.CancellationToken = new System.Threading.CancellationToken();
			decon.CurrentType = MetDef.DeclaringType;
			DecompilerSettings decoset = new DecompilerSettings();
			decon.Settings = decoset;
			AstBuilder ast = new AstBuilder(decon);

			ast.AddMethod(MetDef);
			PlainTextOutput pto = new PlainTextOutput();
			ast.GenerateCode(pto);

			var emc = new ILPatcher.Interface.Actions.EditorMethodCreator(dataStruct);
			//emc.txtInjectCode.Text = pto.ToString();
			((SwooshPanel)Parent).PushPanel(emc, "Debug Disassemble");
		}

		private void openAssembly()
		{
			using (OpenFileDialog openAsm = new OpenFileDialog())
			{
				openAsm.Filter = ".NET Managed Code | *.exe;*.dll";
				if (openAsm.ShowDialog() == DialogResult.OK)
				{
					string assemblyPath = openAsm.FileName;
					string backupPath = assemblyPath + "_ilpbackup";
					if (File.Exists(backupPath))
					{
						using (PatchQuestionWindow pqw = new PatchQuestionWindow())
						{
							DialogResult dr = pqw.ShowDialog(this);
							if (dr == DialogResult.Cancel) return;
							else if (dr == DialogResult.Yes) File.Copy(backupPath, assemblyPath, true); // TODO: check for rights
						}
					}
					loadingBox.ON = true;

					dataStruct.OpenASM(assemblyPath);
					if (dataStruct.AssemblyStatus == AssemblyStatus.RawAssemblyLoaded)
					{
						lblPathNETFile.Text = assemblyPath;
						if (awaitingAssemblySelect)
						{
							dataStruct.OpenILP(ilpFiletmp);
						}
					}
					else if (dataStruct.AssemblyStatus == AssemblyStatus.LoadFailed)
					{
						MessageBox.Show("Couldn't read assembly, it is either unmanaged or obfuscated");
					}
					loadingBox.ON = false;
				}
			}
		}

		public void EditCluster(PatchEntry patchcluster)
		{
			PatchBuilder patchBuilder = new PatchBuilder(dataStruct);
			patchBuilder.LoadEntry(patchcluster);
			((SwooshPanel)Parent).PushPanel(patchBuilder, "PatchBuilder");
		}

		private void RebuildTable(object sender)
		{
			DataStruct senderDataStruct = (DataStruct)sender;
			foreach (var patchEntry in senderDataStruct.PatchEntryList)
			{
				lbxPatchEntryListBox.AddItem(new ILPatcher.Interface.Actions.DefaultDragItem<PatchEntry>(patchEntry));
			}
		}

		private void VisualLog(ErrorLoggerItem eli)
		{
			lbxErrors.AddItem(eli);
			lbxErrors.InvalidateBuffer();
		}
	}
}
