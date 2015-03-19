using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ILPatcher
{
	public partial class EditorMethodCreator : UserControl
	{
		public PatchActionMethodCreator Patchaction { get; set; }

		private Action<PatchAction> callbackAdd;

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
			//callbackAdd(Patchaction);
			((SwooshPanel)Parent).SwooshBack();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			CSCompiler csc = new CSCompiler(null);
			Mono.Cecil.MethodDefinition md = csc.InjectCode(textBox1.Text);
		}
	}
}
