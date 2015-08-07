using ILPatcher.Data;
using ILPatcher.Data.Finder;
using ILPatcher.Utility;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace ILPatcher.Interface.Finder
{
	class EditorFinderClassByName : EditorTargetFinder<TargetFinderClassByName>
	{
		//I: EditorPanel
		public override string PanelName { get { return "Class-Finder"; } }
		public override bool IsInline { get { return true; } }

		//Own:
		int ilNodePathLevel = -1;

		#region Interface Elements
		TextBox txtClassPath;
		#endregion

		public EditorFinderClassByName(DataStruct dataStruct) : base(dataStruct)
		{

		}

		private void InitializeGridLineManager()
		{
			txtClassPath = new TextBox();
			txtClassPath.AutoCompleteMode = AutoCompleteMode.Suggest;
			txtClassPath.AutoCompleteSource = AutoCompleteSource.CustomSource;
			txtClassPath.TextChanged += TxtClassPath_TextChanged;

			var grid = new GridLineManager(this, true);
			int line = grid.AddLineFilling(GlobalLayout.MinFill);
			grid.AddElementFilling(line, txtClassPath, GlobalLayout.LabelWidth);

		}

		private void TxtClassPath_TextChanged(object sender, System.EventArgs e) // TODO: test
		{
			string txtVaue = txtClassPath.Text;
			myData.ILNodePath = txtVaue;
			string[] pathParts = txtVaue.Split(new[] { '.', '/' });
			if (pathParts.Length == ilNodePathLevel) return;
			ilNodePathLevel = pathParts.Length;
			var source = new AutoCompleteStringCollection();
			ICollection<ILNode> sourceCollection;
			if (ilNodePathLevel == 0)
			{
				source.Add("-");
				sourceCollection = dataStruct.ILNodeManager.GetAllModules();
			}
			else
			{
				int okTextIndex = txtVaue.LastIndexOfAny(ILNodeManager.Seperators);
				ILNode searchNode = dataStruct.ILNodeManager.FindNodeByPath(txtVaue.Substring(0, okTextIndex));
				sourceCollection = searchNode?.Children;
			}

			if (sourceCollection != null)
				source.AddRange(sourceCollection.Select(node => node.Name).ToArray());
			txtClassPath.AutoCompleteCustomSource = source;
		}

		public override TargetFinder CreateNewEntryPart()
		{
			return new TargetFinderClassByName(dataStruct);
		}

		protected override void OnPatchDataSet()
		{
			txtClassPath.Text = myData.ILNodePath;
		}
	}
}
