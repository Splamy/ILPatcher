using ILPatcher.Data.Actions;
using ILPatcher.Data.Finder;
using ILPatcher.Utility;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Xml;

namespace ILPatcher.Data
{
	public class DataStruct : ISaveToFile // TODO: Find a better name ^^
	{
		public AssemblyDefinition AssemblyDefinition { get; protected set; }
		public string AssemblyLocation { get; protected set; }
		public AssemblyStatus AssemblyStatus { get; protected set; }

		public string ILPLocation { get; protected set; }

		public readonly List<PatchAction> PatchActionList;
		public readonly List<TargetFinder> TargetFinderList;
		public readonly TreeList<PatchEntry> PatchEntryList;
		public readonly Dictionary<string, ILNode> ModuleList;
		public readonly ILNode StructViewToolBox;
		public readonly ILManager ReferenceTable;

		public delegate void FileLoadedDelegate(object sender);
		public event FileLoadedDelegate OnILPFileLoaded;
		public event FileLoadedDelegate OnASMFileLoaded;

		public DataStruct()
		{
			PatchActionList = new List<PatchAction>();
			TargetFinderList = new List<TargetFinder>();
			PatchEntryList = new TreeList<PatchEntry>();
			ReferenceTable = new ILManager(this);
			ModuleList = new Dictionary<string, ILNode>();

			ClearASM();
		}

		public bool Save(XmlNode output)
		{
			if (output == null)
				throw new ArgumentNullException("output");

			bool allOk = true;

			XmlElement xILPTableNode = output.InsertCompressedElement(SST.ILPTable);

			XmlElement xPatchActionTable = xILPTableNode.InsertCompressedElement(SST.PatchActionTable);
			foreach (PatchAction pa in PatchActionList)
				allOk &= pa.Save(xPatchActionTable);

			XmlElement xTargetFinderTable = xILPTableNode.InsertCompressedElement(SST.TargetFinderTable);
			foreach (TargetFinder tf in TargetFinderList)
				allOk &= tf.Save(xTargetFinderTable);

			XmlElement xPatchEntryTable = xILPTableNode.InsertCompressedElement(SST.PatchEntryTable);
			allOk &= SaveEntryRecursive(xPatchEntryTable, PatchEntryList);

			XmlElement xReferenceTable = xILPTableNode.InsertCompressedElement(SST.ReferenceTable);
			allOk &= ReferenceTable.Save(xReferenceTable);

			return allOk;
		}

		private bool SaveEntryRecursive(XmlNode xnode, TreeListNode<PatchEntry> tlnode)
		{
			bool allOk = true;

			allOk = tlnode.Value.Save(xnode);
			foreach (var tlchild in tlnode.Children)
				allOk &= SaveEntryRecursive(xnode.LastChild, tlchild);

			return allOk;
		}

		public bool Load(XmlNode input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			NameCompressor nc = NameCompressor.Instance;
			bool allOk = true;
			ReferenceTable.Clear();

			// TODO: load everyting

			XmlNode xReferenceTable = input.SelectSingleNode("/" + nc[SST.ReferenceTable]);
			if (xReferenceTable != null)
			{
				allOk &= ReferenceTable.Load(xReferenceTable);
			}
			else
			{
				allOk = false;
				Log.Write(Log.Level.Warning, "No ReferenceTable found!");
			}

			return allOk;
		}

		public void OpenASM(string assemblyPath)
		{
			ClearASM();
			AssemblyLocation = assemblyPath;

			try
			{
				AssemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath);
				AssemblyStatus = AssemblyStatus.RawAssemblyLoaded;
			}
			catch
			{
				AssemblyStatus = AssemblyStatus.LoadFailed;
				return;
			}

			if (OnASMFileLoaded != null)
				OnASMFileLoaded(this);
		}

		public void OpenILP(string ilpPath)
		{
			ClearILP();
			ILPLocation = ilpPath;

			XmlDocument xDoc = XMLUtility.ReadFromFile(ilpPath);
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
				Load(BaseNode);
			}
			else
			{
				Log.Write(Log.Level.Error, "No PatchTable found!");
			}

			if (OnILPFileLoaded != null)
				OnILPFileLoaded(this);
		}

		public void ClearILP()
		{
			PatchActionList.Clear();
			TargetFinderList.Clear();
			PatchEntryList.Clear();
			ReferenceTable.Clear();
		}

		public void ClearASM()
		{
			AssemblyDefinition = null;
			AssemblyLocation = string.Empty;
			AssemblyStatus = AssemblyStatus.Uninitialized;
			ModuleList.Clear();
		}
	}

	public enum AssemblyStatus
	{
		Uninitialized,
		RawAssemblyLoaded,
		LoadFailed,
	}
}
