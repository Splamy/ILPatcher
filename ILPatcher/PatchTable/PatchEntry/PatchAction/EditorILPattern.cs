using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

using MetroObjects;

namespace ILPatcher
{
	public partial class EditorILPattern : UserControl
	{
		MethodDefinition MetDef;

		MultiPicker btn1MultiPicker;
		Control[] OperandCList;

		private static EditorILPattern _Instance;
		public static EditorILPattern Instance
		{
			get { if (_Instance == null) _Instance = new EditorILPattern(); return _Instance; }
			protected set { _Instance = value; }
		}

		public PatchActionILMethodFixed PatchAction { get; set; }

		//openrand combo box add: wildcard, custom value(+default)
		//predeclared variables
		//(predeclared params)

		private EditorILPattern()
		{
			InitializeComponent();

			mInstructBox.MouseClick += mInstructBox_MouseClick;
			chbDelete.OnChange += chbDelete_OnChange;
			instructionEditor.OnItemDrop += instructionEditor_OnItemDrop;
			mInstructBox.OnItemDropFailed += mInstructBox_OnItemDropFailed;
			OperandCList = new Control[] { txtOperand, cbxOperand, lblwip };

			foreach (string dn in ILManager.OpCodeLookup.Keys)
				cbxOpcode.Items.Add(dn);
		}

		private void EditorILPattern_Resize(object sender, EventArgs e)
		{
			Controls_Reorganize();
		}

		// INTERFACE *********************************************************

		private void chbDelete_OnChange(MCheckBox source, bool value)
		{
			if (mInstructBox.SelectedItems.Count <= 0)
				BoxSetHelper(false);
			else
			{
				mInstructBox.SelectedItems.ForEach(x => ((OpCodeTableItem)x).II.Delete = value);
				mInstructBox.InvalidateChildren();
			}
		}

		private void btnDone_Click(object sender, EventArgs e)
		{
			if (PatchAction == null)
			{
				PatchAction = new PatchActionILMethodFixed();
				PatchAction.SetInitWorking();

				PatchAction.instructPatchList = mInstructBox.Items.ConvertAll<InstructionInfo>(x => ((OpCodeTableItem)x).II);
				PatchAction.MethodDef = MetDef;
			}

			PatchAction.ActionName = txtPatchActionName.Text;

			EditorEntry.Instance.Add(PatchAction); // TODO refresh patchaction-list
			((SwooshPanel)Parent).SwooshTo(EditorEntry.Instance);
		}

		private void btnPickMethod_Click(object sender, EventArgs e)
		{
			if (btn1MultiPicker == null || btn1MultiPicker.IsDisposed)
				btn1MultiPicker = new MultiPicker(this);
			btn1MultiPicker.Show();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			// TODO ^^
		}

