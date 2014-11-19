using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILPatcher
{
	public class TableManager : ISaveToFile
	{
		public string filename;
		public List<PatchEntry> EntryList;

		public TableManager()
		{
			EntryList = new List<PatchEntry>();
		}

		public bool Save(XmlNode output)
		{
			NameCompressor nc = NameCompressor.Instance;

			XmlElement xPatchTableNode = output.InsertCompressedElement(SST.PatchTable);
			//ILManager.Instance.Clear(); // TODO do ful resolve / sort out defect patches
			foreach (PatchEntry pe in EntryList)
				pe.Save(xPatchTableNode);
			ILManager.Instance.Save(xPatchTableNode);
			return true;
		}

		public bool Read(XmlNode input)
		{
			NameCompressor nc = NameCompressor.Instance;

			//ILM Resolve

			foreach (XmlElement xnode in input.ChildNodes)
			{
				if (xnode.Name == nc[SST.PatchEntry])
				{
					PatchEntry tmpPE = new PatchEntry();
					tmpPE.EntryName = xnode.GetAttribute(SST.NAME);
					tmpPE.Read(xnode);
					MainPanel.Instance.Add(tmpPE);
				}
			}
			return true;
		}
	}
}
