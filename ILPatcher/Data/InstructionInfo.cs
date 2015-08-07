using ILPatcher.Utility;
using MetroObjects;
using Mono.Cecil.Cil;
using System.Drawing;

namespace ILPatcher.Data
{
	// TODO: rework! seperate data and interface
	public class InstructionInfo : DragItem
	{
		public int dragFrom = -1;
		private static Brush _hbrMismatch = null;
		private static Brush hbrMismatch
		{
			get
			{
				if (_hbrMismatch == null)
					_hbrMismatch = new SolidBrush(Color.Orange); //HatchBrush(HatchStyle.Percent10, Color.Yellow, Color.Transparent);
				return _hbrMismatch;
			}
		}

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
		}
		public bool InstructionOpCodePatch { get { return OldInstructionNum != -1 && OldInstruction.OpCode != NewInstruction.OpCode; } }
		public bool InstructionNumPatch { get { return OldInstructionNum != -1 && OldInstructionNum != NewInstructionNum; } }
		public bool IsNew { get { return OldInstructionNum == -1; } }
		public bool IsOld { get { return OldInstructionNum != -1; } }

		//LoadInfo
		public bool OpCodeMismatch = false;
		public bool PrimitiveMismatch = false;
		public bool ReferenceMismatch = false;
		public bool OperandMismatch { get { return PrimitiveMismatch || ReferenceMismatch; } }

		public InstructionInfo() { }

		protected override void DrawBuffer(Graphics g)
		{
			int split = (int)g.MeasureString("999>999", Font).Width;

			if (Delete)
				g.DrawString(OldInstructionNum + ">X", Font, Brushes.Black, 0, 1);
			else if (NewInstructionNum == -1)
				g.DrawString("(" + dragFrom + ")", Font, Brushes.Black, 0, 1);
			else if (IsOld && InstructionNumPatch)
				g.DrawString(OldInstructionNum + ">" + NewInstructionNum, Font, Brushes.Black, 0, 1);
			else if (IsNew)
				g.DrawString("=" + NewInstructionNum.ToString(), Font, Brushes.Black, 0, 1);
			else
				g.DrawString(NewInstructionNum.ToString(), Font, Brushes.Black, 0, 1);
			if (Delete)
			{
				g.DrawLine(Pens.Red, 0, 0, split, Size.Height);
				g.DrawLine(Pens.Red, 0, Size.Height, split, 0);
			}
			g.DrawLine(Pens.Black, split, 0, split, Size.Height);
			g.DrawString(NewInstruction.OpCode.Name, Font, InstructionOpCodePatch ? Brushes.Red : Brushes.Black, split + 2, 1);
			g.DrawLine(Pens.Black, 100, 0, 100, Size.Height);
			if (OperandMismatch)
				g.FillRectangle(hbrMismatch, 102, 0, Size.Width - 102, Size.Height);
			if (NewInstruction.Operand == null)
				g.DrawString("-", Font, InstructionOperandPatch ? Brushes.Red : Brushes.Black, 102, 1);
			else
				g.DrawString(CecilFormatter.TryFormat(NewInstruction.Operand), Font, InstructionOperandPatch ? Brushes.Red : Brushes.Black, 102, 1);
		}

		protected override int GetHeightFromWidth(int width)
		{
			return Font.Height + 1;
		}

		/// <summary>Creates a shallow copy of this InstructionInfo</summary>
		/// <returns>A new InstructionInfo with the same values</returns>
		public InstructionInfo Clone()
		{
			InstructionInfo II = new InstructionInfo();
			II.OldInstruction = OldInstruction;
			II.NewInstruction = NewInstruction;
			II.OldInstructionNum = OldInstructionNum;
			II.NewInstructionNum = NewInstructionNum;

			II.Delete = Delete;
			II.OpCodeMismatch = OpCodeMismatch;
			II.PrimitiveMismatch = PrimitiveMismatch;
			II.ReferenceMismatch = ReferenceMismatch;

			return II;
		}
	}
}
