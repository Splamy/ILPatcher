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
	public partial class EditorMethodCreator : UserControl
	{
		public PatchActionMethodCreator Patchaction { get; set; }

		private MethodDefinition EmptyMethod;

		private Action<PatchAction> callbackAdd;

		private AssemblyDefinition AssDef;

		//https://github.com/PavelTorgashov/FastColoredTextBox

		public EditorMethodCreator(Action<PatchAction> _cbAdd)
		{
			InitializeComponent();

			callbackAdd = _cbAdd;
		}

		private void btnPickMethod_Click(object sender, EventArgs e)
		{

		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Patchaction = new PatchActionMethodCreator();
			Patchaction.ActionName = "jknfg jasfjn;sadj ao sdf;j dfg ioafjldf giaglkasfgioj adfdfgjn sddfgj af h";
			Patchaction.FillAction = new PatchActionILMethodFixed();
			Patchaction.FillAction.ActionName = "o;ia o;aoia adfgio jad;g dfoi gkjdfhg ihfguh dkjas iuhag dfg isdfg hb";
			callbackAdd(Patchaction);
			((SwooshPanel)Parent).SwooshBack();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			CSCompiler csc = new CSCompiler(null);
			MethodDefinition md = csc.InjectCode(txtInjectCode.Text);

		}

		public void SetAssDef(AssemblyDefinition MyAssDef)
		{
			AssDef = MyAssDef;
		}

		private void button2_Click(object sender, EventArgs e)
		{

		}
	}
}
