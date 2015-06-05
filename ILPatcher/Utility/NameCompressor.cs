using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ILPatcher.Utility
{
	class NameCompressor
	{
		private static NameCompressor instance;
		public static NameCompressor Instance
		{
			get
			{
				if (instance == null)
					instance = new NameCompressor();
				return instance;
			}
		}
		private Tuple<string, string>[] lowtable;
		public static bool Compress = true;

		[Conditional("DEBUG")]
		public void CheckUnique()
		{
			HashSet<string> LongName = new HashSet<string>();
			HashSet<string> CompressName = new HashSet<string>();
			for (int i = 0; i < (int)SST.SSTLISTEND; i++)
			{
				Tuple<string, string> tpl = lowtable[i];
				if (tpl == null)
				{
					Log.Write(Log.Level.Warning, "SST ID entry does not exist: ", ((SST)i).ToString());
					continue;
				}
				if (LongName.Contains(tpl.Item1))
					Log.Write(Log.Level.Warning, "LongName Element Doubled: ", tpl.Item1);
				else
					LongName.Add(tpl.Item1);
				if (CompressName.Contains(tpl.Item2))
					Log.Write(Log.Level.Warning, "CompressName Element Doubled: ", tpl.Item2);
				else
					CompressName.Add(tpl.Item2);
				if (((SST)i).ToString() != tpl.Item1)
					Log.Write(Log.Level.Warning, "SST ID does not match name: ", ((SST)i).ToString());
			}
		}

		private NameCompressor()
		{
			lowtable = new Tuple<string, string>[(int)SST.SSTLISTEND];

			lowtable[(int)SST.PatchTable] = new Tuple<string, string>("PatchTable", "PAT");
			lowtable[(int)SST.PatchCluster] = new Tuple<string, string>("PatchCluster", "PC");

			lowtable[(int)SST.PatchAction] = new Tuple<string, string>("PatchAction", "PA");
			lowtable[(int)SST.PatchType] = new Tuple<string, string>("PatchType", "PT");
			lowtable[(int)SST.PatchStatus] = new Tuple<string, string>("PatchStatus", "PS");

			lowtable[(int)SST.MethodPath] = new Tuple<string, string>("MethodPath", "MP");

			lowtable[(int)SST.PatchList] = new Tuple<string, string>("PatchList", "PL");
			lowtable[(int)SST.InstructionCount] = new Tuple<string, string>("InstructionCount", "IC");

			lowtable[(int)SST.Instruction] = new Tuple<string, string>("Instruction", "I");
			lowtable[(int)SST.InstructionPatch] = new Tuple<string, string>("InstructionPatch", "IP");
			lowtable[(int)SST.InstructionNum] = new Tuple<string, string>("InstructionNum", "N");
			lowtable[(int)SST.OpCode] = new Tuple<string, string>("OpCode", "O");
			lowtable[(int)SST.Delete] = new Tuple<string, string>("Delete", "D");
			lowtable[(int)SST.PrimitiveValue] = new Tuple<string, string>("PrimitiveValue", "PV");

			lowtable[(int)SST.Resolve] = new Tuple<string, string>("Resolve", "RS");
			lowtable[(int)SST.ResolveExtended] = new Tuple<string, string>("ResolveExtended", "RE");
			lowtable[(int)SST.MethodReference] = new Tuple<string, string>("MethodReference", "MR");
			lowtable[(int)SST.FieldReference] = new Tuple<string, string>("FieldReference", "FR");
			lowtable[(int)SST.TypeReference] = new Tuple<string, string>("TypeReference", "TR");
			lowtable[(int)SST.CallSite] = new Tuple<string, string>("CallSite", "CS");
			lowtable[(int)SST.BrTargetIndex] = new Tuple<string, string>("BrTargetIndex", "BI");
			lowtable[(int)SST.BrTargetArray] = new Tuple<string, string>("BrTargetArray", "BA");

			lowtable[(int)SST.RETURN] = new Tuple<string, string>("RETURN", "R");
			lowtable[(int)SST.MODULE] = new Tuple<string, string>("MODULE", "M");
			lowtable[(int)SST.NAMESPACE] = new Tuple<string, string>("NAMESPACE", "NS");
			lowtable[(int)SST.TYPE] = new Tuple<string, string>("TYPE", "T");
			lowtable[(int)SST.NAME] = new Tuple<string, string>("NAME", "NA");
			lowtable[(int)SST.NESTEDIN] = new Tuple<string, string>("NESTEDIN", "NI");
			lowtable[(int)SST.ARRAY] = new Tuple<string, string>("ARRAY", "AR");
			lowtable[(int)SST.PARAMETER] = new Tuple<string, string>("PARAMETER", "PR");
			lowtable[(int)SST.GENERICS] = new Tuple<string, string>("GENERICS", "GN");

			lowtable[(int)SST.True] = new Tuple<string, string>("True", "t");
			lowtable[(int)SST.False] = new Tuple<string, string>("False", "f");
		}

		public string this[SST val]
		{
			get { if (Compress) return GetValComp(val); else return GetValUnComp(val); }
		}

		public string GetValComp(SST val)
		{
#if DEBUG
			if (lowtable[(int)val] == null)
			{
				Log.Write(Log.Level.Error, "NameCompress Value not found: ", val.ToString());
				return val.ToString();
			}
#endif
			return lowtable[(int)val].Item2;
		}

		public string GetValUnComp(SST val)
		{
#if DEBUG
			if (lowtable[(int)val] == null)
			{
				Log.Write(Log.Level.Error, "NameCompress Value not found: ", val.ToString());
				return val.ToString();
			}
#endif
			return lowtable[(int)val].Item1;
		}
	}

	public enum SST : int
	{
		PatchTable,
		PatchCluster,

		PatchAction,
		PatchType,
		PatchStatus,

		MethodPath,

		PatchList,
		InstructionCount,

		Instruction,
		InstructionPatch,
		InstructionNum,
		OpCode,
		Delete,
		PrimitiveValue,

		Resolve,
		ResolveExtended,
		MethodReference,
		FieldReference,
		TypeReference,
		CallSite,
		BrTargetIndex,
		BrTargetArray,

		RETURN,
		MODULE,
		NAMESPACE,
		TYPE,
		NAME,
		NESTEDIN,
		ARRAY,
		PARAMETER,
		GENERICS,

		True,
		False,

		SSTLISTEND,
	}
}
