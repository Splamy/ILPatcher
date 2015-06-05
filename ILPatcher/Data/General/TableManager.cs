using ILPatcher.Utility;
using System.Collections.Generic;
using System.Xml;
using System;

namespace ILPatcher.Data.General
{
	public class TableManager : ISaveToFile
	{
		public string filename { get; set; }
		public List<PatchCluster> ClusterList { get; private set; }

		public TableManager()
		{
			ClusterList = new List<PatchCluster>();
		}

		public void Execute()
		{
			ClusterList.ForEach(pe => pe.Execute());
		}

		public bool Save(XmlNode output)
		{
			if (output == null)
				throw new ArgumentNullException("output");

			XmlElement xPatchTableNode = output.InsertCompressedElement(SST.PatchTable);
			foreach (PatchCluster pe in ClusterList)
				pe.Save(xPatchTableNode);
			ILManager.Instance.Save(xPatchTableNode);
			ILManager.Instance.MergeDoubleElements();
			return true;
		}

		public bool Load(XmlNode input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			NameCompressor nc = NameCompressor.Instance;

			ILManager.Instance.Clear();
			ILManager.Instance.Load(input);

			foreach (XmlElement xnode in input.ChildNodes)
			{
				if (xnode.Name == nc[SST.PatchCluster])
				{
					PatchCluster tmpPE = new PatchCluster();
					tmpPE.Name = xnode.GetAttribute(SST.NAME);
					tmpPE.Load(xnode);
					Add(tmpPE);
				}
			}
			return true;
		}

		public void Add(PatchCluster pe)
		{
			if (!ClusterList.Contains(pe))
				ClusterList.Add(pe);
		}
	}
}
