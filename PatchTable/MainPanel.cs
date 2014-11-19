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

		private static MainPanel _Instance;
		public static MainPanel Instance
		{
			get { if (_Instance == null) _Instance = new MainPanel(); return _Instance; }
			protected set { _Instance = value; }
		}

		private MainPanel()
		{
			InitializeComponent();

			/*Log.Write(Log.Level.Info, "Infotest");
			Log.Write(Log.Level.Careful, "Carefultest");
			Log.Write(Log.Level.Warning, "Warningtest");
			Log.Write(Log.Level.Error, "Errortest");*/

			tablemgr = new TableManager();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			openAssembly();
		}

		private void openAssembly()
		{
			status = AssemblyStatus.Uninitialized;
			OpenFileDialog openAsm = new OpenFileDialog();
			openAsm.Filter = "C# Managed Code | *.exe;*.dll";
			if (openAsm.ShowDialog() == DialogResult.OK)
			{
				mLoading.ON = true;
				AssemblyPath = openAsm.FileName;
				txtilaFile.Text = AssemblyPath;
				LoadAsmOrigin();
				if (status == AssemblyStatus.RawAssemblyLoaded || status == AssemblyStatus.AssemblyAndDataLoaded)
				{
					ILManager.Instance.InitTreeHalfAsync(AssemblyDef, 1);
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
				AssemblyDef = AssemblyDefinition.ReadAssembly(txtilaFile.Text);
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

		private void button2_Click(object sender, EventArgs e)
		{
			MethodDefinition MetDef = structureViever1.SelectedNode.Tag as MethodDefinition;
			if (MetDef == null) return;

			ILProcessor cilProcess = MetDef.Body.GetILProcessor();
			//MessageBox.Show("Test",);
			MethodInfo method = typeof(MessageBox).GetMethod("Show",
			 new Type[] { typeof(string), typeof(string), typeof(MessageBoxButtons), typeof(MessageBoxIcon) });
			MethodReference method2 = AssemblyDef.MainModule.Import(method);
			Instruction instruction = cilProcess.Create(OpCodes.Ldstr, "test");
			Instruction instruction1 = cilProcess.Create(OpCodes.Ldstr, "test2");
			Instruction instruction11 = cilProcess.Create(OpCodes.Ldc_I4_0);
			Instruction instruction111 = cilProcess.Create(OpCodes.Ldc_I4_S, (sbyte)64);

			try
			{
				Instruction instruction2 = cilProcess.Create(OpCodes.Call, method2);
				Instruction instr = cilProcess.Create(OpCodes.Pop);
				ILProcessor cilWorker2 = cilProcess;
				cilWorker2.InsertBefore(MetDef.Body.Instructions[0], instruction);
				cilWorker2.InsertAfter(instruction, instruction1);
				cilWorker2.InsertAfter(instruction1, instruction11);
				cilWorker2.InsertAfter(instruction11, instruction111);
				cilWorker2.InsertAfter(instruction111, instruction2);
				cilWorker2.InsertAfter(instruction2, instr);
			}
			catch
			{
				MessageBox.Show("Error");
				return;
			}

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

		private void button3_Click(object sender, EventArgs e)
		{
			NameCompressor.Compress = false;
			XmlDocument xDoc = new XmlDocument();
			tablemgr.Save(xDoc);
			SaveToFile(xDoc, "test.ilp");
		}

		static public void SaveToFile(XmlDocument doc, string FileName)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = !NameCompressor.Compress;
			settings.IndentChars = "\t";
			settings.NewLineChars = "\r\n";
			settings.NewLineHandling = NewLineHandling.Replace;
			settings.Encoding = Encoding.UTF8;
			using (FileStream fs = new FileStream(FileName, FileMode.Create))
			{
				using (XmlWriter writer = XmlWriter.Create(fs, settings))
				{
					doc.Save(writer);
				}
			}
		}

		static public XmlDocument ReadFromFile(string FileName)
		{
			using (FileStream fs = new FileStream(FileName, FileMode.Open))
			{
				using (XmlReader xReader = XmlReader.Create(fs))
				{
					XmlDocument xDoc = new XmlDocument();
					xDoc.Load(xReader);
					return xDoc;
				}
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			EditorEntry.Instance.LoadEntry(null);
			((SwooshPanel)Parent).SwooshTo(EditorEntry.Instance);
			//new EditorEntry(this, null).Show(); TODO
		}

		public void Add(PatchEntry pe)
		{
			if (tablemgr == null)
				tablemgr = new TableManager();
			if (!tablemgr.EntryList.Contains(pe))
				tablemgr.EntryList.Add(pe);
			RebuildTable();
		}

		private void RebuildTable()
		{
			bool[] checkState = new bool[clbPatchList.Items.Count];
			for (int i = 0; i < clbPatchList.Items.Count; i++)
				checkState[i] = clbPatchList.GetItemChecked(i);
			clbPatchList.Items.Clear();
			for (int i = 0; i < tablemgr.EntryList.Count; i++)
				clbPatchList.Items.Add(tablemgr.EntryList[i].EntryName, i < checkState.Length ? checkState[i] : false);
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

			btnTestPatch.Top = Height - (btnTestPatch.Height + btnCreatePatch.Height + 2 * space);
			btnTestPatch.Left = tabInfoControl.Left;
			btnTestPatch.Width = tabInfoControl.Width / 2;

			btnCreatePatch.Top = Height - (btnCreatePatch.Height + space);
			btnCreatePatch.Left = tabInfoControl.Left;
			btnCreatePatch.Width = btnTestPatch.Width;

			btnSavePatchList.Top = btnCreatePatch.Top;
			btnSavePatchList.Left = btnCreatePatch.Right + space;
			btnSavePatchList.Width = tabInfoControl.Width / 2 - space;
		}

		private void btnLoadilp_Click(object sender, EventArgs e)
		{
			OpenFileDialog openIlp = new OpenFileDialog();
			openIlp.Filter = "ILPatch File | *.ilp";
			if (openIlp.ShowDialog() == DialogResult.OK)
			{
				ilpFiletmp = openIlp.FileName;
				if (status == AssemblyStatus.RawAssemblyLoaded || status == AssemblyStatus.AssemblyAndDataLoaded)
					LoadIlpFile(openIlp.FileName);
				else
					AwaitingAssemblySelect = true;
			}
		}

		public void LoadIlpFile(string filename)
		{
			//mLoading.ON = true;
			XmlDocument xDoc = ReadFromFile(filename);
			txtilpFile.Text = filename;
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
				ILManager.Instance.Load(BaseNode);
				tablemgr.Read(BaseNode);
			}
			else
				Log.Write(Log.Level.Error, "No PatchTable found!");
			//mLoading.ON = false;
		}

		private void btnEditPatch_Click(object sender, EventArgs e)
		{
			if (clbPatchList.SelectedIndex >= 0)
			{
				EditorEntry.Instance.LoadEntry(
					tablemgr.EntryList[clbPatchList.SelectedIndex]);
				((SwooshPanel)Parent).SwooshTo(EditorEntry.Instance);
			}
		}
	}

	public enum AssemblyStatus
	{
		Uninitialized,
		RawAssemblyLoaded,
		AssemblyAndDataLoaded,
		Operatable = RawAssemblyLoaded | AssemblyAndDataLoaded,
		LoadFailed,
	}
}
