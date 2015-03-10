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

		private Control[] OperandCList;
		private PickOperandType currentPOT = PickOperandType.None;

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
			instructionEditor.OnItemDropSuccess += instructionEditor_OnItemDropSuccess;
			mInstructBox.OnItemDropFailed += mInstructBox_OnItemDropFailed;
			mInstructBox.OnItemDropSuccess += mInstructBox_OnItemDropSuccess;
			OperandCList = new Control[] { txtOperand, cbxOperand, lblwip, panTypePicker };

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
				RefershInstructionList();
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
			MultiPicker.ShowStructure(StructureView.classes | StructureView.functions, x => x is MethodDefinition, x => LoadMetDef((MethodDefinition)x), true);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			// TODO ^^
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

			mInstructBox.Height = Height - (mInstructBox.Top + cbxOpcode.Height + txtOperand.Height + instructionEditor.Height + 5 * space);
			btnNewOpCode.Top = mInstructBox.Bottom + space;
			cbxOpcode.Top = btnNewOpCode.Top;
			cbxOpcode.Width = (Width - (3 * space + labelspace + btnCancel.Width)) / 2;

			lblOperandType.Top = btnNewOpCode.Top;
			lblOperandType.Width = cbxOpcode.Width;
			lblOperandType.Left = cbxOpcode.Right + space;

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

			instructionEditor.Invalidate();
		}

		private void RedrawBoth()
		{
			instructionEditor.Invalidate();
			mInstructBox.InvalidateChildren();
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
			if (octi.dragFrom == -1)
			{
				Log.Write(Log.Level.Warning, "OCTI Drag start was saved wrongly");
				mInstructBox.Items.Insert(0, octi);
			}
			else
				mInstructBox.Items.Insert(octi.dragFrom, octi);
			RefershInstructionList();
		}

		void mInstructBox_OnItemDropSuccess(DragItem[] di)
		{
			RefershInstructionList();
		}

		void instructionEditor_OnItemDropSuccess(DragItem[] di)
		{
			instructionEditor.AllowDrag = false;
			ReadFromDragItem();
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
				instructionEditor_OnItemDropSuccess(null);
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

		private void RefershInstructionList()
		{
			int pos = 0;
			foreach (DragItem di in mInstructBox.Items)
			{
				InstructionInfo II = (di as OpCodeTableItem).II;
				if (II.Delete)
					II.NewInstructionNum = -1;
				else
					II.NewInstructionNum = pos++;
			}
			mInstructBox.InvalidateChildren();
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
			// copy all old instructions to the new, so we don't modify the original method
			for (int i = 0; i < MetDef.Body.Instructions.Count; i++)
			{
				InstructionInfo nII = new InstructionInfo();
				nII.OldInstruction = MetDef.Body.Instructions[i];
				nII.NewInstruction = nII.OldInstruction.Clone();
				nII.OldInstructionNum = i;
				nII.NewInstructionNum = i;
				mInstructBox.AddItem(new OpCodeTableItem(mInstructBox, nII));
			}
			// now update all jumps, since they still point to the old list
			for (int i = 0; i < mInstructBox.Items.Count; i++)
			{
				OpCodeTableItem octi = (OpCodeTableItem)mInstructBox.Items[i];
				InstructionInfo nII = octi.II;

				if (nII.OldInstruction.OpCode.OperandType == OperandType.InlineBrTarget ||
					nII.OldInstruction.OpCode.OperandType == OperandType.ShortInlineBrTarget)
				{
					nII.NewInstruction.Operand = ((OpCodeTableItem)mInstructBox.Items[MetDef.Body.Instructions.IndexOf((Instruction)nII.OldInstruction.Operand)]).II.NewInstruction;
				}
				else if (nII.OldInstruction.OpCode.OperandType == OperandType.InlineSwitch)
				{
					Instruction[] tmpold = (Instruction[])nII.OldInstruction.Operand;
					Instruction[] tmpnew = new Instruction[tmpold.Length];
					for (int j = 0; j < tmpold.Length; j++)
						tmpnew[j] = ((OpCodeTableItem)mInstructBox.Items[MetDef.Body.Instructions.IndexOf(tmpold[i])]).II.NewInstruction;
					nII.NewInstruction.Operand = tmpnew;
				}
			}
		}

		// OPERAND TOOLS *****************************************************

		private void ReadFromDragItem()
		{
			OpCodeTableItem octi = instructionEditor.DragItem as OpCodeTableItem;
			if (octi == null) return;
			BoxSetHelper(octi.II.Delete);
			cbxOpcode.Text = octi.II.NewInstruction.OpCode.Name;
			OpCodeToInputType(octi.II.NewInstruction.OpCode);
		}

		private void WriteToDragItem()
		{
			OpCodeTableItem ocp = (OpCodeTableItem)instructionEditor.DragItem;
			string lowtxt = cbxOpcode.Text.ToLower();
			if (ILManager.OpCodeLookup.ContainsKey(lowtxt))
				ocp.II.NewInstruction.OpCode = ILManager.OpCodeLookup[lowtxt];
			else
				ocp.II.NewInstruction.OpCode = OpCodes.Nop;

			RedrawBoth();
		}

		private void OpCodeToInputType(OpCode opc)
		{
			#region Read PickOperandType
			switch (opc.OperandType)
			{
			case OperandType.InlineArg:
			case OperandType.ShortInlineArg:
				currentPOT = PickOperandType.ParameterReference;
				break;
			case OperandType.InlineBrTarget:
			case OperandType.ShortInlineBrTarget:
				currentPOT = PickOperandType.InstructionReference;
				break;
			case OperandType.InlineField:
				currentPOT = PickOperandType.FieldReference;
				break;
			case OperandType.InlineI:
				currentPOT = PickOperandType.Int32;
				break;
			case OperandType.InlineI8:
				currentPOT = PickOperandType.Int64;
				break;
			case OperandType.InlineMethod:
				currentPOT = PickOperandType.MethodReference;
				break;
			case OperandType.InlineNone:
				currentPOT = PickOperandType.None;
				break;
			case OperandType.InlineR:
				currentPOT = PickOperandType.Double;
				break;
			case OperandType.InlineString:
				currentPOT = PickOperandType.String;
				break;
			case OperandType.InlineSwitch:
				currentPOT = PickOperandType.InstructionArrReference;
				break;
			case OperandType.InlineTok:
				currentPOT = PickOperandType.TMFReferenceDynamic;
				break;
			case OperandType.InlineType:
				currentPOT = PickOperandType.TypeReference;
				break;
			case OperandType.InlineVar:
			case OperandType.ShortInlineVar:
				currentPOT = PickOperandType.VariableReference;
				break;
			case OperandType.ShortInlineI:
				if (opc == OpCodes.Ldc_I4_S)
					currentPOT = PickOperandType.Byte;
				else
					currentPOT = PickOperandType.SByte;
				break;
			case OperandType.ShortInlineR:
				currentPOT = PickOperandType.Single;
				break;
			case OperandType.InlineSig:
			case OperandType.InlinePhi:
			default:
				Log.Write(Log.Level.Warning, "Not switced OperandType: ", opc.Name);
				break;
			}
			#endregion

			#region Show the corresponding control
			foreach (Control c in OperandCList)
				c.Visible = false;
			switch (currentPOT)
			{
			case PickOperandType.None:
				break;
			case PickOperandType.Byte:
			case PickOperandType.SByte:
			case PickOperandType.Int32:
			case PickOperandType.Int64:
			case PickOperandType.Single:
			case PickOperandType.Double:
			case PickOperandType.String:
				txtOperand.Visible = true; // done
				break;
			case PickOperandType.InstructionReference:
				lblwip.Visible = true;
				break;
			case PickOperandType.VariableReference:
				lblwip.Visible = true;
				break;
			case PickOperandType.ParameterReference:
				lblwip.Visible = true;
				break;
			case PickOperandType.FieldReference:
				lblwip.Visible = true;
				break;
			case PickOperandType.MethodReference:
				lblwip.Visible = true;
				break;
			case PickOperandType.TypeReference:
				panTypePicker.Visible = true; // done
				break;
			default:
				lblwip.Visible = true;
				Log.Write(Log.Level.Warning, "Not switced PickOperadType in combobox: ", currentPOT.ToString());
				break;
			}
			#endregion

			lblOperandType.Text = currentPOT.ToString(); // TODO: read from dict
			MakeItemAvailable();
			Controls_Reorganize();
		}

		private void cbxOpcode_ValueChanged(object sender, EventArgs e)
		{
			MakeItemAvailable();
			OpCodeToInputType(((OpCodeTableItem)instructionEditor.DragItem).II.NewInstruction.OpCode);
			WriteToDragItem();
		}

		private void txtOperand_TextChanged(object sender, EventArgs e)
		{
			RefreshTextbox2Operand();
		}
		private void RefreshTextbox2Operand()
		{
			OpCodeTableItem ocp = (OpCodeTableItem)instructionEditor.DragItem;

			switch ((PickOperandType)currentPOT)
			{
			case PickOperandType.Byte:
				Byte resultByte;
				if (Byte.TryParse(txtOperand.Text, out resultByte))
					ocp.II.NewInstruction.Operand = resultByte;
				break;
			case PickOperandType.SByte:
				SByte resultSByte;
				if (SByte.TryParse(txtOperand.Text, out resultSByte))
					ocp.II.NewInstruction.Operand = resultSByte;
				break;
			case PickOperandType.Int32:
				Int32 resultInt32;
				if (Int32.TryParse(txtOperand.Text, out resultInt32))
					ocp.II.NewInstruction.Operand = resultInt32;
				break;
			case PickOperandType.Int64:
				Int64 resultInt64;
				if (Int64.TryParse(txtOperand.Text, out resultInt64))
					ocp.II.NewInstruction.Operand = resultInt64;
				break;
			case PickOperandType.Single:
				Single resultSingle;
				if (Single.TryParse(txtOperand.Text, out resultSingle))
					ocp.II.NewInstruction.Operand = resultSingle;
				break;
			case PickOperandType.Double:
				Double resultDouble;
				if (Double.TryParse(txtOperand.Text, out resultDouble))
					ocp.II.NewInstruction.Operand = resultDouble;
				break;
			case PickOperandType.String:
				ocp.II.NewInstruction.Operand = txtOperand.Text;
				break;
			default:
				Log.Write(Log.Level.Warning, "OperandType cannot be processed with a textbox");
				break;
			}

			RedrawBoth();
		}

		private void btnTypePicker_Click(object sender, EventArgs e)
		{
			MultiPicker.ShowStructure(StructureView.classes, x => x is TypeReference, x => ApplyOperand((TypeReference)x));
		}
		private void ApplyOperand(TypeReference tr)
		{
			// TypeDefinition > TypeReference, so this should work
			lblTypePicker.Text = tr.FullName;
			((OpCodeTableItem)instructionEditor.DragItem).II.NewInstruction.Operand = tr;
			RedrawBoth();
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
		InstructionArrReference,
		VariableReference,
		ParameterReference,
		FieldReference,
		MethodReference,
		TypeReference,
		TMFReferenceDynamic,
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
