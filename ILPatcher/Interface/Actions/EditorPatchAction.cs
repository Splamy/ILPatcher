using ILPatcher.Data;
using ILPatcher.Data.Actions;
using ILPatcher.Interface.General;

namespace ILPatcher.Interface.Actions
{
	public class EditorPatchAction : EditorPanel<PatchAction>
	{
		public override string PanelName { get { return "Default PatchAction"; } }

		protected EditorPatchAction() { /*Reserverd for VSDesigner*/ }
		protected EditorPatchAction(DataStruct dataAssociation)
			: base(dataAssociation)
		{ }
	}
}
