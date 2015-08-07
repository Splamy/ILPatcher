using System;
using System.Xml;

namespace ILPatcher.Data.Actions
{
	public abstract class PatchAction : NamedElement, ISaveToFile
	{
		/*
		 * Guide to PatchActions
		 * -Every PA needs to handle a changed binary file autonomous
		 * -A PA can have a tool to repair itself upon changed binary files
		 * -Every PA needs a function to display its data to the managing form
		 * -Every PA must be able to Save/Load independently from the managing form and other (non-own) objects
		 */

		protected DataStruct dataStruct;

		public PatchStatus PatchStatus { get; protected set; }
		public abstract PatchActionType PatchActionType { get; }
		public abstract bool RequiresFixedOutput { get; }
		public abstract Type TInput { get; }

		public abstract void PassTarget(object target);
		/// <summary>Executes its patch-routine to the currently loaded Assembly data</summary>
		/// <returns>Returns true if it succeeded, false otherwise</returns>
		public abstract bool Execute(object target);

		public abstract bool Save(XmlNode output);
		public abstract bool Load(XmlNode input);

		public PatchAction(DataStruct dataAssociation)
		{
			dataStruct = dataAssociation;
		}
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
