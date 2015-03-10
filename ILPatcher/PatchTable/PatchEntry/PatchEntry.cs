using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILPatcher
{
	public class PatchEntry : ISaveToFile
	{
		public string EntryName;
		public List<PatchAction> ActionList;

		public PatchEntry()
		{
			ActionList = new List<PatchAction>();
			EntryName = string.Empty;
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
			XmlElement xPatchEntryNode = output.InsertCompressedElement(SST.PatchEntry);
			xPatchEntryNode.CreateAttribute(SST.NAME, EntryName);
			foreach (PatchAction pa in ActionList)
			{
				XmlElement xPatchActionNode = xPatchEntryNode.InsertCompressedElement(SST.PatchAction); //parent

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
					string pn = xnode.GetAttribute(SST.NAME);

					PatchActionType pat;
					if (!Enum.TryParse<PatchActionType>(pt, out pat))
					{
						Log.Write(Log.Level.Warning, "PatchType (", pt, ") not found");
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
	}
}
