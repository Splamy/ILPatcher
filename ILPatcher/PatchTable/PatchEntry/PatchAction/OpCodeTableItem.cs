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
	public class OpCodeTableItem : DragItem
	{
		public InstructionInfo II;
		private MListBox parent;
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



		public OpCodeTableItem(MListBox nParent, InstructionInfo nII)
		{
			II = nII;
			parent = nParent;

		}

		public override void Draw(System.Drawing.Graphics g, System.Drawing.RectangleF rec)
		{
#if DEBUG
			//This severe Problem shouldn't happen in Release
			if (II.NewInstruction == null)
			{
				Log.Write(Log.Level.Error, "NewInstruction is null!");
				return;
			}
#endif
			int split = (int)g.MeasureString("999>999", Font).Width;
			RefreshHeight(g, (int)rec.Width);

			if (II.Delete)
				g.DrawString(II.OldInstructionNum + ">X", Font, Brushes.Black, rec.Left, rec.Top + 1);
			else if (II.NewInstructionNum == -1)
				g.DrawString("(" + dragFrom + ")", Font, Brushes.Black, rec.Left, rec.Top + 1);
			else if (II.IsOld && II.InstructionNumPatch)
				g.DrawString(II.OldInstructionNum + ">" + II.NewInstructionNum, Font, Brushes.Black, rec.Left, rec.Top + 1);
			else if (II.IsNew)
				g.DrawString("=" + II.NewInstructionNum.ToString(), Font, Brushes.Black, rec.Left, rec.Top + 1);
			else
				g.DrawString(II.NewInstructionNum.ToString(), Font, Brushes.Black, rec.Left, rec.Top + 1);
			if (II.Delete)
			{
				g.DrawLine(Pens.Red, rec.Left, rec.Top, split, rec.Bottom);
				g.DrawLine(Pens.Red, rec.Left, rec.Bottom, split, rec.Top);
			}
			g.DrawLine(Pens.Black, split, rec.Top, split, rec.Bottom);
			bool unchangedOpcode = (II.OldInstruction == null) || (II.OldInstruction.OpCode == II.NewInstruction.OpCode);
			g.DrawString(II.NewInstruction.OpCode.Name, Font, unchangedOpcode ? Brushes.Black : Brushes.Red, split + 2, rec.Top + 1);
			g.DrawLine(Pens.Black, 100, rec.Top, 100, rec.Bottom);
			if (II.OperandMismatch)
			{
				//_hbrMismatch = new HatchBrush(HatchStyle.Percent25, Color.Orange, Color.Transparent);
				g.FillRectangle(hbrMismatch, 102, rec.Top, rec.Right - 102, Height);
			}
			if (II.NewInstruction.Operand == null)
				g.DrawString("-", Font, Brushes.Black, 102, rec.Top + 1);
			else
				g.DrawString(II.NewInstruction.Operand.GetType().Name + " " + II.NewInstruction.Operand.ToString(), Font, Brushes.Black, 102, rec.Top + 1);
		}

		public override void RefreshHeight(System.Drawing.Graphics g, int nWidth)
		{
			if (this.Width != nWidth)
			{
				Width = nWidth;
				Height = Font.Height + 1;
			}
		}
	}
}