		private void btnDebug_Click(object sender, EventArgs e)
		{
			System.Xml.XmlDocument xDoc = MainPanel.ReadFromFile("testEntry.xml");
			PatchAction = new PatchActionILMethodFixed();
			foreach (System.Xml.XmlNode xNode in xDoc.ChildNodes)
			{
				if (xNode.Name == "PatchAction")
				{
					ILManager.Instance.Load(xNode);
					PatchAction.Load(xNode);
				}
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch ((PickOperandType)cbxOperandType.SelectedIndex)
			{
			case PickOperandType.None:
				ShowInput(InputType.None);
				break;
			case PickOperandType.Byte:
			case PickOperandType.SByte:
			case PickOperandType.Int32:
			case PickOperandType.Int64:
			case PickOperandType.Single:
			case PickOperandType.Double:
			case PickOperandType.String:
				ShowInput(InputType.Box);
				break;
			case PickOperandType.InstructionReference:
				ShowInput(InputType.IntructList);
				break;
			case PickOperandType.VariableReference:
				ShowInput(InputType.VarList);
				break;
			case PickOperandType.ParameterReference:
				ShowInput(InputType.ParamList);
				break;
			case PickOperandType.FieldReference:
				ShowInput(InputType.FieldList);
				break;
			case PickOperandType.MethodReference:
				ShowInput(InputType.MethodList);
				break;
			case PickOperandType.TypeReference:
				ShowInput(InputType.TypeList);
				break;
			default:
				Log.Write(Log.Level.Warning, "Not switced PickOperadType in combobox");
				break;
			}
		}

		private void btnNewOpCode_Click(object sender, EventArgs e)
		{
			instructionEditor.DragItem = null;
			instructionEditor.AllowDrag = true;
			cbxOpcode.Text = string.Empty;
			MakeItemAvailable();
		}

		// INTERFACE TOOLS ***************************************************

		private void BoxSetHelper(bool val)
		{
			chbDelete.OnChange -= chbDelete_OnChange;
			chbDelete.ON = val;
			chbDelete.OnChange += chbDelete_OnChange;
		}

		private void Controls_Reorganize()
		{
			const int space = 5;
			const int labelspace = 85 + 2 * space;

			txtPatchActionName.Width = Width - (labelspace + space);
			txtMethodFullName.Width = Width - (2 * space + labelspace + btnPickMethod.Width);
			btnPickMethod.Left = Width - (space + btnPickMethod.Width);
			mInstructBox.Width = Width - (labelspace + space);

			mInstructBox.Height = Height - (mInstructBox.Top + cbxOpcode.Height + cbxOperandType.Height + txtOperand.Height + instructionEditor.Height + 5 * space);
			btnNewOpCode.Top = mInstructBox.Bottom + space;
			cbxOpcode.Top = btnNewOpCode.Top;
			cbxOpcode.Width = Width - (2 * space + labelspace + btnCancel.Width);

			lblOperandType.Top = btnNewOpCode.Bottom + space;
			cbxOperandType.Top = lblOperandType.Top;
			cbxOperandType.Width = Width - (2 * space + labelspace + btnCancel.Width);

			lblOperand.Top = lblOperandType.Bottom + space;
			foreach (Control c in OperandCList)
				if (c.Visible)
				{
					c.Top = lblOperand.Top;
					c.Left = labelspace;
					c.Width = Width - (2 * space + labelspace + btnCancel.Width);
					c.Height = 21;
					break;
				}

			lblDnD.Top = lblOperand.Bottom + space;
			instructionEditor.Top = lblDnD.Top;
			instructionEditor.Width = Width - (4 * space + labelspace + btnCancel.Width + lblDelete.Width + chbDelete.Width);
			lblDelete.Top = lblDnD.Top;
			lblDelete.Left = instructionEditor.Right + space;
			chbDelete.Top = lblDnD.Top;
			chbDelete.Left = lblDelete.Right + space;

			btnCancel.Top = btnNewOpCode.Top;
			btnCancel.Left = Width - (btnCancel.Width + space);
			btnDone.Top = btnCancel.Bottom + space;
			btnDone.Left = Width - (btnDone.Width + space);
			btnDebug.Top = btnDone.Bottom + space;
			btnDebug.Left = Width - (btnDebug.Width + space);

			instructionEditor.Invalidate();
		}

		// DRAG N DROP *******************************************************

		void mInstructBox_OnItemDropFailed(DragItem[] di)
		{
			OpCodeTableItem octi = di[0] as OpCodeTableItem;
			if (octi == null)
			{
				Log.Write(Log.Level.Careful, "Not OCTI Type Element in List");
				return;
			}
			mInstructBox.Items.Insert(octi.dragFrom, octi);
		}

		void instructionEditor_OnItemDrop()
		{
			instructionEditor.AllowDrag = false;
			loadInstructionInfo();
		}

		void mInstructBox_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				OpCodeTableItem octi = mInstructBox.SelectedElement as OpCodeTableItem;
				if (octi == null) return;
				contextMenuStrip1.Show(Cursor.Position);
			}
			else
			{
				instructionEditor.DragItem = mInstructBox.SelectedElement;
				instructionEditor_OnItemDrop();
			}
		}

