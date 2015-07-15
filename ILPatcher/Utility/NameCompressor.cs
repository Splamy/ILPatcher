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
			int valcnt = Enum.GetValues(typeof(SST)).Length;
			for (int i = 0; i < valcnt; i++)
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
			}
		}

		private NameCompressor()
		{
			lowtable = new Tuple<string, string>[Enum.GetValues(typeof(SST)).Length];

			InitializeValue(SST.ID, "ID");
			InitializeValue(SST.ILPTable, "ILP");
			InitializeValue(SST.Version, "V");

			InitializeValue(SST.PatchActionTable, "PAT");
			InitializeValue(SST.TargetFinderTable, "TFT");
			InitializeValue(SST.PatchEntryTable, "PET");
			InitializeValue(SST.ReferenceTable, "RFT");

			InitializeValue(SST.PatchAction, "PA");
			InitializeValue(SST.TargetFinder, "TF");
			InitializeValue(SST.PatchEntry, "PE");
			InitializeValue(SST.MethodReference, "MR");
			InitializeValue(SST.FieldReference, "FR");
			InitializeValue(SST.TypeReference, "TR");

			InitializeValue(SST.PatchType, "PT");
			InitializeValue(SST.PatchStatus, "PS");

			InitializeValue(SST.MethodPath, "MP");

			InitializeValue(SST.PatchList, "PL");
			InitializeValue(SST.InstructionCount, "IC");

			InitializeValue(SST.Instruction, "I");
			InitializeValue(SST.InstructionPatch, "IP");
			InitializeValue(SST.InstructionNum, "N");
			InitializeValue(SST.OpCode, "O");
			InitializeValue(SST.Delete, "D");
			InitializeValue(SST.PrimitiveValue, "PV");

			InitializeValue(SST.Resolve, "RS");
			InitializeValue(SST.ResolveExtended, "RE");
			InitializeValue(SST.CallSite, "CS");
			InitializeValue(SST.BrTargetIndex, "BI");
			InitializeValue(SST.BrTargetArray, "BA");

			InitializeValue(SST.Return, "R");
			InitializeValue(SST.Module, "M");
			InitializeValue(SST.Namespace, "NS");
			InitializeValue(SST.Type, "T");
			InitializeValue(SST.Name, "NA");
			InitializeValue(SST.NestedIn, "NI");
			InitializeValue(SST.Array, "AR");
			InitializeValue(SST.Parameter, "PR");
			InitializeValue(SST.Generics, "GN");

			InitializeValue(SST.True, "t");
			InitializeValue(SST.False, "f");
		}

		private void InitializeValue(SST entry, string lowname)
		{
			lowtable[(int)entry] = new Tuple<string, string>(entry.ToString(), lowname);
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
		ID,
		ILPTable,
		Version,

		PatchActionTable,
		TargetFinderTable,
		PatchEntryTable,
		ReferenceTable,

		PatchAction,
		TargetFinder,
		PatchEntry,
		MethodReference,
		FieldReference,
		TypeReference,

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
		CallSite,
		BrTargetIndex,
		BrTargetArray,

		Return,
		Module,
		Namespace,
		Type,
		Name,
		NestedIn,
		Array,
		Parameter,
		Generics,

		True,
		False,
	}
}
