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
		public string DisplayName { get { return ActionName + " : " + PatchStatus; } protected set { } }
		public abstract PatchActionType PatchActionType { get; protected set; }
		public abstract PatchStatus PatchStatus { get; protected set; }

		public abstract void Execute();
		public abstract bool Save(XmlNode output);
		public abstract bool Load(XmlNode input);
	}

	public enum PatchActionType
	{
		ILMethodFixed,
		ILMethodDynamic,
		ILDynamicScan,
		AoBRawScan,
	}

	public enum PatchStatus
	{
		Unset,
		WoringPerfectly,
		WorkingNoResolve,
		Broken,
	}
}
