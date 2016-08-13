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
	public class PatchEntry : EntryBase, ISaveToFile
	{
		public sealed override EntryKind EntryKind => EntryKind.PatchEntry;

		public override string Label
		{
			get
			{
				var strb = new StringBuilder();
				strb.Append(Name).Append(":\n");
				foreach (var finder in PatchChain)
				{
					strb.Append(finder.Name);
					strb.Append(" ->\n");
				}
				return strb.ToString();
			}
		}
		public override string Description => "Provides the way to find and change a part in the targeted binary.";
		public List<EntryBase> PatchChain { get; private set; }

		public PatchEntry(DataStruct dataStruct) : base(dataStruct)
		{
			PatchChain = new List<EntryBase>();
			Name = string.Empty;
		}

		public void Execute()
		{
			if (!Validate())
				return;

			object currentInput = DataStruct.AssemblyDefinition;
			try
			{
				foreach (var entry in PatchChain)
				{
					if (entry is TargetFinder)
					{
						var targetFinder = (TargetFinder)entry;
						currentInput = targetFinder.FilterInput(currentInput);
						if (!targetFinder.TOutput.IsAssignableFrom(currentInput.GetType())) // TODO: check other way round
							throw new InvalidOperationException($"{targetFinder.TargetFinderType} gave out wrong output type");
					}
					else if (entry is PatchAction)
					{
						var patchAction = (PatchAction)entry;
						bool result = patchAction.Execute(currentInput);
						if (!result)
							Log.Write(Log.Level.Info, $"Action \"{patchAction.Name}\" failed");
					}
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

			if (!val.ValidateTrue(PatchChain.Count == 0, () => $"\"{Name}\": No finders defined")) return false;
			var chainStart = PatchChain.First() as TargetFinder;
			if (!val.ValidateTrue(chainStart.TInput != typeof(AssemblyDefinition), () => $"\"{Name}\": Starting finder has no AssemblyDefinition input")) return false;

			Type currentType = typeof(AssemblyDefinition);
			foreach (var entry in PatchChain)
			{
				if (entry is TargetFinder)
				{
					var targetFinder = (TargetFinder)entry;
					if (!val.ValidateTrue(currentType != targetFinder.TInput, () => $"\"{Name}\": Mismatch in the finder chain for \"{targetFinder.Name}\"")) return false;
					currentType = targetFinder.TOutput;
				}
				else if (entry is PatchAction)
				{
					var patchAction = (PatchAction)entry;
					if (!val.ValidateTrue(patchAction.PatchStatus != PatchStatus.WoringPerfectly, () => $"Patch \"{patchAction.Name}\" is broken and won't be executed")) return false;
					if (!val.ValidateTrue(currentType != patchAction.TInput, () => $"\"{Name}\": Mismatch in the finder chain for \"{patchAction.Name}\"")) return false;
					return val.Ok;
				}
			}
			val.ValidateTrue(false, () => $"\"{Name}\": No action defined");
			return val.Ok;
		}

		public override bool Save(XmlNode output)
		{
			var xTargetFinder = output.InsertCompressedElement(SST.EntryList);
			xTargetFinder.CreateAttribute(SST.Name, string.Join(" ", PatchChain.Select(x => x.Id)));
			return true;
		}

		public override bool Load(XmlNode input)
		{
			Validator val = new Validator();
			var xTargetFinder = input.GetChildNode(SST.EntryList, 0);
			if (!val.ValidateSet(xTargetFinder, () => "No EntryList element found!")) return false;

			string[] finderIds = xTargetFinder.GetAttribute(SST.Name).Split(' ');
			foreach (string finderId in finderIds)
			{
				var entry = DataStruct.EntryBaseList.FirstOrDefault(tf => tf.Id == finderId);
				if (!val.ValidateSet(entry, () => $"Entry with the ID \"{finderId}\" was not found")) continue;
				PatchChain.Add(entry);
			}

			return val.Ok;
		}
	}
}
