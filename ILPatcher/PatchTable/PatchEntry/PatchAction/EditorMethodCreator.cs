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

		public EditorMethodCreator()
		{
			InitializeComponent();
		}

		private void btnPickMethod_Click(object sender, EventArgs e)
		{

		}
	}
}
