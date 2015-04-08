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
		public List<PatchCluster> ClusterList;

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
			NameCompressor nc = NameCompressor.Instance;

			XmlElement xPatchTableNode = output.InsertCompressedElement(SST.PatchTable);
			foreach (PatchCluster pe in ClusterList)
				pe.Save(xPatchTableNode);
			ILManager.Instance.Save(xPatchTableNode);
			ILManager.Instance.MergeDoubleElements();
			return true;
		}

		public bool Load(XmlNode input)
		{
			NameCompressor nc = NameCompressor.Instance;

			ILManager.Instance.Clear();
			ILManager.Instance.Load(input);

			foreach (XmlElement xnode in input.ChildNodes)
			{
				if (xnode.Name == nc[SST.PatchCluster])
				{
					PatchCluster tmpPE = new PatchCluster();
					tmpPE.ClusterName = xnode.GetAttribute(SST.NAME);
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
