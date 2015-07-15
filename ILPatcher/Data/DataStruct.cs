using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ILPatcher.Data.Actions;
using ILPatcher.Data.Finder;
using ILPatcher.Utility;
using System.Xml;

namespace ILPatcher.Data
{
	public class DataStruct : ISaveToFile // TODO: Find a better name ^^
	{
		public readonly List<PatchAction> PatchActionList;
		public readonly List<TargetFinder> TargetFinderList;
		public readonly TreeList<PatchEntry> PatchEntryList;
		public readonly ISaveToFile ReferenceTable;

		public DataStruct()
		{
			PatchActionList = new List<PatchAction>();
			TargetFinderList = new List<TargetFinder>();
			PatchEntryList = new TreeList<PatchEntry>();
			ReferenceTable = ILManager.Instance;
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

			ILManager.Instance.MergeDoubleElements(); // TODO: move to save

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
			ILManager.Instance.Clear();

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
	
		public void Clear()
		{
			// TODO: Implement
		}
	}
}
