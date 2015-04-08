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

		private Action<PatchAction> callbackAdd;

		private AssemblyDefinition AssDef;

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
			MethodReference md = csc.InjectCode(textBox1.Text);

		}

		public void SetAssDef(AssemblyDefinition MyAssDef)
		{
			AssDef = MyAssDef;
		}
	}
}
