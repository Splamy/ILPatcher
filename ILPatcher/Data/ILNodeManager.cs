using ILPatcher.Utility;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mono.Cecil;
using System;
using System.Linq;

namespace ILPatcher.Data
{
	public sealed class ILNodeManager
	{
		private DataStruct dataStruct;
		private readonly Dictionary<string, ILNode> ModuleList;
		public ILNode StructViewToolBox { get; set; }
		public static char[] Seperators { get; } = new[] { '/' };

		public delegate void ModuleChangedDelegate(object sender);
		public event ModuleChangedDelegate OnModuleChanged;

		public ILNodeManager(DataStruct dataStruct)
		{
			ModuleList = new Dictionary<string, ILNode>();
			this.dataStruct = dataStruct;
			dataStruct.OnASMFileLoaded += DataStruct_OnASMFileLoaded;
		}

		private void DataStruct_OnASMFileLoaded(object sender)
		{
			LoadAssembly(dataStruct.AssemblyDefinition);
		}

		/// <summary>Searches for the Cecil Typ/Met/Fld/... matching the searched path in the loaded ILNode ModuleList.
		/// If the final element can't be found, null will be returned.</summary>
		/// <param name="path">A path of the form "asseblyname.namespace.class" or "-.namespace.class.method" to search all assemblys</param>
		/// <returns>Returns the Cecil object if found, otherwise null</returns>
		public object FindMemberByPath(string path)
		{
			return FindNodeByPath(path)?.Value;
		}

		public ILNode FindNodeByPath(string path)
		{
			string[] pathbreaks = path.Split(Seperators);
			pathbreaks = pathbreaks.Select(t => t.Replace("/", ".")).ToArray();

			if (pathbreaks.Length == 0)
			{
				Log.Write(Log.Level.Warning, "FindTypeByName path is empty");
				return null;
			}

			if (pathbreaks[0] == "-")
			{
				foreach (ILNode child in ModuleList.Values)
				{
					ILNode res = FindNodeByPathRecursive(child, pathbreaks, 1);
					if (res != null) return res;
				}
			}
			else if (ModuleList.ContainsKey(pathbreaks[0]))
			{
				var moduleNode = ModuleList[pathbreaks[0]];
				if (pathbreaks.Length == 1)
					return moduleNode;
				else
					return FindNodeByPathRecursive(moduleNode, pathbreaks, 1);
			}
			return null;
		}
		// TODO do inline instead of recursive for better debugging

		/// <summary>Traverses the loaded ILNode children and searches the current path index in its children</summary>
		/// <param name="searchnode">The ILNode with the children for the current path index</param>
		/// <param name="path">The array of all path parts</param>
		/// <param name="index">The current path index</param>
		/// <returns></returns>
		private ILNode FindNodeByPathRecursive(ILNode searchnode, string[] path, int index)
		{
			if (index >= path.Length) return null;

			foreach (ILNode child in searchnode.Children)
			{
				if (child.Name == path[index])
				{
					if (index == path.Length - 1) return child;
					else return FindNodeByPathRecursive(child, path, index + 1);
				}
			}
			return null;
		}

		/// <summary>Returns a collection of all loaded ILNode Assemblys.</summary>
		/// <returns>Returns a ILNode Assembly collection.</returns>
		public ICollection<ILNode> GetAllModules()
		{
			return ModuleList.Values;
		}

		private bool IsModuleLoaded(string name)
		{
			return ModuleList.ContainsKey(name);
		}

		private void AddModule(string name, ILNode dataTree)
		{
			ModuleList.Add(name, dataTree);
		}

