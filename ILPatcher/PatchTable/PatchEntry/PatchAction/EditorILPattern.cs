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
		private MethodDefinition MetDef;

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
			OperandCList = new Control[] { txtOperand, cbxOperand, panTMFPicker };

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
				mInstructBox.SelectedItems.ForEach(x => ((InstructionInfo)x).Delete = value);
				RefershInstructionList();
			}
		}

		private void btnDone_Click(object sender, EventArgs e)
		{
			if (PatchAction == null)
			{
				PatchAction = new PatchActionILMethodFixed();
				PatchAction.SetInitWorking();

				PatchAction.instructPatchList = mInstructBox.Items.ConvertAll<InstructionInfo>(x => (InstructionInfo)x);
				PatchAction.MethodDef = MetDef;
			}

			PatchAction.ActionName = txtPatchActionName.Text;

			EditorEntry.Instance.Add(PatchAction);
			((SwooshPanel)Parent).SwooshTo(EditorEntry.Instance);
		}

		private void btnPickMethod_Click(object sender, EventArgs e)
		{
			MultiPicker.Instance.ShowStructure(StructureView.methods, x => x is MethodDefinition, x => LoadMetDef((MethodDefinition)x), true);
		}

		private void btnNewOpCode_Click(object sender, EventArgs e)
		{
			instructionEditor.DragItem = null;
			instructionEditor.AllowDrag = true;
			cbxOpcode.Text = string.Empty;
			MakeItemAvailable();
		}

		private void tsmRemove_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Do you really want to remove all selected Instructions?\nPlease keep in mind, that only newly created Instructions will be removed,\nold Instructions must be removed with the delete-flag for a patch file.", "Delete confirmation", MessageBoxButtons.OKCancel) != DialogResult.OK) return;
			List<InstructionInfo> remlist = mInstructBox.SelectedItems.ConvertAll<InstructionInfo>(x => (InstructionInfo)x);
			foreach (InstructionInfo II in remlist)
				if (II.IsNew)
					mInstructBox.RemoveItem(II);
		}

		private void tsmDelete_Click(object sender, EventArgs e)
		{
			mInstructBox.SelectedItems.ForEach(x => ((InstructionInfo)x).Delete = true);
		}

		private void tsmUnDelete_Click(object sender, EventArgs e)
		{
			mInstructBox.SelectedItems.ForEach(x => ((InstructionInfo)x).Delete = false);
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
			cbxOpcode.Width = (Width - (3 * space + labelspace + btnDone.Width)) / 2;

			lblOperandType.Top = btnNewOpCode.Top;
			lblOperandType.Width = cbxOpcode.Width;
			lblOperandType.Left = cbxOpcode.Right + space;

			lblOperand.Top = lblOperandType.Bottom + space;
			foreach (Control c in OperandCList)
				if (c.Visible)
				{
					c.Top = lblOperand.Top;
					c.Left = labelspace;
					c.Width = Width - (2 * space + labelspace + btnDone.Width);
					c.Height = 21;
					break;
				}

			lblDnD.Top = lblOperand.Bottom + space;
			instructionEditor.Top = lblDnD.Top;
			instructionEditor.Width = Width - (4 * space + labelspace + btnDone.Width + lblDelete.Width + chbDelete.Width);
			lblDelete.Top = lblDnD.Top;
			lblDelete.Left = instructionEditor.Right + space;
			chbDelete.Top = lblDnD.Top;
			chbDelete.Left = lblDelete.Right + space;

			btnDone.Top = btnNewOpCode.Top;
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
			InstructionInfo II = di[0] as InstructionInfo;
			if (II == null)
			{
				Log.Write(Log.Level.Careful, "Not OCTI Type Element in List");
				return;
			}
			if (II.dragFrom == -1)
			{
				Log.Write(Log.Level.Warning, "OCTI Drag start was saved wrongly");
				mInstructBox.Items.Insert(0, II);
			}
			else
				mInstructBox.Items.Insert(II.dragFrom, II);
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
				InstructionInfo II = mInstructBox.SelectedElement as InstructionInfo;
				if (II == null) return;
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
				instructionEditor.DragItem = nII;
			}
		}

		private void RefershInstructionList()
		{
			int pos = 0;
			foreach (DragItem di in mInstructBox.Items)
			{
				InstructionInfo II = di as InstructionInfo;
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

			mInstructBox.Items = PatchAction.instructPatchList.ConvertAll<DragItem>(x => (DragItem)x);
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
				mInstructBox.AddItem(nII);
			}
			// now update all jumps, since they still point to the old list
			for (int i = 0; i < mInstructBox.Items.Count; i++)
			{
				InstructionInfo nII = (InstructionInfo)mInstructBox.Items[i];

				if (nII.OldInstruction.OpCode.OperandType == OperandType.InlineBrTarget ||
					nII.OldInstruction.OpCode.OperandType == OperandType.ShortInlineBrTarget)
				{
					nII.NewInstruction.Operand = ((InstructionInfo)mInstructBox.Items[MetDef.Body.Instructions.IndexOf((Instruction)nII.OldInstruction.Operand)]).NewInstruction;
				}
				else if (nII.OldInstruction.OpCode.OperandType == OperandType.InlineSwitch)
				{
					Instruction[] tmpold = (Instruction[])nII.OldInstruction.Operand;
					Instruction[] tmpnew = new Instruction[tmpold.Length];
					for (int j = 0; j < tmpold.Length; j++)
						tmpnew[j] = ((InstructionInfo)mInstructBox.Items[MetDef.Body.Instructions.IndexOf(tmpold[j])]).NewInstruction;
					nII.NewInstruction.Operand = tmpnew;
				}
			}
		}

		// OPERAND TOOLS *****************************************************

		private void ReadFromDragItem()
		{
			InstructionInfo II = instructionEditor.DragItem as InstructionInfo;
			if (II == null) return;
			BoxSetHelper(II.Delete);
			cbxOpcode.Text = II.NewInstruction.OpCode.Name;
			OpCodeToInputType(II.NewInstruction, true);
		}

		private void WriteToDragItem()
		{
			InstructionInfo II = (InstructionInfo)instructionEditor.DragItem;
			if (II == null) return;
			string lowtxt = cbxOpcode.Text.ToLower();
			if (ILManager.OpCodeLookup.ContainsKey(lowtxt))
				II.NewInstruction.OpCode = ILManager.OpCodeLookup[lowtxt];
			else
				II.NewInstruction.OpCode = OpCodes.Nop;

			RedrawBoth();
		}

		private void OpCodeToInputType(Instruction instr, bool readOperand)
		{
			#region Read PickOperandType
			switch (instr.OpCode.OperandType)
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
				if (instr.OpCode == OpCodes.Ldc_I4_S)
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
				Log.Write(Log.Level.Warning, "Not switced OperandType: ", instr.OpCode.Name);
				break;
			}
			#endregion

			#region Show the corresponding control
			foreach (Control c in OperandCList)
				c.Visible = false;
			switch (currentPOT)
			{
			case PickOperandType.Byte:
			case PickOperandType.SByte:
			case PickOperandType.Int32:
			case PickOperandType.Int64:
			case PickOperandType.Single:
			case PickOperandType.Double:
			case PickOperandType.String:
				if (readOperand)
					txtOperand.Text = instr.Operand.ToString();
				txtOperand.Visible = true;
				break;
			case PickOperandType.InstructionReference:
				InitCbxOperand();
				if (readOperand)
				{
					Instruction pr = instr.Operand as Instruction;
					if (pr != null)
						cbxOperand.SelectedIndex = mInstructBox.Items.FindIndex(x => ((InstructionInfo)x).NewInstruction == pr);
				}
				cbxOperand.Visible = true;
				break;
			case PickOperandType.InstructionArrReference:
				lblTMFPicker.Text = "<Instruction Array>";
				panTMFPicker.Visible = true;
				break;
			case PickOperandType.VariableReference:
				InitCbxOperand();
				if (readOperand)
				{
					ParameterReference pr = instr.Operand as ParameterReference;
					if (pr != null)
						cbxOperand.SelectedIndex = pr.Index;
				}
				cbxOperand.Visible = true;
				break;
			case PickOperandType.ParameterReference:
				InitCbxOperand();
				if (readOperand)
				{
					VariableReference vr = instr.Operand as VariableReference;
					if (vr != null)
						cbxOperand.SelectedIndex = vr.Index;
				}
				cbxOperand.Visible = true;
				break;
			case PickOperandType.None:
			case PickOperandType.FieldReference:
			case PickOperandType.MethodReference:
			case PickOperandType.TypeReference:
			case PickOperandType.TMFReferenceDynamic:
				if (readOperand)
					lblTMFPicker.Text = CecilFormatter.TryFormat(instr.Operand);

				btnTMFPicker.Text = currentPOT == PickOperandType.None ? "Clear" : "Pick";
				panTMFPicker.Visible = true;
				break;
			default:
				Log.Write(Log.Level.Warning, "Not switced PickOperadType: ", currentPOT.ToString());
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
			OpCodeToInputType(((InstructionInfo)instructionEditor.DragItem).NewInstruction, false);
			WriteToDragItem();
		}

		private void txtOperand_TextChanged(object sender, EventArgs e)
		{
			ApplyOperand(txtOperand.Text);
		}
		private void ApplyOperand(string str)
		{
			InstructionInfo II = (InstructionInfo)instructionEditor.DragItem;

			switch (currentPOT)
			{
			case PickOperandType.Byte:
				Byte resultByte;
				if (Byte.TryParse(str, out resultByte))
					II.NewInstruction.Operand = resultByte;
				break;
			case PickOperandType.SByte:
				SByte resultSByte;
				if (SByte.TryParse(str, out resultSByte))
					II.NewInstruction.Operand = resultSByte;
				break;
			case PickOperandType.Int32:
				Int32 resultInt32;
				if (Int32.TryParse(str, out resultInt32))
					II.NewInstruction.Operand = resultInt32;
				break;
			case PickOperandType.Int64:
				Int64 resultInt64;
				if (Int64.TryParse(str, out resultInt64))
					II.NewInstruction.Operand = resultInt64;
				break;
			case PickOperandType.Single:
				Single resultSingle;
				if (Single.TryParse(str, out resultSingle))
					II.NewInstruction.Operand = resultSingle;
				break;
			case PickOperandType.Double:
				Double resultDouble;
				if (Double.TryParse(str, out resultDouble))
					II.NewInstruction.Operand = resultDouble;
				break;
			case PickOperandType.String:
				II.NewInstruction.Operand = str;
				break;
			default:
				Log.Write(Log.Level.Warning, "OperandType cannot be processed with a textbox: ", currentPOT.ToString());
				break;
			}

			RedrawBoth();
		}

		private void btnTMFPicker_Click(object sender, EventArgs e)
		{
			switch (currentPOT)
			{
			case PickOperandType.None:
				((InstructionInfo)instructionEditor.DragItem).NewInstruction.Operand = null;
				lblTMFPicker.Text = string.Empty;
				RedrawBoth();
				break;
			case PickOperandType.FieldReference:
				MultiPicker.Instance.ShowStructure(StructureView.fields, x => x is FieldReference, x => ApplyOperand((MemberReference)x));
				break;
			case PickOperandType.MethodReference:
				MultiPicker.Instance.ShowStructure(StructureView.methods, x => x is MethodReference, x => ApplyOperand((MemberReference)x));
				break;
			case PickOperandType.TypeReference:
				MultiPicker.Instance.ShowStructure(StructureView.classes, x => x is TypeReference, x => ApplyOperand((MemberReference)x));
				AddGenericsToToolBox();
				break;
			case PickOperandType.TMFReferenceDynamic:
				MultiPicker.Instance.ShowStructure(StructureView.all, x => x is MemberReference, x => ApplyOperand((MemberReference)x));
				AddGenericsToToolBox();
				break;
			case PickOperandType.InstructionArrReference:
				Instruction[] oldop = ((InstructionInfo)instructionEditor.DragItem).NewInstruction.Operand as Instruction[];
				InstructArrPicker.Instance.ShowStructure(mInstructBox.Items, oldop, x => ApplyOperand(x));
				break;
			default:
				Log.Write(Log.Level.Warning, "OperandType cannot be processed with the TMFPicker: ", currentPOT.ToString());
				break;
			}
		}
		private void ApplyOperand(MemberReference tr)
		{
			// TypeDefinition > TypeReference, so this should work
			lblTMFPicker.Text = CecilFormatter.TryFormat(tr);
			((InstructionInfo)instructionEditor.DragItem).NewInstruction.Operand = tr;
			RedrawBoth();
		}
		private void ApplyOperand(Instruction[] iarr)
		{
			((InstructionInfo)instructionEditor.DragItem).NewInstruction.Operand = iarr;
			RedrawBoth();
		}

		private void InitCbxOperand()
		{
			cbxOperand.Items.Clear();
			StringBuilder strb = new StringBuilder();

			switch (currentPOT)
			{
			case PickOperandType.VariableReference:
				CecilFormatter.SetMaxNumer(MetDef.Body.Variables.Count);
				foreach (VariableDefinition vardef in MetDef.Body.Variables)
					cbxOperand.Items.Add(CecilFormatter.Format(vardef));
				break;
			case PickOperandType.ParameterReference:
				CecilFormatter.SetMaxNumer(MetDef.Parameters.Count);
				foreach (ParameterDefinition pardef in MetDef.Parameters)
					cbxOperand.Items.Add(CecilFormatter.Format(pardef));
				break;
			case PickOperandType.InstructionReference:
				CecilFormatter.SetMaxNumer(mInstructBox.Items.Count);
				foreach (InstructionInfo II in mInstructBox.Items)
					cbxOperand.Items.Add(CecilFormatter.Format(II.NewInstruction, II.NewInstructionNum));
				break;
			default:
				Log.Write(Log.Level.Warning, "OperandType cannot be processed with a Combobox: ", currentPOT.ToString());
				break;
			}
			CecilFormatter.ClearMaxNumer();
		}
		private void cbxOperand_SelectedIndexChanged(object sender, EventArgs e)
		{
			InstructionInfo II = (InstructionInfo)instructionEditor.DragItem;

			switch (currentPOT)
			{
			case PickOperandType.VariableReference:
				II.NewInstruction.Operand = MetDef.Body.Variables[cbxOperand.SelectedIndex];
				break;
			case PickOperandType.ParameterReference:
				II.NewInstruction.Operand = MetDef.Parameters[cbxOperand.SelectedIndex];
				break;
			case PickOperandType.InstructionReference:
				II.NewInstruction.Operand = ((InstructionInfo)mInstructBox.Items[cbxOperand.SelectedIndex]).NewInstruction;
				break;
			default:
				Log.Write(Log.Level.Warning, "OperandType cannot be processed with a Combobox: ", currentPOT.ToString());
				break;
			}
			RedrawBoth();
		}

		private void AddGenericsToToolBox()
		{
			ILNode AddToolBoxNode = new ILNode(null, null, null, StructureView.none);
			ILNode GenericExtension = AddToolBoxNode.Add("<Local GenericParameter>", "<Local GenericParameter>", null, StructureView.structure);
			if (MetDef.HasGenericParameters)
				foreach (GenericParameter gpar in MetDef.GenericParameters)
					GenericExtension.Add(gpar.Name, gpar.FullName, gpar, StructureView.classes);
			TypeDefinition recdef = MetDef.DeclaringType;
			while (recdef != null)
			{
				if (recdef.HasGenericParameters)
					foreach (GenericParameter gpar in recdef.GenericParameters)
						GenericExtension.Add(gpar.Name, gpar.FullName, gpar, StructureView.classes);
				if (recdef.IsNested)
					recdef = recdef.DeclaringType;
				else
					recdef = null;
			}
			MultiPicker.Instance.AddToolBoxNode(AddToolBoxNode);
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
