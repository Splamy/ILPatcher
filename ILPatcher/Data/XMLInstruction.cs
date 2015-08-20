using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

using Mono.Cecil;
using Mono.Cecil.Cil;
using ILPatcher.Utility;

namespace ILPatcher.Data
{
	class XMLInstruction<T>
	{
		private ILManager manager;
		private MethodDefinition workingMethod;
		private IList<T> workingInstructionList;
		private Func<IList<T>, Instruction, int> indexFinder;
		private List<PostInitData> postinitbrs;
		private XMLIType initType;

		private XMLInstruction(ILManager manager, MethodDefinition workingMethod, IList<T> workingInstructionList, Func<IList<T>, Instruction, int> indexFinder)
		{
			this.manager = manager;
			this.workingMethod = workingMethod;
			this.workingInstructionList = workingInstructionList;
			this.indexFinder = indexFinder;
			postinitbrs = new List<PostInitData>();
			initType = XMLIType.Unknown;
		}

		public XMLInstruction(ILManager manager, IList<T> workingInstructionList, Func<IList<T>, Instruction, int> indexFinder)
			: this(manager, null, workingInstructionList, indexFinder)
		{
			initType = XMLIType.WriteOnly;
		}

		public XMLInstruction(ILManager manager, MethodDefinition workingMethod)
			: this(manager, workingMethod, null, null)
		{
			initType = XMLIType.ReadOnly;
		}

		/// <summary>Saves an Operand from an Instruction to a Instruction XmlNode</summary>
		/// <param name="xParent">The XmlNode where the new Instruction XmlNode will be added as child node</param>
		/// <param name="i">The Instruction with the Operand to be saved</param>
		/// <param name="OldI">If the Instruction is a branch, true will look for the target in the old Instruction list, false in the new one</param>
		public void Operand2Node(XmlNode xParent, Instruction i)
		{
			if (initType != XMLIType.WriteOnly) throw new InvalidOperationException("Wrong XMLI constructor used.");

			OpCode oc = i.OpCode;
			object operand = i.Operand;
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
				xParent.CreateAttribute(SST.Resolve, manager.Reference(operand).ToBaseAlph());
				break;

			case OperandType.InlineArg:
			case OperandType.ShortInlineArg:
				ParameterReference parref = ((ParameterReference)operand); // TODO: check this
				strb = new StringBuilder();
				strb.Append(parref.Index.ToString());
				strb.Append(' ');
				strb.Append(manager.Reference(parref.ParameterType).ToBaseAlph());
				xParent.CreateAttribute(SST.Resolve, strb.ToString());
				break;

			case OperandType.InlineVar:
			case OperandType.ShortInlineVar:
				VariableReference varref = ((VariableReference)operand); // TODO: check this
				strb = new StringBuilder();
				strb.Append(varref.Index.ToString());
				strb.Append(' ');
				strb.Append(manager.Reference(varref.VariableType).ToBaseAlph());
				xParent.CreateAttribute(SST.Resolve, strb.ToString());
				break;

			case OperandType.InlineBrTarget:
			case OperandType.ShortInlineBrTarget:
				xParent.CreateAttribute(SST.BrTargetIndex, FindInstruction((Instruction)i.Operand).ToString());
				break;

			case OperandType.InlineSwitch:
				strb = new StringBuilder();
				Instruction[] arr = (Instruction[])operand;
				foreach (Instruction instr in arr)
				{
					strb.Append(FindInstruction(instr));
					strb.Append(' ');
				}
				xParent.CreateAttribute(SST.BrTargetArray, strb.ToString());
				break;

			case OperandType.InlineSig:
			case OperandType.InlinePhi:
			default:
				Log.Write(Log.Level.Error, $"Opcode \"{oc.Name}\" not processed");
				break;
			}
		}