		/// <summary>Creates an ILNode-Tree representing the structure of the given Assembly
		/// and stores it in the ModuleList Dictionary with the AssemblyDefinition name as key.</summary>
		/// <param name="AssDef">The AssemblyDefinition which should be loaded into the searchlist</param>
		/// <param name="SubResolveDepth">When the given AssemblyDefinition uses references to other Assemblys
		/// the method will add them recursivly to the given depth</param>
		public void LoadAssembly(AssemblyDefinition AssDef, int SubResolveDepth = 0)
		{
			if (AssDef == null) throw new ArgumentNullException(nameof(AssDef));
			if (SubResolveDepth < 0) throw new ArgumentException(nameof(SubResolveDepth) + " must be non-negative.");
			if (IsModuleLoaded(AssDef.Name.Name)) return;

			ILNode ilParent = new ILNode(AssDef.Name.Name, AssDef.FullName, AssDef, StructureView.structure); // StructureView.Module
			AddModule(AssDef.Name.Name, ilParent);

			foreach (ModuleDefinition ModDef in AssDef.Modules)
			{
				ILNode tnModDef = ilParent.Add(ModDef.Name, ModDef.Name, ModDef, StructureView.structure);
				DefaultAssemblyResolver dar = (DefaultAssemblyResolver)ModDef.AssemblyResolver;
				Array.ForEach(dar.GetSearchDirectories(), dar.RemoveSearchDirectory);
				dar.AddSearchDirectory(Path.GetDirectoryName(dataStruct.AssemblyLocation));

				// Subresolving references
				foreach (AssemblyNameReference anr in ModDef.AssemblyReferences)
				{
					try
					{
						AssemblyDefinition AssSubRef = ModDef.AssemblyResolver.Resolve(anr);
						tnModDef.Add(anr.Name, AssSubRef.FullName, AssSubRef, StructureView.structure);
						if (SubResolveDepth > 0)
							LoadAssembly(AssSubRef, SubResolveDepth - 1);
					}
					catch { Log.Write(Log.Level.Warning, $"AssemblyReference \"{anr.Name}\" couldn't be found for \"{ ModDef.Name}\""); }
				}

				Dictionary<string, ILNode> nsDict = new Dictionary<string, ILNode>();
				foreach (TypeDefinition TypDef in ModDef.Types)
				{
					string nsstr = TypDef.Namespace;
					ILNode tnAssemblyContainer;
					if (!nsDict.ContainsKey(nsstr))
					{
						string displaystr = string.IsNullOrEmpty(nsstr) ? "<Default Namespace>" : nsstr;
						tnAssemblyContainer = ilParent.Add(displaystr, displaystr, new NamespaceHolder(displaystr), StructureView.namesp);
						nsDict.Add(nsstr, tnAssemblyContainer);
					}
					else
						tnAssemblyContainer = nsDict[nsstr];

					ILNode tnTypDef = tnAssemblyContainer.Add(TypDef.Name, TypDef.FullName, TypDef, StructureView.classes);
					LoadSubItemsRecursive(tnTypDef, TypDef);
				}
			}
			ilParent.Sort();

			if (SubResolveDepth == 0) // If this is the last LoadAssembly recursion call then invoke the callback
				OnModuleChanged?.Invoke(this);
		}

		/// <summary>Traverses the Assembly recursivly and adds the new ILnodes to the given ILNode</summary>
		/// <param name="parentNode">The parent ILNode for the new subelements</param>
		/// <param name="TypDef">The TypeDefinition to read</param>
		private void LoadSubItemsRecursive(ILNode parentNode, TypeDefinition TypDef)
		{
			#region Functions
			//if (ViewElements.HasFlag(StructureView.functions))
			foreach (MethodDefinition MetDef in TypDef.Methods)
			{
				StringBuilder strb = new StringBuilder();
				StringBuilder strbfn = new StringBuilder();
				strb.Append(MetDef.Name); strbfn.Append(MetDef.Name);
				strb.Append('('); strbfn.Append('(');
				foreach (ParameterDefinition ParDef in MetDef.Parameters)
				{
					strbfn.Append(ParDef.ParameterType.FullName);
					strb.Append(ParDef.ParameterType.Name);
					strb.Append(','); strbfn.Append(',');
				}
				if (MetDef.Parameters.Count > 0)
				{
					strb.Remove(strb.Length - 1, 1); strbfn.Remove(strb.Length - 1, 1);
				}
				strb.Append(") : "); strbfn.Append(") : ");
				strbfn.Append(MetDef.ReturnType.FullName);
				strb.Append(MetDef.ReturnType.Name);
				parentNode.Add(strb.ToString(), strbfn.ToString(), MetDef, StructureView.methods);
			}
			#endregion

			#region Fields
			//if (ViewElements.HasFlag(StructureView.fields))
			foreach (FieldDefinition FieDef in TypDef.Fields)
			{
				StringBuilder strb = new StringBuilder();
				StringBuilder strbfn = new StringBuilder();
				strb.Append(FieDef.Name); strbfn.Append(FieDef.Name);
				strb.Append(" : "); strbfn.Append(" : ");

				strbfn.Append(FieDef.FieldType.FullName);
				strb.Append(FieDef.FieldType.Name);

				parentNode.Add(strb.ToString(), strbfn.ToString(), FieDef, StructureView.fields);
			}
			#endregion

			#region SubClasses
			foreach (TypeDefinition SubTypDef in TypDef.NestedTypes)
			{
				ILNode tnSubTypDef = parentNode.Add(SubTypDef.Name, SubTypDef.Name, SubTypDef, StructureView.classes);
				LoadSubItemsRecursive(tnSubTypDef, SubTypDef);
			}
			#endregion
		}

		public void Clear()
		{
			ModuleList.Clear();
		}
	}

	public class NamespaceHolder
	{
		public string Namespace { get; private set; }

		public NamespaceHolder(string _nns)
		{
			Namespace = _nns;
		}
	}
}
