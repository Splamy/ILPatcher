using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILPatcher.Utility;

namespace ILPatcher.Data
{
	class EntryFactory
	{
		private DataStruct dataStruct { get; }

		protected List<Type> allEntrys;
		protected List<Type> finderEntrys;
		protected List<Type> actionEntrys;

		public ICollection<Type> AllEditors => allEntrys.AsReadOnly();
		public ICollection<Type> FinderEditors => finderEntrys.AsReadOnly();
		public ICollection<Type> ActionEditors => actionEntrys.AsReadOnly();

		private static readonly Type entryBaseType = typeof(EntryBase);
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
						if (checkEntryType.IsSubclassOf(typeof(Actions.PatchAction)))
							actionEntrys.Add(checkEntryType);
						else if (checkEntryType.IsSubclassOf(typeof(Finder.TargetFinder)))
							finderEntrys.Add(checkEntryType);
					}
					else
					{
						Log.Write(Log.Level.Warning, "EntryBase without valid constructor \"public ctor(DataStruct)\" was found.");
					}
				}
			}
		}

		public EntryBase GetEntryByType(Type type)
		{
			return (EntryBase)Activator.CreateInstance(type, new[] { dataStruct });
		}
	}
}
