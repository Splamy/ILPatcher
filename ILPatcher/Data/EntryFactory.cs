using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILPatcher.Utility;

namespace ILPatcher.Data
{
	public sealed class EntryFactory
	{
		private DataStruct dataStruct { get; }

		private List<Type> allEntrys;
		private List<Type> finderEntrys;
		private List<Type> actionEntrys;

		public ICollection<Type> AllEditors => allEntrys.AsReadOnly();
		public ICollection<Type> FinderEditors => finderEntrys.AsReadOnly();
		public ICollection<Type> ActionEditors => actionEntrys.AsReadOnly();

		private static readonly Type entryBaseType = typeof(EntryBase);
		private static readonly Type patchEntryType = typeof(PatchEntry);
		private static readonly Type patchActionType = typeof(Actions.PatchAction);
		private static readonly Type targetFinderType = typeof(Finder.TargetFinder);
		private static readonly Type[] factoryContructor = new[] { typeof(DataStruct) };

		public EntryFactory(DataStruct dataStruct)
		{
			this.dataStruct = dataStruct;
			ReloadAllEntrys();
		}

		public void ReloadAllEntrys()
		{
			allEntrys = new List<Type>();
			finderEntrys = new List<Type>();
			actionEntrys = new List<Type>();

			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				var derivedOptions = asm.GetTypes().
					Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(entryBaseType));

				foreach (var checkEntryType in derivedOptions)
				{
					allEntrys.Add(checkEntryType);

					var validConstructor = checkEntryType.GetConstructor(factoryContructor);
					if (validConstructor != null)
					{
						if (checkEntryType.IsSubclassOf(patchActionType))
							actionEntrys.Add(checkEntryType);
						else if (checkEntryType.IsSubclassOf(targetFinderType))
							finderEntrys.Add(checkEntryType);
					}
					else
					{
						Log.Write(Log.Level.Warning, "EntryBase without valid constructor \"public ctor(DataStruct)\" was found.");
					}
				}
			}
		}

		public EntryBase CreateEntryByType(Type entryType)
		{
			var retEntry = (EntryBase)Activator.CreateInstance(entryType, new[] { dataStruct });
			if (patchEntryType.IsAssignableFrom(entryType))
				dataStruct.PatchEntryList.Add((PatchEntry)retEntry);
			else if (patchActionType.IsAssignableFrom(entryType))
				dataStruct.PatchActionList.Add((Actions.PatchAction)retEntry);
			else if (targetFinderType.IsAssignableFrom(entryType))
				dataStruct.TargetFinderList.Add((Finder.TargetFinder)retEntry);
			else
				throw new InvalidOperationException("The instanced Type cannot be added to the DataStruct pool.");
			return retEntry;
		}
	}
}
