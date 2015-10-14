using ILPatcher.Data;
using ILPatcher.Data.Finder;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace ILPatcher.Interface.Finder
{
	[EditorAttributes("Class-Finder", Inline = true)]
	class EditorFinderClassByName : EditorTargetFinder<TargetFinderClassByName>
	{
		private int ilNodePathLevel = -1;

		#region Interface Elements
		TextBox txtName;
		TextBox txtClassPath;
		#endregion

		public EditorFinderClassByName(DataStruct dataStruct) : base(dataStruct)
		{
			InitializeGridLineManager();
		}

		private void InitializeGridLineManager()
		{
			txtName = new TextBox();
			txtName.TextChanged += TxtName_TextChanged;
			txtClassPath = new TextBox();
			txtClassPath.AutoCompleteMode = AutoCompleteMode.Suggest;
			txtClassPath.AutoCompleteSource = AutoCompleteSource.CustomSource;
			txtClassPath.TextChanged += TxtClassPath_TextChanged;

			var grid = new GridLineManager(this, true);
			int line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroLabel("Name"), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, txtName, GlobalLayout.MinFill);
			line = grid.AddLineFixed(GlobalLayout.LineHeight);
			grid.AddElementFixed(line, GlobalLayout.GenMetroLabel("ILNodePath"), GlobalLayout.LabelWidth);
			grid.AddElementFilling(line, txtClassPath, GlobalLayout.MinFill);

		}

		private void TxtName_TextChanged(object sender, System.EventArgs e)
		{
			myData.Name = txtName.Text;
		}

		private void TxtClassPath_TextChanged(object sender, System.EventArgs e) // TODO: test
		{
			string txtVaue = txtClassPath.Text;
			myData.ILNodePath = txtVaue;
			string[] pathParts = txtVaue.Split(new[] { '.', '/' });
			if (pathParts.Length == ilNodePathLevel) return;

			int okTextIndex = txtVaue.LastIndexOfAny(ILNodeManager.Seperators);
			string okText = txtVaue.Substring(0, okTextIndex + 1);
			var source = new AutoCompleteStringCollection();

			ICollection<ILNode> sourceCollection;
			ilNodePathLevel = pathParts.Length;
			if (ilNodePathLevel <= 1) // module
			{
				sourceCollection = dataStruct.ILNodeManager.GetAllModules();
			}
			else // rest
			{
				sourceCollection = dataStruct.ILNodeManager.FindNodeByPath(txtVaue.Substring(0, okTextIndex))?.Children;
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

		protected override TargetFinder GetNewEntryPart()
		{
			return new TargetFinderClassByName(dataStruct);
		}

		protected override void OnPatchDataSet()
		{
			txtClassPath.Text = myData.ILNodePath;
		}
	}
}
