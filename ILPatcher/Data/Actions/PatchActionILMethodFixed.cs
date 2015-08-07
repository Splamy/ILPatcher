using ILPatcher.Utility;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ILPatcher.Data.Actions
{
	public class PatchActionILMethodFixed : PatchAction
	{
		//I: NamedElement
		public override string Label { get { return "+/- changes"; } } // TODO: implement
		public override string Description { get { return "Modifies an entire method by changing/adding instructions individually"; } }

		//I: PatchAction
		public override PatchActionType PatchActionType { get { return PatchActionType.ILMethodFixed; } }
		public override bool RequiresFixedOutput { get { return true; } }
		public override Type TInput { get { return typeof(MethodDefinition); } }

		//Own:
		public MethodDefinition methodDefinition;
		private int OriginalInstructionCount;
		public List<InstructionInfo> instructPatchList;

		public PatchActionILMethodFixed(DataStruct dataStruct)
			: base(dataStruct)
		{

		}

		/// <summary>Sets the status of the Patch to "WorkingPerfectly" and and saves the given MethodDefinition</summary>
		/// <param name="target">The MethodDefinition for this Patch</param>
		public override void PassTarget(object target)
		{
			methodDefinition = (MethodDefinition)target;
			OriginalInstructionCount = methodDefinition.Body.Instructions.Count;
			PatchStatus = PatchStatus.WoringPerfectly;
		}

		public override bool Execute(object target)
		{
			var modMethod = (MethodDefinition)target;
			AnyArray<Instruction> cpyBuffer = new AnyArray<Instruction>();
			foreach (InstructionInfo II in instructPatchList)
			{
				if (II.Delete) continue;
				cpyBuffer[II.NewInstructionNum] = II.NewInstruction;
			}
			Mono.Collections.Generic.Collection<Instruction> nList = modMethod.Body.Instructions;
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
			if (methodDefinition == null)
			{
				Log.Write(Log.Level.Careful, "The PatchAction(ILMethodFixed) ", this.Name, " is empty and won't be saved!");
				return false;
			}

			NameCompressor nc = NameCompressor.Instance;

			output.Attributes[nc[SST.PatchType]].Value = PatchActionType.ToString();
			output.Attributes[nc[SST.Name]].Value = this.Name;

			instructPatchList = instructPatchList.FindAll(x => !x.Delete || x.IsOld);

			XmlNode xListPatched = output.InsertCompressedElement(SST.PatchList);
			xListPatched.CreateAttribute(SST.MethodPath, dataStruct.ReferenceTable.Reference(methodDefinition).ToBaseAlph());
			xListPatched.CreateAttribute(SST.InstructionCount, OriginalInstructionCount.ToString());

			var xmlInstructionOriginalMethod = new XMLInstruction<Instruction>(
				dataStruct.ReferenceTable,
				methodDefinition.Body.Instructions,
				(list, instruct) => list.IndexOf(instruct));

			var xmlInstructionModifiedList = new XMLInstruction<InstructionInfo>(
				dataStruct.ReferenceTable,
				instructPatchList,
				(list, instruct) => { var res = list.FirstOrDefault(x => x.NewInstruction == instruct); return res != null ? res.NewInstructionNum : -1; });

			int instructionPos = 0;
			foreach (InstructionInfo II in instructPatchList)
			{
				XmlNode xInstruction = xListPatched.InsertCompressedElement(SST.Instruction);
				II.NewInstructionNum = instructionPos;

				if (II.IsOld)
				{
					//OldInstructionNum/OriginalInstructionNum: -1 if new command

					xInstruction.CreateAttribute(SST.InstructionNum, II.OldInstructionNum.ToString());
					xInstruction.CreateAttribute(SST.OpCode, II.OldInstruction.OpCode.Name);
					xInstruction.CreateAttribute(SST.Delete, nc[II.Delete ? SST.True : SST.False]);
					xmlInstructionOriginalMethod.Operand2Node(xInstruction, II.OldInstruction);

					if (!II.Delete)
					{
						instructionPos++;

						XmlNode patchelem = null;
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
							xmlInstructionModifiedList.Operand2Node(patchelem, II.NewInstruction);
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
						xmlInstructionModifiedList.Operand2Node(xInstruction, II.NewInstruction);
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

			XmlNode PatchList = input.GetChildNode(SST.PatchList, 0);
			if (PatchList == null) { Log.Write(Log.Level.Error, "No PatchList Child found"); PatchStatus = PatchStatus.Broken; return false; }

			string metpathunres = PatchList.GetAttribute(SST.MethodPath);
			if (string.IsNullOrEmpty(metpathunres)) { Log.Write(Log.Level.Error, "MethodPath Attribute not found or empty"); PatchStatus = PatchStatus.Broken; return false; }
			methodDefinition = dataStruct.ReferenceTable.Resolve(metpathunres.ToBaseInt()) as MethodDefinition;
			if (methodDefinition == null) { Log.Write(Log.Level.Error, "MethodID <", metpathunres, "> couldn't be resolved"); PatchStatus = PatchStatus.Broken; return false; }

			// TODO: move methodDef related checks to Execute
			OriginalInstructionCount = int.Parse(PatchList.GetAttribute(SST.InstructionCount));
			if (methodDefinition.Body.Instructions.Count != OriginalInstructionCount)
			{
				// new method body has changed -> patching the new assembly will not work
				Log.Write(Log.Level.Error, "The PatchAction \"", this.Name, "\" cannot be applied to a changend method"); PatchStatus = PatchStatus.Broken; return false;
			}

			// TODO: init with given params, instead of static
			AnyArray<InstructionInfo> iibuffer = new AnyArray<InstructionInfo>(OriginalInstructionCount);
			bool checkopcdes = true;
			bool resolveparams = false; // resolves types/methods/...
			bool checkprimitives = true; // checks if primitive types are identical
			
			var xmlInstructionLoader = new XMLInstruction<InstructionInfo>(
				dataStruct.ReferenceTable,
				methodDefinition);

			#region Load all InstructionInfo
			foreach (XmlNode xelem in PatchList.ChildNodes)
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
						nII.OldInstruction = methodDefinition.Body.Instructions[nII.OldInstructionNum];
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
						XmlNode xpatchelem = xelem.ChildNodes[0];

						string instnum = xpatchelem.GetAttribute(SST.InstructionNum);
						if (string.IsNullOrEmpty(instnum)) // check InstructionNum Patch
							nII.NewInstructionNum = nII.OldInstructionNum;
						else
							nII.NewInstructionNum = int.Parse(instnum);

						OpCode patchopc;
						string patchopcode = xpatchelem.GetAttribute(SST.OpCode);
						if (string.IsNullOrEmpty(patchopcode)) // check Opcode patch
							patchopc = opcode;
						else
							patchopc = ILManager.OpCodeLookup[patchopcode];

						nII.NewInstruction = xmlInstructionLoader.Node2Instruction(xpatchelem, patchopc, nII.NewInstructionNum);
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
					nII.NewInstruction = xmlInstructionLoader.Node2Instruction(xelem, opcode, nII.NewInstructionNum);
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
				Log.Write(Log.Level.Error, "PatchList has holes: ", string.Join(", ", instructPatchList.Select((b, i) => b == null ? i : -1).Where(i => i != -1).ToArray()));
				PatchStatus = PatchStatus.Broken;
				return false;
			}
			#endregion

			#region Set all jump operands from PostInitData
			foreach (PostInitData pid in xmlInstructionLoader.GetPostInitalisationList())
			{
				if (pid.isArray)
				{
					bool success = true;
					instructPatchList[pid.InstructionNum].NewInstruction.Operand = Array.ConvertAll(pid.targetArray, a =>
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
	}
}
