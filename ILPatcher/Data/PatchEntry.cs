using ILPatcher.Data.Actions;
using ILPatcher.Data.Finder;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Mono.Cecil;
using ILPatcher.Utility;
using System;
using System.Linq;

namespace ILPatcher.Data
{
	public class PatchEntry : NamedElement, ISaveToFile
	{
		public override string Label
		{
			get
			{
				var strb = new StringBuilder();
				strb.Append(Name);
				strb.Append(":\n");
				foreach (var finder in FinderChain)
				{
					strb.Append(finder.Name);
					strb.Append(" ->\n");
				}
				if(PatchAction != null)
					strb.Append(PatchAction.Name);
				else
					strb.Append("<X>");
				return strb.ToString();
			}
		}
		public override string Description { get { return "Provides the way to find and change a part in the targeted binary."; } }
		public List<TargetFinder> FinderChain { get; set; }
		public PatchAction PatchAction { get; set; }
		private DataStruct dataStruct;

		public PatchEntry(DataStruct dataStruct)
		{
			FinderChain = new List<TargetFinder>();
			PatchAction = null;
			Name = string.Empty;
			this.dataStruct = dataStruct;
		}

		public void Execute()
		{
			if (!Validate())
				return;

			if (PatchAction.PatchStatus != PatchStatus.WoringPerfectly)
			{
				Log.Write(Log.Level.Info, $"Patch \"{PatchAction.Name}\" is broken and won't be executed");
				return;
			}

			object currentInput = dataStruct.AssemblyDefinition;
			try
			{
				foreach (var tf in FinderChain)
				{
					currentInput = tf.FilterInput(currentInput);
					throw new InvalidOperationException($"{tf.TargetFinderType} gave out wrong output type");
				}
			}
			catch (TargetNotFoundException tnf)
			{
				Log.Write(Log.Level.Info, $"Finder \"{tnf.FailedFinder}\" found no target");
				return;
			}
		}

		private bool Validate()
		{
			Validator val = new Validator();

			if (!val.ValidateSet(PatchAction, () => $"\"{Name}\": No action defined")) return false;
			if (!val.ValidateTrue(FinderChain.Count == 0, () => $"\"{Name}\": No finders defined")) return false;
			if (!val.ValidateTrue(FinderChain[0].TInput != typeof(AssemblyDefinition), () => $"\"{Name}\": Starting finder has no AssemblyDefinition input")) return false;

			Type currentType = typeof(AssemblyDefinition);
			foreach (var tf in FinderChain)
			{
				if (!val.ValidateTrue(currentType != tf.TInput, () => $"\"{Name}\": Mismatch in the finder chain for \"{tf.Name}\"")) return false;
				currentType = tf.TOutput;
			}
			return PatchAction.TInput == currentType;
		}

		public bool Save(XmlNode output)
		{
			var xTargetFinder = output.InsertCompressedElement(SST.TargetFinder);
			xTargetFinder.Value = string.Join(" ", FinderChain.Select(x => x.ID));

			var xPatchAction = output.InsertCompressedElement(SST.PatchAction);
			xTargetFinder.Value = PatchAction.ID;

			return true;
		}

		public bool Load(XmlNode input)
		{
			Validator val = new Validator();
			var xTargetFinder = input.GetChildNode(SST.TargetFinder, 0);
			if (!val.ValidateSet(xTargetFinder, () => "No TargetFinder element found!")) return false;

			var xPatchAction = input.GetChildNode(SST.PatchAction, 1);
			if (!val.ValidateSet(xPatchAction, () => "No PatchAction element found!")) return false;

			string[] finderIds = xTargetFinder.Value.Split(' ');
			foreach (string finderId in finderIds)
			{
				var targetFinder = dataStruct.TargetFinderList.FirstOrDefault(tf => tf.ID == finderId);
				if (!val.ValidateSet(targetFinder, () => $"TargetFinder with the ID \"{finderId}\" was not found")) continue;
				FinderChain.Add(targetFinder);
			}

			return val.Ok;
		}
	}
}
