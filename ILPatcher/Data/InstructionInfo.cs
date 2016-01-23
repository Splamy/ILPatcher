using System;
using System.Drawing;
using ILPatcher.Utility;
using MetroObjects;
using Mono.Cecil.Cil;

namespace ILPatcher.Data
{
	// TODO: rework! seperate data and interface
	public class InstructionInfo : DragItem
	{
		public int DragFrom { get; set; } = -1;
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

		public Instruction OldInstruction { get; set; }
		public Instruction NewInstruction { get; set; }
		public int OldInstructionNum { get; set; }
		public int NewInstructionNum { get; set; }

		//PatchInfo
		public bool Delete { get; set; } = false;
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
						Log.Write(Log.Level.Warning, $"Operand info \"{NewInstruction}\" is wrong");
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
		public bool InstructionOpCodePatch => OldInstructionNum != -1 && OldInstruction.OpCode != NewInstruction.OpCode;
		public bool InstructionNumPatch => OldInstructionNum != -1 && OldInstructionNum != NewInstructionNum;
		public bool IsNew => OldInstructionNum == -1;
		public bool IsOld => OldInstructionNum != -1;

		//LoadInfo
		public bool OpCodeMismatch { get; set; } = false;
		public bool PrimitiveMismatch { get; set; } = false;
		public bool ReferenceMismatch { get; set; } = false;
		public bool OperandMismatch => PrimitiveMismatch || ReferenceMismatch;

		public InstructionInfo() { }

		protected override void DrawBuffer(Graphics g)
		{
			if (g == null)
				throw new ArgumentNullException(nameof(g));

			int split = (int)g.MeasureString("999>999", Font).Width;

			if (Delete)
				g.DrawString(OldInstructionNum + ">X", Font, Brushes.Black, 0, 1);
			else if (NewInstructionNum == -1)
				g.DrawString("(" + DragFrom + ")", Font, Brushes.Black, 0, 1);
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
