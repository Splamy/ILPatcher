using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

using MetroObjects;
using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
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
			set { }
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

		public InstructionInfo() { }

		public override void Draw(System.Drawing.Graphics g, System.Drawing.RectangleF rec)
		{
			int split = (int)g.MeasureString("999>999", Font).Width;
			RefreshHeight(g, (int)rec.Width);

			if (Delete)
				g.DrawString(OldInstructionNum + ">X", Font, Brushes.Black, rec.Left, rec.Top + 1);
			else if (NewInstructionNum == -1)
				g.DrawString("(" + dragFrom + ")", Font, Brushes.Black, rec.Left, rec.Top + 1);
			else if (IsOld && InstructionNumPatch)
				g.DrawString(OldInstructionNum + ">" + NewInstructionNum, Font, Brushes.Black, rec.Left, rec.Top + 1);
			else if (IsNew)
				g.DrawString("=" + NewInstructionNum.ToString(), Font, Brushes.Black, rec.Left, rec.Top + 1);
			else
				g.DrawString(NewInstructionNum.ToString(), Font, Brushes.Black, rec.Left, rec.Top + 1);
			if (Delete)
			{
				g.DrawLine(Pens.Red, rec.Left, rec.Top, split, rec.Bottom);
				g.DrawLine(Pens.Red, rec.Left, rec.Bottom, split, rec.Top);
			}
			g.DrawLine(Pens.Black, split, rec.Top, split, rec.Bottom);
			g.DrawString(NewInstruction.OpCode.Name, Font, InstructionOpCodePatch ? Brushes.Red : Brushes.Black, split + 2, rec.Top + 1);
			g.DrawLine(Pens.Black, 100, rec.Top, 100, rec.Bottom);
			if (OperandMismatch)
				g.FillRectangle(hbrMismatch, 102, rec.Top, rec.Right - 102, Height);
			if (NewInstruction.Operand == null)
				g.DrawString("-", Font, InstructionOperandPatch ? Brushes.Red : Brushes.Black, 102, rec.Top + 1);
			else // TODO: operand type at the beginning only temp: NewInstruction.Operand.GetType().Name + " " +
				g.DrawString(CecilFormatter.TryFormat(NewInstruction.Operand), Font, InstructionOperandPatch ? Brushes.Red : Brushes.Black, 102, rec.Top + 1);
		}

		public override void RefreshHeight(System.Drawing.Graphics g, int nWidth)
		{
			if (this.Width != nWidth)
			{
				Width = nWidth;
				Height = Font.Height + 1;
			}
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
