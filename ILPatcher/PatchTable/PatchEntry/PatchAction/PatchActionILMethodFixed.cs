using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public class PatchActionILMethodFixed : PatchAction, ISaveToFile
	{
		public override PatchActionType PatchActionType { get { return PatchActionType.ILMethodFixed; } protected set { } }
		private PatchStatus _PatchStatus = PatchStatus.Unset;
		public override PatchStatus PatchStatus { get { return _PatchStatus; } protected set { _PatchStatus = value; } }

		public MethodDefinition MethodDef;
		public List<InstructionInfo> instructPatchList;

		public PatchActionILMethodFixed()
		{
		}

		public override bool Execute()
		{
			return true;
		}

		public override bool Save(XmlNode output)
		{
			NameCompressor nc = NameCompressor.Instance;

			output.Attributes[nc[SST.PatchType]].Value = PatchActionType.ToString();
			output.Attributes[nc[SST.NAME]].Value = ActionName;

			#region PatchList
			int c = instructPatchList.Count;

			XmlElement xListPatched = output.InsertCompressedElement(SST.PatchList);
			xListPatched.CreateAttribute(SST.MethodPath, ILManager.Instance.Reference(MethodDef).ToBaseAlph());
			xListPatched.CreateAttribute(SST.InstructionCount, c.ToString());

			for (int i = 0; i < c; i++)
			{
				InstructionInfo II = instructPatchList[i];
				XmlElement xInstruction = xListPatched.InsertCompressedElement(SST.Instruction);

				if (II.OldInstructionNum != -1)
				{
					//OldInstructionNum/OriginalInstructionNum: -1 if new command
					xInstruction.CreateAttribute(SST.InstructionNum, II.OldInstructionNum.ToString());
					xInstruction.CreateAttribute(SST.OpCode, II.OldInstruction.OpCode.Name);
					xInstruction.CreateAttribute(SST.Delete, nc[II.Delete ? SST.True : SST.False]);
					Operand2Node(xInstruction, II.OldInstruction, true);

					XmlElement patchelem = null;
					if (II.OldInstructionNum != II.NewInstructionNum)
					{
						if (patchelem == null) patchelem = xInstruction.CreateCompressedElement(SST.InstructionPatch);
						patchelem.CreateAttribute(SST.InstructionNum, II.NewInstructionNum.ToString());
					}

					if (II.OldInstruction.OpCode != II.NewInstruction.OpCode)
					{
						if (patchelem == null) patchelem = xInstruction.CreateCompressedElement(SST.InstructionPatch);
						patchelem.CreateAttribute(SST.OpCode, II.NewInstruction.OpCode.Name);
					}

					if (PatchStatus != PatchStatus.Broken &&
						II.OldInstruction.Operand != II.NewInstruction.Operand &&
						II.OldInstruction.OpCode.OperandType != OperandType.InlineNone)
					{
						if (patchelem == null) patchelem = xInstruction.CreateCompressedElement(SST.InstructionPatch);
						Operand2Node(patchelem, II.NewInstruction, false);
					}

					if (patchelem != null)
						xInstruction.AppendChild(patchelem);
				}
				else
				{
					xInstruction.CreateAttribute(SST.InstructionNum, instructPatchList[i].NewInstructionNum.ToString());
					xInstruction.CreateAttribute(SST.OpCode, instructPatchList[i].NewInstruction.OpCode.Name);
					if (PatchStatus != PatchStatus.Broken)
						Operand2Node(xInstruction, II.NewInstruction, false);
				}
			}
			#endregion

			return true;
		}

		public override bool Read(XmlNode input)
		{
			NameCompressor nc = NameCompressor.Instance;

			if (input.ChildNodes.Count == 0) { Log.Write(Log.Level.Error, "Node ", input.Name, " has no Childnodes"); return false; }

			XmlElement PatchList = null;
			for (int i = 0; i < input.ChildNodes.Count; i++)
				if (input.ChildNodes[i].Name == nc[SST.PatchList])
				{
					PatchList = input.ChildNodes[i] as XmlElement;
					break;
				}
			if (PatchList == null) { Log.Write(Log.Level.Error, "No PatchList Child found"); return false; }

			string metpathunres = PatchList.GetAttribute(SST.MethodPath);
			if (metpathunres == string.Empty) { Log.Write(Log.Level.Error, "MethodPath Attribute not found or empty"); return false; }
			MethodDef = ILManager.Instance.Resolve(metpathunres.ToBaseInt()) as MethodDefinition;
			if (MethodDef == null) { Log.Write(Log.Level.Error, "MethodID <", metpathunres, "> couldn't be resolved"); return false; }

			//TODO init useful
			int instructioncount = int.Parse(PatchList.GetAttribute(SST.InstructionCount));
			InstructionInfo[] iibuffer = new InstructionInfo[instructioncount];
			bool checkopcdes = true;
			bool resolveparams = false; // resolves types/methods/...
			bool checkprimitives = true; // checks if primitive types are identical

			List<InstructionInfo> postinitderef = new List<InstructionInfo>();
			XmlElement xDummy = PatchList.CreateCompressedElement(SST.NAME);

			for (int i = 0; i < instructioncount; i++)
			{
				XmlElement xelem = PatchList.ChildNodes[i] as XmlElement;
				if (xelem == null) { Log.Write(Log.Level.Warning, "PatchList elemtent nr.", i.ToString(), " couldn't be found"); continue; }

				InstructionInfo nII = new InstructionInfo();
				XmlAttribute xdelatt = xelem.Attributes[nc[SST.Delete]];
				OpCode opcode = ILManager.OpCodeLookup[xelem.GetAttribute(SST.OpCode)];

				if (xdelatt != null) // is an old instriction
				{
					nII.OldInstructionNum = int.Parse(xelem.GetAttribute(SST.InstructionNum));
					if (nII.OldInstructionNum < MethodDef.Body.Instructions.Count)
					{
						nII.OldInstruction = MethodDef.Body.Instructions[nII.OldInstructionNum];
						if (checkopcdes && opcode != nII.OldInstruction.OpCode) { Log.Write(Log.Level.Careful, "Opcode of Instruction ", nII.OldInstructionNum.ToString(), " has changed"); }  // TODO set mismatch | from-to log
					}
					nII.Delete = xdelatt.Value == nc[SST.True];

					if (checkprimitives) // 3 cases >> Old:PV // New:PV_same | New:PV_change | New:AnyVal
					{
						XmlAttribute xprim = xelem.Attributes[nc[SST.PrimitiveValue]];
						if (xprim != null && nII.NewInstructionNum < MethodDef.Body.Instructions.Count)
						{
							Operand2Node(xDummy, MethodDef.Body.Instructions[nII.NewInstructionNum], false);

							XmlAttribute xprimcmp = xDummy.Attributes[nc[SST.PrimitiveValue]];
							nII.PrimitiveMismatch = xprimcmp == null || xprimcmp.Value != xprim.Value;

							xDummy.Attributes.RemoveAll();
						}
					}
					if (resolveparams)
					{
						Log.Write(Log.Level.Info, "Resolve not implemented yet.");
						//todo PatchStatus = PatchStatus.Broken; of not found ref
					}

					if (xelem.ChildNodes.Count == 1)
					{
						XmlElement xpatchelem = xelem.ChildNodes[0] as XmlElement;

						string instnum = xpatchelem.GetAttribute(SST.InstructionNum);
						string primval = xpatchelem.GetAttribute(SST.PrimitiveValue);
						string resolve = xpatchelem.GetAttribute(SST.Resolve);
						string patchopcode = xpatchelem.GetAttribute(SST.OpCode).Replace(".", "");

						if (instnum == string.Empty)
							nII.NewInstructionNum = nII.OldInstructionNum;
						else
							nII.NewInstructionNum = int.Parse(instnum);

						OpCode patchopc;
						if (patchopcode == string.Empty)
							patchopc = opcode;
						else
							patchopc = ILManager.OpCodeLookup[patchopcode];

						if (primval != string.Empty)
							nII.NewInstruction = ILManager.GenInstruction(patchopc, primval);
						else if (resolve != string.Empty)
							nII.NewInstruction = ILManager.GenInstruction(patchopc, ILManager.Instance.Resolve(resolve.ToBaseInt()));
						else if (nII.OldInstruction.Operand != null)
							nII.NewInstruction = nII.OldInstruction.Clone();
						else
							nII.NewInstruction = ILManager.GenInstruction(patchopc, null);
					}
					else
					{
						nII.NewInstructionNum = nII.OldInstructionNum;
						nII.NewInstruction = nII.OldInstruction.Clone();
					}
				}
				else // new nstruction
				{
					nII.OldInstructionNum = -1;
					nII.NewInstructionNum = int.Parse(xelem.GetAttribute(SST.InstructionNum));
					string primval = xelem.GetAttribute(SST.PrimitiveValue);
					string resolve = xelem.GetAttribute(SST.Resolve);
					if (primval != string.Empty)
						nII.NewInstruction = ILManager.GenInstruction(opcode, primval);
					else if (resolve != string.Empty)
					{
						nII.NewInstruction = ILManager.GenInstruction(opcode, ILManager.Instance.Resolve(resolve.ToBaseInt()));
						PatchStatus = PatchStatus.Broken;
					}
					else
						nII.NewInstruction = ILManager.GenInstruction(opcode, null);
				}

				iibuffer[i] = nII;
			}

			// TODO do postreferene

			instructPatchList = new List<InstructionInfo>(iibuffer);

			return true;
		}

		public void Operand2Node(XmlElement xParent, Instruction i, bool OldI)
		{
			OpCode oc = i.OpCode;
			object operand = i.Operand;
			NameCompressor nc = NameCompressor.Instance;

			if (oc.OperandType == OperandType.InlineNone)
				return;
			else if (oc.OperandType == OperandType.InlineI ||
					 oc.OperandType == OperandType.InlineI8 ||
					 oc.OperandType == OperandType.InlineString ||
					 oc.OperandType == OperandType.InlineR ||
					 oc.OperandType == OperandType.ShortInlineI ||
					 oc.OperandType == OperandType.ShortInlineR)
			{
				xParent.CreateAttribute(SST.PrimitiveValue, operand.ToString());
			}
			else if (oc.OperandType == OperandType.InlineArg ||
					 oc.OperandType == OperandType.ShortInlineArg ||
					 oc.OperandType == OperandType.InlineVar ||
					 oc.OperandType == OperandType.ShortInlineVar ||
					 oc.OperandType == OperandType.InlineMethod ||
					 oc.OperandType == OperandType.InlineType ||
					 oc.OperandType == OperandType.InlineTok ||
					 oc.OperandType == OperandType.InlineSig ||
					 oc.OperandType == OperandType.InlineField)
			{
				xParent.CreateAttribute(SST.Resolve, ILManager.Instance.Reference(operand).ToBaseAlph());
			}
			else if (oc.OperandType == OperandType.InlineBrTarget ||
				oc.OperandType == OperandType.ShortInlineBrTarget)
			{
				xParent.CreateAttribute(SST.BrTargetIndex, FindInstruction((Instruction)i.Operand, OldI).ToString());
			}
			else if (oc.OperandType == OperandType.InlineSwitch)
			{
				StringBuilder strb = new StringBuilder();
				Instruction[] arr = (Instruction[])operand;
				foreach (Instruction instr in arr)
				{
					strb.Append(FindInstruction(instr, OldI));
					strb.Append(' ');
				}
				xParent.CreateAttribute(SST.BrTargetArray, strb.ToString());
			}
			else
			{
				Log.Write(Log.Level.Error, "Opcode not processed: ", oc.OperandType.ToString());
			}
		}

		/*public void Opernad2NodeObsolete(XmlElement xParent, Instruction i, bool OldI)
		{
			OpCode oc = i.OpCode;
			object operand = i.Operand;
			XmlDocument xDoc = xParent.OwnerDocument;
			NameCompressor nc = NameCompressor.Instance;

			if (oc.OperandType == OperandType.InlineNone)
				return;
			else if (oc.OperandType == OperandType.InlineI ||
			oc.OperandType == OperandType.InlineI8 ||
			oc.OperandType == OperandType.InlineString ||
			oc.OperandType == OperandType.InlineR ||
			oc.OperandType == OperandType.ShortInlineI ||
			oc.OperandType == OperandType.ShortInlineR)
			{
				xParent.CreateAttribute(SST.PrimitiveValue, operand.ToString());
			}
			else if (oc.OperandType == OperandType.InlineArg ||
				oc.OperandType == OperandType.ShortInlineArg)
			{
				ParameterDefinition ParDef = (ParameterDefinition)operand;
				xParent.CreateAttribute(SST.ParameterDefinition, ILManager.Instance.Reference(ParDef).ToBaseAlph());
			}
			else if (oc.OperandType == OperandType.InlineVar ||
			   oc.OperandType == OperandType.ShortInlineVar)
			{
				VariableDefinition VarDef = (VariableDefinition)operand;
				xParent.CreateAttribute(SST.VariableDefinition, VarDef.Index.ToString());
			}
			else if (oc.OperandType == OperandType.InlineMethod)
			{
				//Type t = operand.GetType(); // MethodDefinition : MethodReference
				if (operand is MethodDefinition)
					xParent.CreateAttribute(SST.MethodDefinition, ILManager.Instance.Reference(operand).ToBaseAlph());
				else if (operand is MethodReference)
					xParent.CreateAttribute(SST.MethodReference, ILManager.Instance.Reference(operand).ToBaseAlph());
				else
					Log.Write("Operand not processed: ", operand.GetType().Name, " at <InlineMethod>");
			}
			else if (oc.OperandType == OperandType.InlineType)
			{
				xParent.CreateAttribute(SST.TypeReference, ILManager.Instance.Reference(operand).ToBaseAlph());
			}
			else if (oc.OperandType == OperandType.InlineBrTarget ||
				oc.OperandType == OperandType.ShortInlineBrTarget)
			{
				xParent.CreateAttribute(SST.BrTargetIndex, FindInstruction((Instruction)i.Operand, OldI).ToString());
			}
			else if (oc.OperandType == OperandType.InlineSwitch)
			{
				StringBuilder strb = new StringBuilder();
				Instruction[] arr = (Instruction[])operand;
				foreach (Instruction instr in arr)
				{
					strb.Append(FindInstruction(instr, OldI));
					strb.Append(' ');
				}
				xParent.CreateAttribute(SST.BrTargetArray, strb.ToString());
			}
			else if (oc.OperandType == OperandType.InlineTok)
			{
				xParent.CreateAttribute(SST.InlineTok, ILManager.Instance.Reference(operand).ToBaseAlph());
			}
			else if (oc.OperandType == OperandType.InlineSig)
			{
				xParent.CreateAttribute(SST.CallSite, ILManager.Instance.Reference(operand).ToBaseAlph());
			}
			else if (oc.OperandType == OperandType.InlineField)
			{
				if (operand is FieldDefinition)
					xParent.CreateAttribute(SST.FieldDefinition, ILManager.Instance.Reference(operand).ToBaseAlph());
				else if (operand is FieldReference)
					xParent.CreateAttribute(SST.FieldReference, ILManager.Instance.Reference(operand).ToBaseAlph());
				else
					Log.Write("Operand not processed: ", operand.GetType().Name, " at <InlineField>");
			}
			else
			{
				Log.Write("Opcode not processed: ", oc.OperandType.ToString());
			}
		}*/

		private int FindInstruction(Instruction i, bool OldI)
		{
			if (OldI)
			{
				/*for (int j = 0; j < instructPatchList.Count; j++)
					if (instructPatchList[j].OldInstruction == i)
						return instructPatchList[j].OldInstructionNum;*/
				Mono.Collections.Generic.Collection<Instruction> gencol = MethodDef.Body.Instructions;
				int gcolcnt = gencol.Count;
				for (int j = 0; j < gcolcnt; j++)
					if (gencol[j] == i)
						return j;
			}
			else
			{
				for (int j = 0; j < instructPatchList.Count; j++)
					if (instructPatchList[j].NewInstruction == i)
						return instructPatchList[j].NewInstructionNum;
			}
			Log.Write(Log.Level.Warning, "Instruction not found: ", i.Offset.ToString());
			return -1;
		}
	}

	// IDEA Maybe put them into <OpCodeTableItem>
	public class InstructionInfo
	{
		public Instruction OldInstruction;
		public Instruction NewInstruction;
		public int OldInstructionNum;
		public int NewInstructionNum;
		public bool Delete = false;

		public bool PrimitiveMismatch = false;
		public bool ReferenceMismatch = false;
		public bool OperandMismatch { get { return PrimitiveMismatch || ReferenceMismatch; } protected set { } }
	}
}
