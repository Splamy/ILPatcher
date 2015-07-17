using ILPatcher.Data;
using ILPatcher.Utility;
using Mono.Cecil;
using System;
using System.Windows.Forms;

namespace ILPatcher.Interface.General
{
	partial class CreateTypeForm : Form
	{
		private Action<TypeReference> callback;
		public TypeReference createTypeDefinition { get; protected set; }
		DataStruct dataStruct;
		CSCompiler cSCompiler;
		string CapsuleVarNamespaces = @"using System.Windows.Forms;";
		string CapsuleVarStart = @"namespace A{class B{";
		string CapsuleVarEnd = @" C(){throw new System.Exception();}}}";

		public CreateTypeForm(DataStruct pDataStruct, Action<TypeReference> pCallback)
		{
			InitializeComponent();

			dataStruct = pDataStruct;
			callback = pCallback;
		}

		private void btnPickMethod_Click(object sender, EventArgs e)
		{
			MultiPicker<TypeReference>.ShowStructure(dataStruct, StructureView.classes, x => true, SetTypeReference);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			callback(createTypeDefinition);
			Close();
		}

		private void btnTypeCompiler_Click(object sender, EventArgs e)
		{
			string compileVar = CapsuleVarNamespaces
								+ CapsuleVarStart
								+ txtTypeCompile.Text
								+ CapsuleVarEnd;
			if (cSCompiler == null)
				cSCompiler = new CSCompiler(dataStruct.AssemblyDefinition);
			cSCompiler.Code = compileVar;
			MethodDefinition methodDefiniton = cSCompiler.GetMethodDefinition(string.Empty, string.Empty);
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
