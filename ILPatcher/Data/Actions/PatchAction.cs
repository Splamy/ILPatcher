using System;

namespace ILPatcher.Data.Actions
{
	public abstract class PatchAction : EntryBase
	{
		/*
		 * Guide to PatchActions
		 * -Every PA needs to handle a changed binary file autonomous
		 * -A PA can have a tool to repair itself upon changed binary files
		 * -Every PA needs a function to display its data to the managing form
		 * -Every PA must be able to Save/Load independently from the managing form and other (non-own) objects
		 */

		public sealed override EntryKind EntryKind => EntryKind.PatchAction;

		public PatchStatus PatchStatus { get; protected set; }
		public abstract PatchActionType PatchActionType { get; }
		public abstract bool RequiresFixedOutput { get; }
		public abstract Type TInput { get; }

		public abstract void PassTarget(object target);
		/// <summary>Executes its patch-routine to the currently loaded Assembly data</summary>
		/// <returns>Returns true if it succeeded, false otherwise</returns>
		public abstract bool Execute(object target);

		public PatchAction(DataStruct dataStruct) : base(dataStruct) { }
	}

	public enum PatchActionType
	{
		ILMethodFixed,
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
