using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Mono.Cecil;

namespace ILPatcher
{
	public partial class EditorMethodCreator : EditorPatchAction
	{
		public override string PanelName { get { return "PatchAction: ILMethodFixed"; } }

		private PatchActionMethodCreator patchAction;
		private MethodDefinition blankMethodDefinition;
		private AssemblyDefinition assemblyDefinition;

		//https://github.com/PavelTorgashov/FastColoredTextBox

		public EditorMethodCreator(Action<PatchAction> pParentAddCallback)
			: base(pParentAddCallback)
		{
			InitializeComponent();
		}

		private void btnPickMethod_Click(object sender, EventArgs e)
		{

		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			patchAction = new PatchActionMethodCreator();
			patchAction.ActionName = "jknfg jasfjn;sadj ao sdf;j dfg ioafjldf giaglkasfgioj adfdfgjn sddfgj af h";
			patchAction.FillAction = new PatchActionILMethodFixed();
			patchAction.FillAction.ActionName = "o;ia o;aoia adfgio jad;g dfoi gkjdfhg ihfguh dkjas iuhag dfg isdfg hb";
			ParentAddCallback(patchAction);
			((SwooshPanel)Parent).SwooshBack();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//CSCompiler csc = new CSCompiler(assemblyDefinition);
			//MethodDefinition md = csc.InjectCode(txtInjectCode.Text);
		}

		public void SetAssDef(AssemblyDefinition myAssDef)
		{
			assemblyDefinition = myAssDef;
		}

		public override void SetPatchAction(PatchAction pPatchAction)
		{
			PatchActionMethodCreator pamc = (PatchActionMethodCreator)pPatchAction;
			patchAction = pamc;
		}

		private void button2_Click(object sender, EventArgs e)
		{

		}
	}
}
