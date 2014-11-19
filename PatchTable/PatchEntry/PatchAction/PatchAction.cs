using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public abstract class PatchAction : ISaveToFile
	{
		public string ActionName;

		public abstract bool Execute();
		public abstract PatchActionType GetPatchActionType();
		public abstract bool Save(XmlNode output);
		public abstract bool Read(XmlNode input);
	}

	public enum PatchActionType
	{
		ILMethodFixed,
		ILMethodDynamic,
		ILDynamicScan,
		AoBRawScan,
	}
}
