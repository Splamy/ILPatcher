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
		public string ClusterName
		{
			get
			{
				return string.Format("{0} -> {1}",
					FindAction != null ? FindAction.ActionName : "{}",
					PatchAction != null ? PatchAction.ActionName : "{}");
			}
		}
		public PatchAction FindAction { get; set; } // TODO change to FindAction-Type
		public PatchAction PatchAction { get; set; }

		public PatchCluster()
		{
			FindAction = null;
			PatchAction = null;
		}

		public void Execute()
		{
			// TODO
			// object foundarget(s) = TargetFinder.Find();
			// foundtarget can be an array but the patchacation must know what to do with it.
			// PatchAction.ExecuteOn(foundtarget);

			/*ActionList.ForEach(pa =>
			{
				if (pa.PatchStatus == PatchStatus.WoringPerfectly && pa.Execute())
					Log.Write(Log.Level.Info, "Patch <", pa.ActionName, "> executed successfully!");
				else
					Log.Write(Log.Level.Info, "Patch <", pa.ActionName, "> is broken and won't be executed");
			});*/
		}

		public bool Save(XmlNode output)
		{
			//TODO:

			// possible with same cluster
			// <cluster>
			// <finder> ... <\finder>
			// <action> ... <\action>
			// <\cluster>


			/*XmlElement xPatchClusterNode = output.InsertCompressedElement(SST.PatchCluster);
			xPatchClusterNode.CreateAttribute(SST.NAME, ClusterName);
			foreach (PatchAction pa in ActionList)
			{
				XmlElement xPatchActionNode = xPatchClusterNode.InsertCompressedElement(SST.PatchAction); //parent

				xPatchActionNode.CreateAttribute(SST.PatchType, string.Empty);
				xPatchActionNode.CreateAttribute(SST.NAME, string.Empty);
				pa.Save(xPatchActionNode);
			}*/
			return true;
		}

		public bool Load(XmlNode input)
		{
			//TODO see save

			/*NameCompressor nc = NameCompressor.Instance;

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
					case PatchActionType.ILMethodCreator:
						Log.Write(Log.Level.Info, "ILMethodCreator not implemented");
						continue;
					default:
						continue;
					}
					pa.ActionName = xnode.GetAttribute(SST.NAME);
					pa.Load(xnode);
					ActionList.Add(pa);
				}
			}*/
			return true;
		}

		public void SetPatchAction(PatchAction pa)
		{
			PatchAction = pa;
		}

		public void SetFindAction(PatchAction fa)
		{
			FindAction = fa;
		}
	}
}
