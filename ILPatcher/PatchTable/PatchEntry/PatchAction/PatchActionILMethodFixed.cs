﻿using System;
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
			AnyArray<Instruction> cpyBuffer = new AnyArray<Instruction>();
			foreach (InstructionInfo II in instructPatchList)
			{
				if (II.Delete) continue;
				cpyBuffer[II.NewInstructionNum] = II.NewInstruction;
			}
			Mono.Collections.Generic.Collection<Instruction> nList = MethodDef.Body.Instructions;
			nList.Clear();
			for (int i = 0; i < cpyBuffer.Length; i++)
				if (cpyBuffer[i] != null) nList.Add(cpyBuffer[i]);
			return true; // no safety checks atm
		}

		//TODO: FIX Brachnes: something is broken, offsets dont work...

		public override bool Save(XmlNode output)
		{
			NameCompressor nc = NameCompressor.Instance;

			output.Attributes[nc[SST.PatchType]].Value = PatchActionType.ToString();
			output.Attributes[nc[SST.NAME]].Value = ActionName;

			instructPatchList = instructPatchList.FindAll(x => !x.Delete || x.IsOld);

			XmlElement xListPatched = output.InsertCompressedElement(SST.PatchList);
			xListPatched.CreateAttribute(SST.MethodPath, ILManager.Instance.Reference(MethodDef).ToBaseAlph());
			xListPatched.CreateAttribute(SST.InstructionCount, MethodDef.Body.Instructions.Count.ToString());

			int instructionPos = 0;
			foreach (InstructionInfo II in instructPatchList)
			{
				XmlElement xInstruction = xListPatched.InsertCompressedElement(SST.Instruction);
				II.NewInstructionNum = instructionPos;

				if (II.IsOld)
				{
					//OldInstructionNum/OriginalInstructionNum: -1 if new command

					xInstruction.CreateAttribute(SST.InstructionNum, II.OldInstructionNum.ToString());
					xInstruction.CreateAttribute(SST.OpCode, II.OldInstruction.OpCode.Name);
					xInstruction.CreateAttribute(SST.Delete, nc[II.Delete ? SST.True : SST.False]);
					Operand2Node(xInstruction, II.OldInstruction, true);

					if (!II.Delete)
					{
						instructionPos++;

						XmlElement patchelem = null;
						if (II.InstructionNumPatch)
						{
							if (patchelem == null) patchelem = xInstruction.CreateCompressedElement(SST.InstructionPatch);
							patchelem.CreateAttribute(SST.InstructionNum, II.NewInstructionNum.ToString());
						}

						if (II.InstructionOpCodePatch)
						{
							if (patchelem == null) patchelem = xInstruction.CreateCompressedElement(SST.InstructionPatch);
							patchelem.CreateAttribute(SST.OpCode, II.NewInstruction.OpCode.Name);
						}

						if (II.InstructionOperandPatch)
						{
							if (patchelem == null) patchelem = xInstruction.CreateCompressedElement(SST.InstructionPatch);
							Operand2Node(patchelem, II.NewInstruction, false);
						}

						if (patchelem != null)
							xInstruction.AppendChild(patchelem);
					}
				}
				else
				{
					xInstruction.CreateAttribute(SST.InstructionNum, II.NewInstructionNum.ToString());
					xInstruction.CreateAttribute(SST.OpCode, II.NewInstruction.OpCode.Name);
					if (PatchStatus != PatchStatus.Broken) // maby safer if == PatchStatus.WoringPerfectly 
						Operand2Node(xInstruction, II.NewInstruction, false);
					instructionPos++;
				}
			}

			return true;
		}

		public override bool Load(XmlNode input)
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
			if (PatchList == null) { Log.Write(Log.Level.Error, "No PatchList Child found"); PatchStatus = PatchStatus.Broken; return false; }

			string metpathunres = PatchList.GetAttribute(SST.MethodPath);
			if (metpathunres == string.Empty) { Log.Write(Log.Level.Error, "MethodPath Attribute not found or empty"); PatchStatus = PatchStatus.Broken; return false; }
			MethodDef = ILManager.Instance.Resolve(metpathunres.ToBaseInt()) as MethodDefinition;
			if (MethodDef == null) { Log.Write(Log.Level.Error, "MethodID <", metpathunres, "> couldn't be resolved"); PatchStatus = PatchStatus.Broken; return false; }
			EditorILPattern.Instance.txtMethodFullName.Text = MethodDef.FullName;

			int instructioncount = int.Parse(PatchList.GetAttribute(SST.InstructionCount));
			InstructionInfo[] iibuffer = new InstructionInfo[instructioncount];

			if (MethodDef.Body.Instructions.Count != instructioncount)
			{
				// new method body has changed -> patching the new assembly will not work
				Log.Write(Log.Level.Error, "The patch script cannot be applied to the changend method"); PatchStatus = PatchStatus.Broken; return false;
			}

			//TODO init with given params, instead of static
			bool checkopcdes = true;
			bool resolveparams = false; // resolves types/methods/...
			bool checkprimitives = true; // checks if primitive types are identical

			List<PostInitData> postinitbrs = new List<PostInitData>();
			Instruction iDummy = Instruction.Create(OpCodes.Nop);
			XmlElement xDummy = PatchList.CreateCompressedElement(SST.NAME);

			for (int i = 0; i < instructioncount; i++)
			{
				XmlElement xelem = PatchList.ChildNodes[i] as XmlElement;
				if (xelem == null) { Log.Write(Log.Level.Warning, "PatchList elemtent nr.", i.ToString(), " couldn't be found"); PatchStatus = PatchStatus.Broken; continue; }

				InstructionInfo nII = new InstructionInfo();
				XmlAttribute xdelatt = xelem.Attributes[nc[SST.Delete]];
				OpCode opcode = ILManager.OpCodeLookup[xelem.GetAttribute(SST.OpCode)];

				if (xdelatt != null)
				#region Old_Instruction
				{
					nII.OldInstructionNum = int.Parse(xelem.GetAttribute(SST.InstructionNum));
					if (nII.OldInstructionNum < MethodDef.Body.Instructions.Count)
					{
						nII.OldInstruction = MethodDef.Body.Instructions[nII.OldInstructionNum];
						if (checkopcdes && opcode != nII.OldInstruction.OpCode)
						{
							PatchStatus = PatchStatus.Broken;
							Log.Write(Log.Level.Careful, "Opcode of Instruction ", nII.OldInstructionNum.ToString(), " has changed");
							nII.OpCodeMismatch = true;
						}  // TODO set mismatch | from-to log
					}
					nII.Delete = xdelatt.Value == nc[SST.True];

					if (checkprimitives) // 3 cases >> Old:PV // New:PV_same | New:PV_change | New:AnyVal
					{
						XmlAttribute xprim = xelem.Attributes[nc[SST.PrimitiveValue]];
						if (xprim != null && nII.OldInstructionNum < MethodDef.Body.Instructions.Count)
						{
							Operand2Node(xDummy, MethodDef.Body.Instructions[nII.OldInstructionNum], false);

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
						string patchopcode = xpatchelem.GetAttribute(SST.OpCode);

						if (instnum == string.Empty)
							nII.NewInstructionNum = nII.OldInstructionNum;
						else
							nII.NewInstructionNum = int.Parse(instnum);

						OpCode patchopc;
						if (patchopcode == string.Empty)
							patchopc = opcode;
						else
							patchopc = ILManager.OpCodeLookup[patchopcode];

						string operandvalue;
						if (xpatchelem.GetAttribute(SST.PrimitiveValue, out operandvalue))
							nII.NewInstruction = ILManager.GenInstruction(patchopc, operandvalue);
						else if ((operandvalue = xpatchelem.GetAttribute(SST.Resolve)) != string.Empty)
							nII.NewInstruction = ILManager.GenInstruction(patchopc, ILManager.Instance.Resolve(operandvalue.ToBaseInt()));
						else if ((operandvalue = xpatchelem.GetAttribute(SST.BrTargetIndex)) != string.Empty)
						{
							nII.NewInstruction = ILManager.GenInstruction(patchopc, iDummy);
							PostInitData pid = new PostInitData();
							pid.InstructionNum = nII.OldInstructionNum;
							pid.isArray = false;
							pid.targetNum = int.Parse(operandvalue);
							postinitbrs.Add(pid);
						}
						else if (xpatchelem.GetAttribute(SST.BrTargetArray, out operandvalue))
						{
							nII.NewInstruction = ILManager.GenInstruction(patchopc, new[] { iDummy });
							PostInitData pid = new PostInitData();
							pid.InstructionNum = nII.OldInstructionNum;
							pid.isArray = true;
							pid.targetArray = Array.ConvertAll<string, int>(operandvalue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
							postinitbrs.Add(pid);
						}
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
				#endregion
				else
				#region New_Instruction
				{
					nII.OldInstructionNum = -1;
					nII.NewInstructionNum = int.Parse(xelem.GetAttribute(SST.InstructionNum));

					string operandvalue;
					if (xelem.GetAttribute(SST.PrimitiveValue, out operandvalue))
						nII.NewInstruction = ILManager.GenInstruction(opcode, operandvalue);
					else if ((operandvalue = xelem.GetAttribute(SST.Resolve)) != string.Empty)
					{
						nII.NewInstruction = ILManager.GenInstruction(opcode, ILManager.Instance.Resolve(operandvalue.ToBaseInt()));
						PatchStatus = PatchStatus.Broken; // TODO temporary, when resolving works -> do properly
					}
					else if ((operandvalue = xelem.GetAttribute(SST.BrTargetIndex)) != string.Empty)
					{
						nII.NewInstruction = nII.NewInstruction = ILManager.GenInstruction(opcode, iDummy);
						PostInitData pid = new PostInitData();
						pid.InstructionNum = nII.NewInstructionNum;
						pid.isArray = false;
						pid.targetNum = int.Parse(operandvalue);
						postinitbrs.Add(pid);
					}
					else if (xelem.GetAttribute(SST.BrTargetArray, out operandvalue))
					{
						nII.NewInstruction = nII.NewInstruction = ILManager.GenInstruction(opcode, new[] { iDummy });
						PostInitData pid = new PostInitData();
						pid.InstructionNum = nII.NewInstructionNum;
						pid.isArray = true;
						pid.targetArray = Array.ConvertAll<string, int>(operandvalue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
						postinitbrs.Add(pid);
					}
					else
						nII.NewInstruction = ILManager.GenInstruction(opcode, null);
				}
				#endregion

				iibuffer[i] = nII;
			}

			foreach (PostInitData pid in postinitbrs)
			{
				if (pid.isArray)
					iibuffer[pid.InstructionNum].NewInstruction.Operand = Array.ConvertAll<int, Instruction>(pid.targetArray, a => iibuffer.First(x => x.NewInstructionNum == a).NewInstruction); // TODO: Check correct (but seems to be ok)
				else
					iibuffer[pid.InstructionNum].NewInstruction.Operand = iibuffer.First(x => x.NewInstructionNum == pid.targetNum).NewInstruction;
			}

			instructPatchList = new List<InstructionInfo>(iibuffer);

			if (PatchStatus == PatchStatus.Unset)
				PatchStatus = PatchStatus.WoringPerfectly;

			return true;
		}

		public void Operand2Node(XmlElement xParent, Instruction i, bool OldI)
		{
			OpCode oc = i.OpCode;
			object operand = i.Operand;
			NameCompressor nc = NameCompressor.Instance;
			StringBuilder strb;

			switch (oc.OperandType)
			{
			case OperandType.InlineNone:
				return;

			case OperandType.InlineI:
			case OperandType.InlineI8:
			case OperandType.InlineR:
			case OperandType.InlineString:
			case OperandType.ShortInlineI:
			case OperandType.ShortInlineR:
				xParent.CreateAttribute(SST.PrimitiveValue, operand.ToString());
				break;

			case OperandType.InlineField:
			case OperandType.InlineMethod:
			case OperandType.InlineTok:
			case OperandType.InlineType:
				xParent.CreateAttribute(SST.Resolve, ILManager.Instance.Reference(operand).ToBaseAlph());
				break;

			case OperandType.InlineArg:
			case OperandType.ShortInlineArg:
				ParameterReference parref = ((ParameterReference)operand); // TODO: check this
				strb = new StringBuilder();
				strb.Append(parref.Index.ToString());
				strb.Append(' ');
				strb.Append(ILManager.Instance.Reference(parref.ParameterType).ToBaseAlph());
				xParent.CreateAttribute(SST.Resolve, strb.ToString());
				break;

			case OperandType.InlineVar:
			case OperandType.ShortInlineVar:
				VariableReference varref = ((VariableReference)operand); // TODO: check this
				strb = new StringBuilder();
				strb.Append(varref.Index.ToString());
				strb.Append(' ');
				strb.Append(ILManager.Instance.Reference(varref.VariableType).ToBaseAlph());
				xParent.CreateAttribute(SST.Resolve, strb.ToString());
				break;

			case OperandType.InlineBrTarget:
			case OperandType.ShortInlineBrTarget:
				xParent.CreateAttribute(SST.BrTargetIndex, FindInstruction((Instruction)i.Operand, OldI).ToString());
				break;

			case OperandType.InlineSwitch:
				strb = new StringBuilder();
				Instruction[] arr = (Instruction[])operand;
				foreach (Instruction instr in arr)
				{
					strb.Append(FindInstruction(instr, OldI));
					strb.Append(' ');
				}
				xParent.CreateAttribute(SST.BrTargetArray, strb.ToString());
				break;

			case OperandType.InlineSig:
			case OperandType.InlinePhi:
			default:
				Log.Write(Log.Level.Error, "Opcode not processed: ", oc.OperandType.ToString());
				break;
			}
		}

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

		public void SetInitWorking()
		{
			_PatchStatus = PatchStatus.WoringPerfectly;
		}

		private class PostInitData
		{
			public int InstructionNum;
			public bool isArray;
			public int targetNum;
			public int[] targetArray;
		}
	}

	// IDEA Maybe put them into <OpCodeTableItem>
	public class InstructionInfo
	{
		public Instruction OldInstruction;
		public Instruction NewInstruction;
		public int OldInstructionNum;
		public int NewInstructionNum;

		//PatchInfo
		public bool Delete = false;
		public bool InstructionOperandPatch
		{
			get
			{
				if (IsNew) return false;
				OperandType oot = OldInstruction.OpCode.OperandType;
				OperandType not = NewInstruction.OpCode.OperandType;
				if (oot == OperandType.InlineNone && not != OperandType.InlineNone) return true;
				if (oot == OperandType.InlineBrTarget && not == OperandType.InlineBrTarget ||
					oot == OperandType.ShortInlineBrTarget && not == OperandType.ShortInlineBrTarget)
				{
					Instruction oop = OldInstruction.Operand as Instruction;
					Instruction nop = NewInstruction.Operand as Instruction;
					if (oop == null || nop == null)
					{
						Log.Write(Log.Level.Warning, "Operand info wrong: ", NewInstruction.ToString());
						return true;
					}
					return true; // TODO check if br targets differ || same for brarray (witch)
				}
				if (oot == OperandType.InlineSwitch && not == OperandType.InlineSwitch)
					return true;

				if (OldInstruction.Operand == null && NewInstruction.Operand == null) return false;
				if (OldInstruction.Operand == null && NewInstruction.Operand != null) return true;
				return !OldInstruction.Operand.Equals(NewInstruction.Operand);
			}
			protected set { }
		}
		public bool InstructionOpCodePatch { get { return OldInstructionNum != -1 && OldInstruction.OpCode != NewInstruction.OpCode; } protected set { } }
		public bool InstructionNumPatch { get { return OldInstructionNum != -1 && OldInstructionNum != NewInstructionNum; } protected set { } }
		public bool IsNew { get { return OldInstructionNum == -1; } protected set { } }
		public bool IsOld { get { return OldInstructionNum != -1; } protected set { } }

		//LoadInfo
		public bool OpCodeMismatch = false;
		public bool PrimitiveMismatch = false;
		public bool ReferenceMismatch = false;
		public bool OperandMismatch { get { return PrimitiveMismatch || ReferenceMismatch; } protected set { } }
	}
}
