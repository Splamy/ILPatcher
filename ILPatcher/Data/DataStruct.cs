using ILPatcher.Data.Actions;
using ILPatcher.Data.Finder;
using ILPatcher.Utility;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Xml;

namespace ILPatcher.Data
{
	public sealed class DataStruct : ISaveToFile // TODO: Find a better name ^^
	{
		public AssemblyDefinition AssemblyDefinition { get; private set; }
		public string AssemblyLocation { get; private set; }
		public AssemblyStatus AssemblyStatus { get; private set; }

		public string ILPLocation { get; private set; }

		public IList<PatchAction> PatchActionList { get; }
		public IList<TargetFinder> TargetFinderList { get; }
		public IList<PatchEntry> PatchEntryList { get; }
		public ILNodeManager ILNodeManager { get; }
		public ILManager ReferenceTable { get; }
		public EntryFactory EntryFactory { get; }

		public delegate void FileLoadedDelegate(object sender);
		public event FileLoadedDelegate OnILPFileLoaded;
		public event FileLoadedDelegate OnASMFileLoaded;

		public DataStruct()
		{
			PatchActionList = new List<PatchAction>();
			TargetFinderList = new List<TargetFinder>();
			PatchEntryList = new List<PatchEntry>();
			ReferenceTable = new ILManager(this);
			ILNodeManager = new ILNodeManager(this);
			EntryFactory = new EntryFactory(this);

			ClearASM();
		}

		public bool Save(XmlNode output)
		{
			if (output == null)
				throw new ArgumentNullException(nameof(output));

			int idNum = 0;
			bool allOk = true;

			XmlNode xILPTableNode = output.InsertCompressedElement(SST.ILPTable);

			XmlNode xPatchActionTable = xILPTableNode.InsertCompressedElement(SST.PatchActionTable);
			foreach (PatchAction pa in PatchActionList)
				allOk &= Save(xPatchActionTable, pa, (idNum++).ToBaseAlph());

			XmlNode xTargetFinderTable = xILPTableNode.InsertCompressedElement(SST.TargetFinderTable);
			foreach (TargetFinder tf in TargetFinderList)
				allOk &= Save(xTargetFinderTable, tf, (idNum++).ToBaseAlph());
			
			XmlNode xPatchEntryTable = xILPTableNode.InsertCompressedElement(SST.PatchEntryTable);
			foreach (PatchEntry pe in PatchEntryList)
				allOk &= Save(xPatchEntryTable, pe);

			XmlNode xReferenceTable = xILPTableNode.InsertCompressedElement(SST.ReferenceTable);
			allOk &= ReferenceTable.Save(xReferenceTable);

			return allOk;
		}

		private static bool Save(XmlNode xGroupNode, PatchAction patchAction, string ID)
		{
			patchAction.Id = "PA_" + ID;
			XmlNode xPatchAction = xGroupNode.InsertCompressedElement(SST.PatchAction);
			xPatchAction.CreateAttribute(SST.PatchType, patchAction.PatchActionType.ToString());
			xPatchAction.CreateAttribute(SST.ID, patchAction.Id);
			xPatchAction.CreateAttribute(SST.Name, patchAction.Name);
			return patchAction.Save(xPatchAction);
		}

		private static bool Save(XmlNode xGroupNode, TargetFinder targetFinder, string ID)
		{
			targetFinder.Id = "TF_" + ID;
			XmlNode xPatchAction = xGroupNode.InsertCompressedElement(SST.TargetFinder);
			xPatchAction.CreateAttribute(SST.PatchType, targetFinder.TargetFinderType.ToString());
			xPatchAction.CreateAttribute(SST.ID, targetFinder.Id);
			xPatchAction.CreateAttribute(SST.Name, targetFinder.Name);
			return targetFinder.Save(xPatchAction);
		}

		private static bool Save(XmlNode xGroupNode, PatchEntry patchEntry)
		{
			XmlNode xPatchAction = xGroupNode.InsertCompressedElement(SST.PatchEntry);
			xPatchAction.CreateAttribute(SST.Name, patchEntry.Name);
			return patchEntry.Save(xPatchAction);
		}

		public bool Load(XmlNode input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

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
			catch // TODO: Error description
			{
				AssemblyStatus = AssemblyStatus.LoadFailed;
				return;
			}

			OnASMFileLoaded?.Invoke(this);
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

			OnILPFileLoaded?.Invoke(this);
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
			ILNodeManager.Clear();
		}
	}

	public enum AssemblyStatus
	{
		Uninitialized,
		RawAssemblyLoaded,
		LoadFailed,
	}
}
