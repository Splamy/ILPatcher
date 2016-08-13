using System;
using System.Collections.Generic;
using System.Linq;
using ILPatcher.Interface.Finder;
using ILPatcher.Interface.Actions;
using ILPatcher.Utility;
using ILPatcher.Data;

namespace ILPatcher.Interface
{
	public sealed class EditorFactory
	{
		private List<Type> allEditors;
		private List<Type> finderEditors;
		private List<Type> actionEditors;
		private Dictionary<Type, Type> dataToEditorMap;
		private Dictionary<Type, Type> editorToEntryMap;

		public ICollection<Type> AllEditors => allEditors.AsReadOnly();
		public ICollection<Type> FinderEditors => finderEditors.AsReadOnly();
		public ICollection<Type> ActionEditors => actionEditors.AsReadOnly();

		private static readonly Type editorBaseType = typeof(EditorBase<>);
		private static readonly Type editorActionType = typeof(EditorPatchAction<>);
		private static readonly Type editorFinderType = typeof(EditorTargetFinder<>);
		private static readonly Type[] factoryContructor = new[] { typeof(DataStruct) };

		public EditorFactory()
		{
			ReloadAllEditors();
		}

		public void ReloadAllEditors()
		{
			allEditors = new List<Type>();
			finderEditors = new List<Type>();
			actionEditors = new List<Type>();
			dataToEditorMap = new Dictionary<Type, Type>();
			editorToEntryMap = new Dictionary<Type, Type>();

			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				var derivedOptions = asm.GetTypes().
					Where(myType => myType.IsClass && !myType.IsAbstract && HasGenericBaseType(myType, editorBaseType));

				foreach (var checkEditorType in derivedOptions)
				{
					allEditors.Add(checkEditorType);

					var validConstructor = checkEditorType.GetConstructor(factoryContructor);
					if (validConstructor != null)
					{
						if (HasGenericBaseType(checkEditorType, editorActionType))
							actionEditors.Add(checkEditorType);
						else if (HasGenericBaseType(checkEditorType, editorFinderType))
							finderEditors.Add(checkEditorType);
					}
					else
					{
						Log.Write(Log.Level.Warning, "Editor without valid constructor \"public ctor(DataStruct)\" was found.");
					}
				}
			}

			foreach (var editorType in AllEditors)
			{
				Type entryType = GetEditorTarget(editorType);
				dataToEditorMap.Add(entryType, editorType);
				editorToEntryMap.Add(editorType, entryType);
			}
		}

		public EditorPanel CreateEditorForEntry(EntryBase entry)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));

			Type editorType = GetEditorTypeByEntry(entry);
			var editor = (EditorPanel)Activator.CreateInstance(editorType, new[] { entry.DataStruct });
			editor.SetPatchData(entry);
			return editor;
		}

		public Type GetEditorTypeByEntry(EntryBase entry)
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

		public Type GetEntryTypeByEditorType(Type editorType)
		{
			if (editorType == null) throw new ArgumentNullException(nameof(editorType));

			Type entryType;
			if (editorToEntryMap.TryGetValue(editorType, out entryType))
			{
				return entryType;
			}
			else
			{
				Log.Write(Log.Level.Warning, "There is no entry registered for this editor.");
				return null;
			}
		}

		// Static info methods

		private static void ValidateType(Type editorType)
		{
			if (!HasGenericBaseType(editorType, editorBaseType))
				throw new ArgumentException("The argument does not describe an editor", nameof(editorType));
		}

		private static bool HasGenericBaseType(Type checkType, Type baseType)
		{
			if (baseType == null) throw new ArgumentNullException(nameof(baseType));

			while (checkType != null)
			{
				if (checkType.IsGenericType)
					checkType = checkType.GetGenericTypeDefinition();

				if (checkType == baseType)
					return true;
				else
				{
					checkType = checkType.BaseType;
					if (checkType == typeof(object))
						return false;
				}
			}
			return false;
		}

		private static EditorAttributes GetEditorAttribute(Type editorType)
		{
			ValidateType(editorType);
			return editorType.GetCustomAttributes(typeof(EditorAttributes), true).OfType<EditorAttributes>().FirstOrDefault();
		}

		public static string GetEditorName(Type editorType) => GetEditorAttribute(editorType).EditorName;

		public static Type GetEditorTarget(Type editorType)
		{
			for (Type checkType = editorType; checkType != null; checkType = checkType.BaseType)
			{
				if (checkType.IsGenericType && checkType.GetGenericTypeDefinition() == editorBaseType)
				{
					// generic parameter in EditorBase<Spec>
					return checkType.GetGenericArguments()[0];
				}
			}
			throw new InvalidOperationException();
		}
	}
}