		private void MakeItemAvailable()
		{
			if (instructionEditor.DragItem == null)
			{
				InstructionInfo nII = new InstructionInfo();
				nII.OldInstructionNum = -1;
				nII.NewInstructionNum = -1;
				nII.NewInstruction = ILManager.GenInstruction(OpCodes.Nop, null);
				instructionEditor.DragItem = new OpCodeTableItem(mInstructBox, nII);
			}
		}

		// LOAD METHODS ******************************************************

		public void LoadPatchAction(PatchActionILMethodFixed loadpa)
		{
			mInstructBox.ClearItems();

			PatchAction = loadpa;
			MetDef = loadpa.MethodDef;
			txtPatchActionName.Text = loadpa.ActionName;

			if (loadpa.instructPatchList == null) { Log.Write(Log.Level.Error, "PatchAction ", loadpa.ActionName, " is not initialized correctly"); return; }

			PatchAction.instructPatchList.ForEach((instr) => { mInstructBox.AddItem(new OpCodeTableItem(mInstructBox, instr)); });
		}

		public void LoadMetDef(MethodDefinition MetDef)
		{
			mInstructBox.ClearItems();

			PatchAction = null;

			txtMethodFullName.Text = MetDef.FullName;
			this.MetDef = MetDef;
			for (int i = 0; i < MetDef.Body.Instructions.Count; i++)
			{
				InstructionInfo nII = new InstructionInfo();
				nII.OldInstruction = MetDef.Body.Instructions[i];
				nII.NewInstruction = nII.OldInstruction.Clone();
				nII.OldInstructionNum = i;
				nII.NewInstructionNum = i;
				mInstructBox.AddItem(new OpCodeTableItem(mInstructBox, nII));
			}
		}

		// OPERAND TOOLS *****************************************************

		private void loadInstructionInfo()
		{
			OpCodeTableItem octi = instructionEditor.DragItem as OpCodeTableItem;
			if (octi == null) return;
			BoxSetHelper(octi.II.Delete);
			cbxOpcode.Text = octi.II.NewInstruction.OpCode.Name;
		}

		private void UpdateDragItem()
		{
			MakeItemAvailable();
			OpCodeTableItem ocp = instructionEditor.DragItem as OpCodeTableItem;
			string lowtxt = cbxOpcode.Text.ToLower();
			if (ILManager.OpCodeLookup.ContainsKey(lowtxt))
				ocp.II.NewInstruction.OpCode = ILManager.OpCodeLookup[lowtxt];
			else
				ocp.II.NewInstruction.OpCode = OpCodes.Nop;

			instructionEditor.Invalidate();
			mInstructBox.InvalidateChildren();
		}

		private void comboBox3_ValueChanged(object sender, EventArgs e)
		{
			UpdateDragItem();
		}

		private void ShowInput(InputType inpt)
		{
			foreach (Control c in OperandCList)
				c.Visible = false;
			switch (inpt)
			{
			case InputType.None:
				break;
			case InputType.Box:
				txtOperand.Visible = true;
				break;
			case InputType.IntructList:
				lblwip.Visible = true;
				break;
			case InputType.VarList:
				lblwip.Visible = true;
				break;
			case InputType.ParamList:
				lblwip.Visible = true;
				break;
			case InputType.FieldList:
				lblwip.Visible = true;
				break;
			case InputType.MethodList:
				lblwip.Visible = true;
				break;
			case InputType.TypeList:
				lblwip.Visible = true;
				break;
			default:
				lblwip.Visible = true;
				break;
			}
			Controls_Reorganize();
		}

	}

	public enum PickOperandType
	{
		None,
		Byte,
		SByte,
		Int32,
		Int64,
		Single,
		Double,
		String,
		InstructionReference,
		VariableReference,
		ParameterReference,
		FieldReference,
		MethodReference,
		TypeReference,
	}

	public enum InputType
	{
		None,
		Box,
		IntructList,
		VarList,
		ParamList,
		FieldList,
		MethodList,
		TypeList,
	}
}
