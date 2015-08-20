using System;
using System.Collections.Generic;
using System.Linq;
using ILPatcher.Interface.Finder;
using ILPatcher.Interface.Actions;
using ILPatcher.Utility;

namespace ILPatcher.Interface
{
	public class EditorFactory
	{
		protected List<Type> allEditors;
		protected List<Type> finderEditors;
		protected List<Type> actionEditors;
		protected Dictionary<Type, Type> dataToEditorMap;

		public ICollection<Type> AllEditors => allEditors.AsReadOnly();
		public ICollection<Type> FinderEditors => finderEditors.AsReadOnly();
		public ICollection<Type> ActionEditors => actionEditors.AsReadOnly();

		private static readonly Type[] factoryContructor = new[] { typeof(Data.DataStruct) };
		private static readonly Type editorBaseType = typeof(EditorBase<,>);

		public EditorFactory()
		{
			ReloadAllEditors();
		}

		public void ReloadAllEditors()
		{
			allEditors = new List<Type>();
			finderEditors = new List<Type>();
			actionEditors = new List<Type>();

			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				var derivedOptions = asm.GetTypes().
					Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(editorBaseType));

				foreach (var checkEditorType in derivedOptions)
				{
					allEditors.Add(checkEditorType);

					var validConstructor = checkEditorType.GetConstructor(factoryContructor);
					if (validConstructor != null)
					{
						if (typeof(EditorPatchAction<>).IsAssignableFrom(checkEditorType))
							actionEditors.Add(checkEditorType);
						else if (typeof(EditorTargetFinder<>).IsAssignableFrom(checkEditorType))
							finderEditors.Add(checkEditorType);
					}
					else
					{
						Log.Write(Log.Level.Warning, "Editor without valid constructor \"public ctor(DataStruct)\" was found.");
					}
				}
			}
		}

		public IEditorPanel GetEditor(Data.EntryBase entry)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));

			Type editorType = GetEditorType(entry);
			return (IEditorPanel)Activator.CreateInstance(editorType, new[] { entry.dataStruct });
        }

		public Type GetEditorType(Data.EntryBase entry)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));

			Type editorType;
			if (dataToEditorMap.TryGetValue(entry.GetType(), out editorType))
			{
				return editorType;
			}
			else
			{
				Log.Write(Log.Level.Warning, "There is no editor registered for this entry.");
				return null;
			}
		}

		// Static info methods

		private static void ValidateType(Type editorType)
		{
			if (!editorBaseType.IsAssignableFrom(editorType))
				throw new ArgumentException("The argument does not describe an editor", nameof(editorType));
		}

		private static EditorAttributes GetEditorAttribute(Type editorType)
		{
			ValidateType(editorType);
			return editorType.GetCustomAttributes(typeof(EditorAttributes), true).OfType<EditorAttributes>().FirstOrDefault();
		}

		public static bool IsInline(Type editorType) => GetEditorAttribute(editorType).Inline;

		public static string GetEditorName(Type editorType) => GetEditorAttribute(editorType).EditorName;

		public static Type GetDataTarget(Type editorType)
		{
			ValidateType(editorType);
			return editorType.GetGenericArguments()[1]; // second parameter in <Kind, Spec>
		}
	}
}
