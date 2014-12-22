using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ILPatcher
{
	public partial class PatchQuestionWindow : Form
	{
		public PatchQuestionWindow()
		{
			InitializeComponent();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnMerge_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.No;
			Close();
		}

		private void btnFresh_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Yes;
			Close();
		}
	}
}
