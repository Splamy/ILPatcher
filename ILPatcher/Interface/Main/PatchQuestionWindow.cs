using System;
using System.Windows.Forms;

namespace ILPatcher.Interface.General
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
