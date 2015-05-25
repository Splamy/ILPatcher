using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	partial class CreateTypeForm : Form
	{
		private Action<TypeReference> callback;
		public TypeReference createTypeDefinition { get; protected set; }
		AssemblyDefinition assemblyDefinition;
		CSCompiler cSCompiler;
		string CapsuleVarNamespaces = @"using System.Windows.Forms;";
		string CapsuleVarStart = @"namespace A{class B{";
		string CapsuleVarEnd = @" C(){throw new System.Exception();}}}";

		public CreateTypeForm(AssemblyDefinition pAssemblyDefinition, Action<TypeReference> pCallback)
		{
			InitializeComponent();

			assemblyDefinition = pAssemblyDefinition;
			callback = pCallback;
		}

		private void btnPickMethod_Click(object sender, EventArgs e)
		{
			MultiPicker.Instance.ShowStructure(StructureView.classes, x => x is TypeReference, x => SetTypeReference((TypeReference)x));
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
				cSCompiler = new CSCompiler(assemblyDefinition);
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
