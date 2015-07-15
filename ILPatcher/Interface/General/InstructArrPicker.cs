using ILPatcher.Data;
using ILPatcher.Utility;
using MetroObjects;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ILPatcher.Interface.General
{
	public partial class InstructArrPicker : Form
	{
		private Action<Instruction[]> callback;

		private static InstructArrPicker _Instance;
		public static InstructArrPicker Instance
		{
			get { if (_Instance == null || _Instance.IsDisposed) _Instance = new InstructArrPicker(); return _Instance; }
			set { _Instance = value; }
		}

		private InstructArrPicker()
		{
			InitializeComponent();

			lbxAllInstruct.MouseDoubleClick += lbxAllInstruct_MouseDoubleClick;
			lbxSwitchInstruct.MouseDoubleClick += lbxSwitchInstruct_MouseDoubleClick;
		}

		private void lbxAllInstruct_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			InstructionInfo II = ((InstructionInfo)lbxAllInstruct.SelectedElement);
			lbxSwitchInstruct.AddItem(new InstructElement(II.NewInstruction, II.NewInstructionNum));
		}

		private void lbxSwitchInstruct_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			lbxSwitchInstruct.RemoveItem(lbxSwitchInstruct.SelectedElement);
		}

		public void ShowStructure(List<DragItem> inlist, Instruction[] oldlist, Action<Instruction[]> cb)
		{
			callback = cb;

			lbxAllInstruct.ClearItems();
			lbxSwitchInstruct.ClearItems();

			lbxAllInstruct.Items = inlist;
			if (oldlist != null)
			{
				List<InstructionInfo> inlistconv = inlist.ConvertAll<InstructionInfo>(x => (InstructionInfo)x);
				Array.ForEach(oldlist, instr => lbxSwitchInstruct.AddItem(new InstructElement(instr, inlistconv.Find(x => x.NewInstruction == instr).NewInstructionNum)));
			}
			this.Show();
		}

		private void btn_Select_Click(object sender, EventArgs e)
		{
			Hide();
			callback(lbxSwitchInstruct.Items.Select<DragItem, Instruction>(x => ((InstructElement)x).instr).ToArray());
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Hide();
		}

		private void InstructArrPicker_Resize(object sender, EventArgs e)
		{
			const int space = 5;

			lblAllInstruct.Width = (this.Width - (3 * space + 20)) / 2;
			lblSwitchInstruct.Left = lblAllInstruct.Right + space;
			lblSwitchInstruct.Width = lblAllInstruct.Width;

			lbxAllInstruct.Width = lblAllInstruct.Width;
			lbxSwitchInstruct.Left = lblSwitchInstruct.Left;
			lbxSwitchInstruct.Width = lblSwitchInstruct.Width;

			lbxAllInstruct.Height = this.Height - (lblAllInstruct.Height + btn_Select.Height + 3 * space + 50);
			lbxSwitchInstruct.Height = lbxAllInstruct.Height;

			btn_Cancel.Top = lbxAllInstruct.Bottom + space;
			btn_Select.Top = btn_Cancel.Top;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		private class InstructElement : DragItem
		{
			private int instrnum = 0;
			public Instruction instr;
			private string compoundstr;

			public InstructElement(Instruction i, int num)
			{
				instr = i;
				instrnum = num; // check use
				compoundstr = CecilFormatter.Format(instr, num);
			}

			protected override void DrawBuffer(Graphics g)
			{
				g.DrawString(compoundstr, Font, Brushes.Black, 2, 1);
			}

			protected override int GetHeightFromWidth(int width)
			{
				return Font.Height + 1;
			}
		}
	}
}
