using ILPatcher.Data;
using ILPatcher.Data.Finder;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace ILPatcher.Interface.Finder
{
	[EditorAttributes("Class-Finder")]
	class EditorFinderClassByName : EditorTargetFinder<TargetFinderClassByName>
	{
		private int ilNodePathLevel = -1;

		public override bool FixedHeight => true;
		public override int DefaultHeight => 50;

		#region Interface Elements
		TextBox txtClassPath;
		#endregion

		public EditorFinderClassByName(DataStruct dataStruct) : base(dataStruct)
		{
			InitializeGridLineManager();
		}

		private void InitializeGridLineManager()
		{
			txtClassPath = new TextBox();
			txtClassPath.AutoCompleteMode = AutoCompleteMode.Suggest;
			txtClassPath.AutoCompleteSource = AutoCompleteSource.CustomSource;
			txtClassPath.TextChanged += TxtClassPath_TextChanged;

			var grid = new GridLineManager(this, true);
			int line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroLabel("ILNodePath"), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, txtClassPath, GlobalLayout.MinFill);
		}

		private void TxtClassPath_TextChanged(object sender, System.EventArgs e) // TODO: test,... and improve
		{
			string txtVaue = txtClassPath.Text;
			myData.ILNodePath = txtVaue;
			string[] pathParts = txtVaue.Split(new[] { '.', '/' });
			if (pathParts.Length == ilNodePathLevel) return;

			int okTextIndex = txtVaue.LastIndexOfAny(ILNodeManager.Seperators); // check stuff...
			string okText = txtVaue.Substring(0, okTextIndex + 1);
			var source = new AutoCompleteStringCollection();

			ICollection<ILNode> sourceCollection;
			ilNodePathLevel = pathParts.Length;
			if (ilNodePathLevel <= 1) // module
			{
				sourceCollection = dataStruct.ILNodeManager.AllModules;
			}
			else // rest
			{
				sourceCollection = dataStruct.ILNodeManager.FindNodeByPath(txtVaue.Substring(0, okTextIndex))?.Children; // crashing here cause okTextIndex was -1
			}

			if (sourceCollection != null)
			{
				if (ilNodePathLevel == 2)
					source.AddRange(sourceCollection.Select(node => okText + node.Name.Replace('.', ':')).ToArray()); // HACK: improve
				else
					source.AddRange(sourceCollection.Select(node => okText + node.Name).ToArray());
			}
			if (source.Count > 0)
				txtClassPath.AutoCompleteCustomSource = source;

		}

		protected override void OnPatchDataSet()
		{
			txtClassPath.Text = myData.ILNodePath;
		}
	}
}
