using System.Xml;

namespace ILPatcher.Data.Actions
{
	public abstract class PatchAction : ISaveToFile
	{
		/*
		 * Guide to PatchActions
		 * -Every PA needs to handle a changed binary file by itself
		 * -A PA can have a tool to repair itself upon changed binary files
		 * -Every PA needs a function to display its data to the managing form
		 * -Every PA must be able to Save/Load independently from the managing form and other (non-own) objects
		 */

		public string ActionName { get; set; }
		public PatchStatus PatchStatus { get; protected set; }
		public abstract PatchActionType PatchActionType { get; protected set; }
		protected DataStruct dataStruct { get; private set; }

		public PatchAction(DataStruct dataAssociation)
		{
			dataStruct = dataAssociation;
		}

		public abstract bool Execute();
		public abstract bool Save(XmlNode output);
		public abstract bool Load(XmlNode input);
	}

	public enum PatchActionType
	{
		ILMethodFixed,
		ILMethodDynamic,
		ILDynamicScan,
		AoBRawScan,
		ILMethodCreator,
	}

	public enum PatchStatus
	{
		Unset,
		WoringPerfectly,
		WorkingNoResolve,
		Broken,
	}
}
