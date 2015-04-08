using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILPatcher
{
	public class PatchCluster : ISaveToFile
	{
		public string ClusterName;
		public List<PatchAction> ActionList;

		public PatchCluster()
		{
			ActionList = new List<PatchAction>();
			ClusterName = string.Empty;
		}

		public void Execute()
		{
			ActionList.ForEach(pa =>
			{
				if (pa.PatchStatus == PatchStatus.WoringPerfectly && pa.Execute())
					Log.Write(Log.Level.Info, "Patch <", pa.ActionName, "> executed successfully!");
				else
					Log.Write(Log.Level.Info, "Patch <", pa.ActionName, "> is broken and won't be executed");
			});
		}

		public bool Save(XmlNode output)
		{
			XmlElement xPatchClusterNode = output.InsertCompressedElement(SST.PatchCluster);
			xPatchClusterNode.CreateAttribute(SST.NAME, ClusterName);
			foreach (PatchAction pa in ActionList)
			{
				XmlElement xPatchActionNode = xPatchClusterNode.InsertCompressedElement(SST.PatchAction); //parent

				xPatchActionNode.CreateAttribute(SST.PatchType, string.Empty);
				xPatchActionNode.CreateAttribute(SST.NAME, string.Empty);
				pa.Save(xPatchActionNode);
			}
			return true;
		}

		public bool Load(XmlNode input)
		{
			NameCompressor nc = NameCompressor.Instance;

			foreach (XmlElement xnode in input.ChildNodes)
			{
				if (xnode.Name == nc[SST.PatchAction])
				{
					string pt = xnode.GetAttribute(SST.PatchType);
					PatchActionType pat;
					if (!Enum.TryParse<PatchActionType>(pt, out pat))
					{
						string pn = xnode.GetAttribute(SST.NAME);
						Log.Write(Log.Level.Warning, "PatchType \"", pt, "\" couldn't be found");
						continue;
					}
					PatchAction pa = null;
					switch (pat)
					{
					case PatchActionType.ILMethodFixed:
						pa = new PatchActionILMethodFixed();
						break;
					case PatchActionType.ILMethodDynamic:
						Log.Write(Log.Level.Info, "ILMethodDynamic not implemented");
						continue;
					case PatchActionType.ILDynamicScan:
						Log.Write(Log.Level.Info, "ILDynamicScan not implemented");
						continue;
					case PatchActionType.AoBRawScan:
						Log.Write(Log.Level.Info, "AoBRawScan not implemented");
						continue;
					default:
						continue;
					}
					pa.ActionName = xnode.GetAttribute(SST.NAME);
					pa.Load(xnode);
					ActionList.Add(pa);
				}
			}
			return true;
		}

		public void Add(PatchAction pa)
		{
			if (!ActionList.Contains(pa))
				ActionList.Add(pa);
		}
	}
}