		/// <summary>Reads an XmlNode and creates an Instruction from the data</summary>
		/// <param name="xOperandNode">The XmlElement containing the operand info, created by Operand2Node</param>
		/// <param name="opcode">The OpCode the new Instruction should have, independently of the XmlElement</param>
		/// <param name="nInstructionNum">The NewInstructionNum the Instruction will have after the patch (InstructionInfo.NewInstructionNum)</param>
		/// <param name="postinitbrs">A reference to the PostInitData list, in order to load branch Instructions correctly</param>
		/// <returns>Returns the new Instruction if succeeded, null otherwise</returns>
		public Instruction Node2Instruction(XmlNode xOperandNode, OpCode opcode, int nInstructionNum)
		{
			if (initType != XMLIType.ReadOnly) throw new InvalidOperationException("Wrong XMLI constructor used.");

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
					Log.Write(Log.Level.Error, $"Expected 'PrimitiveValue' with \"{opcode.Name}\", but no matching Attribute was found in \"{xOperandNode.InnerXml}\"");
				break;

			case OperandType.InlineField:
			case OperandType.InlineMethod:
			case OperandType.InlineTok:
			case OperandType.InlineType:
				string opVal = xOperandNode.GetAttribute(SST.Resolve);
				if (!string.IsNullOrEmpty(opVal))
					return ILManager.GenInstruction(opcode, manager.Resolve(opVal.ToBaseInt()));
				else
					Log.Write(Log.Level.Error, $"Expected 'Resolve' with \"{opcode.Name}\", but no matching Attribute was found in \"{xOperandNode.InnerXml}\"");
				break;

			case OperandType.InlineArg:
			case OperandType.ShortInlineArg:
				string[] parArrVal = xOperandNode.GetAttribute(SST.Resolve).Split(' ');
				if (parArrVal.Length != 2)
				{
					int parIndexVal = int.Parse(parArrVal[0]);
					//object parTypVal = ILManager.Instance.Resolve(parArrVal[1].ToBaseInt());
					return ILManager.GenInstruction(opcode, workingMethod.Parameters[parIndexVal]);
				}
				else
					Log.Write(Log.Level.Error, $"Expected 'Resolve' with \"{opcode.Name}\", but the Attribute is either missing or incorrect in \"{xOperandNode.InnerXml}\"");
				break;

			case OperandType.InlineVar:
			case OperandType.ShortInlineVar:
				string[] varArrVal = xOperandNode.GetAttribute(SST.Resolve).Split(' ');
				if (varArrVal.Length != 2)
				{
					int varIndexVal = int.Parse(varArrVal[0]);
					//object varTypVal = ILManager.Instance.Resolve(varArrVal[1].ToBaseInt());
					return ILManager.GenInstruction(opcode, workingMethod.Parameters[varIndexVal]);
				}
				else
					Log.Write(Log.Level.Error, $"Expected 'Resolve' with \"{opcode.Name}\", but the Attribute is either missing or incorrect in \"{xOperandNode.InnerXml}\"");
				break;

			case OperandType.InlineBrTarget:
			case OperandType.ShortInlineBrTarget:
				string briVal = xOperandNode.GetAttribute(SST.BrTargetIndex);
				if (!string.IsNullOrEmpty(briVal))
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
				if (!string.IsNullOrEmpty(braVal))
				{
					PostInitData pid = new PostInitData();
					pid.InstructionNum = nInstructionNum;
					pid.isArray = true;
					pid.targetArray = Array.ConvertAll(braVal.Trim().Split(new[] { ' ' }), s => int.Parse(s));
					postinitbrs.Add(pid);
					ILManager.GenInstruction(opcode, new Instruction[0]);
				}
				break;

			case OperandType.InlineSig:
			case OperandType.InlinePhi:
			default:
				Log.Write(Log.Level.Error, $"Opcode \"{opcode.Name}\" not processed");
				break;
			}
			return null;
		}

		/// <summary>Looks for the index of a Instruction</summary>
		/// <param name="i">The instruction to be searched</param>
		/// <param name="OldI">True will look for the target in the old Instruction list, false in the new one</param>
		/// <returns></returns>
		private int FindInstruction(Instruction i)
		{
			int res = indexFinder(workingInstructionList, i);
			if (res == -1) Log.Write(Log.Level.Warning, $"Instruction \"{i.Offset}\" not found");
			return res;
		}

		public ReadOnlyCollection<PostInitData> GetPostInitalisationList()
		{
			return postinitbrs.AsReadOnly();
		}

		private enum XMLIType
		{
			Unknown,
			ReadOnly,
			WriteOnly,
		}
	}

	/// <summary>Data staructure to save branch Instructions in order to initialize the later</summary>
	public class PostInitData
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
