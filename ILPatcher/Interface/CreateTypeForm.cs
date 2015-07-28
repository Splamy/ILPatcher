using ILPatcher.Data;
using ILPatcher.Utility;
using Mono.Cecil;
using System;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace ILPatcher.Interface
{
	partial class CreateTypeForm : Form
	{
		private Action<TypeReference> callback;
		public TypeReference createTypeDefinition { get; protected set; }
		DataStruct dataStruct;
		CSCompiler cSCompiler;
		string CapsuleVarNamespaces = @"using System;";
		string CapsuleVarStart = @"namespace A{class B{";
		string CapsuleVarEnd = @" C(){throw new System.Exception();}}}";

		#region Interface Elements
		FastColoredTextBox txtTypeCompile;
		TextBox txtTypeName;
		#endregion

		public CreateTypeForm(DataStruct pDataStruct, Action<TypeReference> pCallback)
		{
			dataStruct = pDataStruct;
			callback = pCallback;

			InitializeGridLineManager();
			Size = new System.Drawing.Size(500, 130);
		}

		private void InitializeGridLineManager()
		{
			Text = "TypeSelector";
			txtTypeCompile = new FastColoredTextBox();
			txtTypeCompile.Multiline = false;
			txtTypeCompile.ShowLineNumbers = false;
			txtTypeCompile.Language = Language.CSharp;
			txtTypeName = new TextBox();
			txtTypeName.ReadOnly = true;

			var grid = new GridLineManager(this, true);
			int line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroLabel("TypeCompiler"), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, txtTypeCompile, GlobalLayout.MinFill);
			grid.AddElementStretchable(line, GlobalLayout.GenMetroButton("Compile", TypeCompiler_Click), GlobalLayout.MinFill, GlobalLayout.LabelWidth);
			line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroLabel("Type"), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, txtTypeName, GlobalLayout.MinFill);
			grid.AddElementStretchable(line, GlobalLayout.GenMetroButton("Pick", PickMethod_Click), GlobalLayout.MinFill, GlobalLayout.LabelWidth);
			line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFilling(line, null, 0);
			grid.AddElementFixed(line, GlobalLayout.GenMetroButton("OK", OK_Click), GlobalLayout.LabelWidth);
		}

		private void PickMethod_Click(object sender, EventArgs e)
		{
			MultiPicker<TypeReference>.ShowStructure(dataStruct, StructureView.classes, x => true, SetTypeReference);
		}

		private void OK_Click(object sender, EventArgs e)
		{
			callback(createTypeDefinition);
			Close();
		}

		private void TypeCompiler_Click(object sender, EventArgs e)
		{
			string compileVar = CapsuleVarNamespaces
								+ CapsuleVarStart
								+ txtTypeCompile.Text
								+ CapsuleVarEnd;
			if (cSCompiler == null)
				cSCompiler = new CSCompiler(dataStruct.AssemblyDefinition);
			cSCompiler.Code = compileVar;
			MethodDefinition methodDefiniton = cSCompiler.GetMethodDefinition("B", "C");
			if (methodDefiniton == null)
				return;
			SetTypeReference(methodDefiniton.ReturnType);
		}

		public void SetTypeReference(TypeReference pTypeReference)
		{
			createTypeDefinition = pTypeReference;
			txtTypeName.Text = CecilFormatter.Format(createTypeDefinition);
		}
	}
}
