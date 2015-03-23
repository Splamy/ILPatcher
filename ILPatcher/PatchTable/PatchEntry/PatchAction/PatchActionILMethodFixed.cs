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
		private int OriginalInstructionCount;
		public List<InstructionInfo> instructPatchList;

		public PatchActionILMethodFixed()
		{
		}

		/// <summary>Executes its patch-routine to the currently loaded Assembly data</summary>
		/// <returns>Returns true if it succeeded, false otherwise</returns>
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

		/// <summary>Saves the patch data to an XmlNode</summary>
		/// <param name="output">The node where the Instructions will be added as childnodes</param>
		/// <returns>Returns true if it succeeded, false otherwise</returns>
		public override bool Save(XmlNode output)
		{
			if (MethodDef == null)
			{
				Log.Write(Log.Level.Careful, "The PatchAction(ILMethodFixed) ", ActionName, " is empty and won't be saved!");
				return false;
			}

			NameCompressor nc = NameCompressor.Instance;

			output.Attributes[nc[SST.PatchType]].Value = PatchActionType.ToString();
			output.Attributes[nc[SST.NAME]].Value = ActionName;

			instructPatchList = instructPatchList.FindAll(x => !x.Delete || x.IsOld);

			XmlElement xListPatched = output.InsertCompressedElement(SST.PatchList);
			xListPatched.CreateAttribute(SST.MethodPath, ILManager.Instance.Reference(MethodDef).ToBaseAlph());
			xListPatched.CreateAttribute(SST.InstructionCount, OriginalInstructionCount.ToString());

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

		/// <summary>Loads the patch data from an XmlNode</summary>
		/// <param name="input">The XmlNode containing the Instruction nodes</param>
		/// <returns>Returns true if it succeeded, false otherwise</returns>
		public override bool Load(XmlNode input)
		{
			NameCompressor nc = NameCompressor.Instance;

			if (input.ChildNodes.Count == 0) { Log.Write(Log.Level.Careful, "Node ", input.Name, " has no Childnodes"); PatchStatus = PatchStatus.Broken; return false; }

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

			OriginalInstructionCount = int.Parse(PatchList.GetAttribute(SST.InstructionCount));
			if (MethodDef.Body.Instructions.Count != OriginalInstructionCount)
			{
				// new method body has changed -> patching the new assembly will not work
				Log.Write(Log.Level.Error, "The PatchAction \"", ActionName, "\" cannot be applied to a changend method"); PatchStatus = PatchStatus.Broken; return false;
			}

			// TODO: init with given params, instead of static
			AnyArray<InstructionInfo> iibuffer = new AnyArray<InstructionInfo>(OriginalInstructionCount);
			bool checkopcdes = true;
			bool resolveparams = false; // resolves types/methods/...
			bool checkprimitives = true; // checks if primitive types are identical

			List<PostInitData> postinitbrs = new List<PostInitData>();
			XmlElement xDummy = PatchList.CreateCompressedElement(SST.NAME);

			#region Load all InstructionInfo
			foreach (XmlElement xelem in PatchList.ChildNodes)
			{
				if (xelem.Name != nc[SST.Instruction]) { Log.Write(Log.Level.Warning, "PatchList elemtent \"", xelem.Name, "\" is not recognized"); PatchStatus = PatchStatus.Broken; continue; }

				InstructionInfo nII = new InstructionInfo();
				XmlAttribute xdelatt = xelem.Attributes[nc[SST.Delete]];
				OpCode opcode = ILManager.OpCodeLookup[xelem.GetAttribute(SST.OpCode)];

				if (xdelatt != null) // Old_Instruction
				{
					nII.Delete = xdelatt.Value == nc[SST.True];

					//Load old Instruction into InstructionInfo
					nII.OldInstructionNum = int.Parse(xelem.GetAttribute(SST.InstructionNum));
					if (nII.OldInstructionNum < OriginalInstructionCount)
					{
						nII.OldInstruction = MethodDef.Body.Instructions[nII.OldInstructionNum];
						if (checkopcdes && opcode != nII.OldInstruction.OpCode)
						{
							PatchStatus = PatchStatus.Broken;
							Log.Write(Log.Level.Careful, "Opcode of Instruction ", nII.OldInstructionNum.ToString(), " has changed");
							nII.OpCodeMismatch = true;
						}  // TODO: set mismatch | from-to log
					}
					else continue;

					if (checkprimitives)
					{
						if (opcode.OperandType == nII.OldInstruction.OpCode.OperandType)
						{
							string oldPrimOperand;
							if (nII.OldInstruction.Operand == null)
								nII.PrimitiveMismatch = xelem.GetAttribute(SST.PrimitiveValue, out oldPrimOperand);
							else
								nII.PrimitiveMismatch = xelem.GetAttribute(SST.PrimitiveValue) != nII.OldInstruction.Operand.ToString();
						}
						else nII.PrimitiveMismatch = true;
					}

					if (resolveparams)
					{
						Log.Write(Log.Level.Info, "Resolve not implemented yet.");
						//TODO: PatchStatus = PatchStatus.Broken; of not found ref implement, when DeepTypeCompare works
					}

					if (xelem.ChildNodes.Count == 1) // check if patchnode exists
					{
						XmlElement xpatchelem = xelem.ChildNodes[0] as XmlElement;

						string instnum = xpatchelem.GetAttribute(SST.InstructionNum);
						if (instnum == string.Empty) // check InstructionNum Patch
							nII.NewInstructionNum = nII.OldInstructionNum;
						else
							nII.NewInstructionNum = int.Parse(instnum);

						OpCode patchopc;
						string patchopcode = xpatchelem.GetAttribute(SST.OpCode);
						if (patchopcode == string.Empty) // check Opcode patch
							patchopc = opcode;
						else
							patchopc = ILManager.OpCodeLookup[patchopcode];

						nII.NewInstruction = Node2Instruction(xpatchelem, patchopc, nII.NewInstructionNum, postinitbrs);
						if (nII.NewInstruction == null)
							nII.NewInstruction = ILManager.GenInstruction(patchopc, nII.OldInstruction.Operand);
					}
					else // no patchnode -> clone old one
					{
						nII.NewInstructionNum = nII.OldInstructionNum;
						nII.NewInstruction = nII.OldInstruction.Clone();
					}
				}
				else // New_Instruction
				{
					nII.OldInstructionNum = -1;
					nII.NewInstructionNum = int.Parse(xelem.GetAttribute(SST.InstructionNum));
					nII.NewInstruction = Node2Instruction(xelem, opcode, nII.NewInstructionNum, postinitbrs);
					if (nII.NewInstruction == null)
					{
						PatchStatus = PatchStatus.Broken;
						Log.Write(Log.Level.Error, "Expected Operand for '", opcode.Name, "', but no matching Attribute was found in ", xelem.InnerXml);
						nII.NewInstruction = Instruction.Create(OpCodes.Nop);
						nII.NewInstruction.OpCode = opcode;
						nII.NewInstruction.Operand = "!!!!! Dummy !!!!!";
					}
				}

				iibuffer[nII.NewInstructionNum] = nII;
			}
			#endregion

			instructPatchList = new List<InstructionInfo>(iibuffer.ToArray());

			#region Check for not loaded Instructions
			if (!instructPatchList.All(x => x != null))
			{
				Log.Write(Log.Level.Error, "PatchList has holes: ", string.Join<int>(", ", instructPatchList.Select((b, i) => b == null ? i : -1).Where(i => i != -1).ToArray()));
				PatchStatus = PatchStatus.Broken;
				return false;
			}
			#endregion

			#region Set all jump operands from PostInitData
			foreach (PostInitData pid in postinitbrs)
			{
				if (pid.isArray)
				{
					bool success = true;
					instructPatchList[pid.InstructionNum].NewInstruction.Operand = Array.ConvertAll<int, Instruction>(pid.targetArray, a =>
					{
						InstructionInfo pidinstr = instructPatchList.First(x => x.NewInstructionNum == a);
						if (pidinstr == null) { success = false; Log.Write(Log.Level.Error, "PID_At: ", a.ToString()); return null; }
						return pidinstr.NewInstruction;
					});
					if (!success) { Log.Write(Log.Level.Error, "PostInitData failed: ", pid.ToString()); PatchStatus = PatchStatus.Broken; continue; }
				}
				else
				{
					InstructionInfo pidinstr = instructPatchList.First(x => x.NewInstructionNum == pid.targetNum);
					if (pidinstr == null) { Log.Write(Log.Level.Error, "PostInitData failed: ", pid.ToString()); PatchStatus = PatchStatus.Broken; continue; }
					instructPatchList[pid.InstructionNum].NewInstruction.Operand = pidinstr.NewInstruction;
				}
			}
			#endregion

			if (PatchStatus == PatchStatus.Unset)
				PatchStatus = PatchStatus.WoringPerfectly;

			return true;
		}

		/// <summary>Saves an Operand from an Instruction to a Instruction XmlNode</summary>
		/// <param name="xParent">The XmlNode where the new Instruction XmlNode will be added as child node</param>
		/// <param name="i">The Instruction with the Operand to be saved</param>
		/// <param name="OldI">If the Instruction is a branch, true will look for the target in the old Instruction list, false in the new one</param>
		private void Operand2Node(XmlElement xParent, Instruction i, bool OldI)
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
				Log.Write(Log.Level.Error, "Opcode not processed: ", oc.Name);
				break;
			}
		}

		/// <summary>Reads an XmlNode and creates an Instruction from the data</summary>
		/// <param name="xOperandNode">The XmlElement containing the operand info, created by Operand2Node</param>
		/// <param name="opcode">The OpCode the new Instruction should have, independently of the XmlElement</param>
		/// <param name="nInstructionNum">The NewInstructionNum the Instruction will have after the patch (InstructionInfo.NewInstructionNum)</param>
		/// <param name="postinitbrs">A reference to the PostInitData list, in order to load branch Instructions correctly</param>
		/// <returns>Returns the new Instruction if succeeded, null otherwise</returns>
		private Instruction Node2Instruction(XmlElement xOperandNode, OpCode opcode, int nInstructionNum, List<PostInitData> postinitbrs)
		{
			NameCompressor nc = NameCompressor.Instance;

			switch (opcode.OperandType)
			{
			case OperandType.InlineNone:
				return ILManager.GenInstruction(opcode, null);

			case OperandType.InlineI:
			case OperandType.InlineI8:
			case OperandType.InlineR:
			case OperandType.InlineString:
			case OperandType.ShortInlineI:
			case OperandType.ShortInlineR:
				string primVal;
				if (xOperandNode.GetAttribute(SST.PrimitiveValue, out primVal))
					return ILManager.GenInstruction(opcode, primVal);
				else
					Log.Write(Log.Level.Error, "Expected 'PrimitiveValue' with '", opcode.Name, "', but no matching Attribute was found in ", xOperandNode.InnerXml);
				break;

			case OperandType.InlineField:
			case OperandType.InlineMethod:
			case OperandType.InlineTok:
			case OperandType.InlineType:
				string opVal = xOperandNode.GetAttribute(SST.Resolve);
				if (opVal != string.Empty)
					return ILManager.GenInstruction(opcode, ILManager.Instance.Resolve(opVal.ToBaseInt()));
				else
					Log.Write(Log.Level.Error, "Expected 'Resolve' with '", opcode.Name, "', but no matching Attribute was found in ", xOperandNode.InnerXml);
				break;

			case OperandType.InlineArg:
			case OperandType.ShortInlineArg:
				string[] parArrVal = xOperandNode.GetAttribute(SST.Resolve).Split(' ');
				if (parArrVal.Length != 2)
				{
					int parIndexVal = int.Parse(parArrVal[0]);
					//object parTypVal = ILManager.Instance.Resolve(parArrVal[1].ToBaseInt());
					return ILManager.GenInstruction(opcode, MethodDef.Parameters[parIndexVal]);
				}
				else
					Log.Write(Log.Level.Error, "Expected 'Resolve' with '", opcode.Name, "', but the Attribute is either missing or incorrect in ", xOperandNode.InnerXml);
				break;

			case OperandType.InlineVar:
			case OperandType.ShortInlineVar:
				string[] varArrVal = xOperandNode.GetAttribute(SST.Resolve).Split(' ');
				if (varArrVal.Length != 2)
				{
					int varIndexVal = int.Parse(varArrVal[0]);
					//object varTypVal = ILManager.Instance.Resolve(varArrVal[1].ToBaseInt());
					return ILManager.GenInstruction(opcode, MethodDef.Parameters[varIndexVal]);
				}
				else
					Log.Write(Log.Level.Error, "Expected 'Resolve' with '", opcode.Name, "', but the Attribute is either missing or incorrect in ", xOperandNode.InnerXml);
				break;

			case OperandType.InlineBrTarget:
			case OperandType.ShortInlineBrTarget:
				string briVal = xOperandNode.GetAttribute(SST.BrTargetIndex);
				if (briVal != string.Empty)
				{
					PostInitData pid = new PostInitData();
					pid.InstructionNum = nInstructionNum;
					pid.isArray = false;
					pid.targetNum = int.Parse(briVal);
					postinitbrs.Add(pid);
					ILManager.GenInstruction(opcode, Instruction.Create(OpCodes.Nop));
				}
				break;

			case OperandType.InlineSwitch:
				string braVal = xOperandNode.GetAttribute(SST.BrTargetArray);
				if (braVal != string.Empty)
				{
					PostInitData pid = new PostInitData();
					pid.InstructionNum = nInstructionNum;
					pid.isArray = true;
					pid.targetArray = Array.ConvertAll<string, int>(braVal.Trim().Split(new[] { ' ' }), s => int.Parse(s));
					postinitbrs.Add(pid);
					ILManager.GenInstruction(opcode, new Instruction[0]);
				}
				break;

			case OperandType.InlineSig:
			case OperandType.InlinePhi:
			default:
				Log.Write(Log.Level.Error, "Opcode not processed: ", opcode.Name);
				break;
			}
			return null;
		}

		/// <summary>Looks for the index of a Instruction</summary>
		/// <param name="i">The instruction to be searched</param>
		/// <param name="OldI">True will look for the target in the old Instruction list, false in the new one</param>
		/// <returns></returns>
		private int FindInstruction(Instruction i, bool OldI)
		{
			int res = -1;
			if (OldI)
				res = MethodDef.Body.Instructions.IndexOf(i);
			else
			{
				InstructionInfo fII = instructPatchList.First<InstructionInfo>(x => x.NewInstruction == i);
				if (fII != null) res = fII.NewInstructionNum;
			}

			if (res == -1) Log.Write(Log.Level.Warning, "Instruction not found: ", i.Offset.ToString());
			return res;
		}

		/// <summary>Sets the status of the Patch to "WorkingPerfectly" and and saves the given MethodDefinition</summary>
		/// <param name="MetDef">The MethodDefinition for this Patch</param>
		public void SetInitWorking(MethodDefinition MetDef)
		{
			MethodDef = MetDef;
			OriginalInstructionCount = MetDef.Body.Instructions.Count;
			_PatchStatus = PatchStatus.WoringPerfectly;
		}

		/// <summary>Data staructure to save branch Instructions in order to initialize the later</summary>
		private class PostInitData
		{
			/// <summary>The index of the branch Instruction after the patch</summary>
			public int InstructionNum;
			/// <summary>True if the Instruction is a switch, false if single branch</summary>
			public bool isArray;
			/// <summary>The branch target index after the patch</summary>
			public int targetNum;
			/// <summary>The switch targets after the patch</summary>
			public int[] targetArray;

			/// <summary>Prints a human readable branch patch</summary>
			/// <returns>Returns the String</returns>
			public override string ToString()
			{
				if (isArray)
				{
					if (targetArray != null)
						return InstructionNum + " -> {" + string.Join(", ", targetArray) + "}";
					else
						return InstructionNum + " -> null";
				}
				else
					return InstructionNum + " -> " + targetNum;
			}

		}
	}
}
