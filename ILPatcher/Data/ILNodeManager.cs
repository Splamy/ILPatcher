using ILPatcher.Utility;
using System.Collections.Generic;

namespace ILPatcher.Data
{
	public class ILNodeManager
	{
		private readonly Dictionary<string, ILNode> ModuleList;
		public ILNode StructViewToolBox { get; set; }
		public static char[] Seperators { get; } = new[] { '.', '/' };

		public ILNodeManager()
		{
			ModuleList = new Dictionary<string, ILNode>();
        }

		/// <summary>Searches for the Cecil Typ/Met/Fld/... matching the searched path in the loaded ILNode ModuleList.
		/// If the final element can't be found, null will be returned.</summary>
		/// <param name="path">A path of the form "asseblyname.namespace.class" or "-.namespace.class.method" to search all assemblys</param>
		/// <returns>Returns the Cecil object if found, otherwise null</returns>
		public object FindMemberByPath(string path)
		{
			return FindNodeByPath(path)?.Name;
		}

		public ILNode FindNodeByPath(string path)
		{
			string[] pathbreaks = path.Split(Seperators);

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
				return FindNodeByPathRecursive(ModuleList[pathbreaks[0]], pathbreaks, 1);
			}
			return null;
		}

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

		public bool IsModuleLoaded(string name)
		{
			return ModuleList.ContainsKey(name);
		}

		public void AddModule(string name, ILNode dataTree)
		{
			ModuleList.Add(name, dataTree);
		}

		/// <summary>Returns a collection of all loaded ILNode Assemblys.</summary>
		/// <returns>Returns a ILNode Assembly collection.</returns>
		public ICollection<ILNode> GetAllModules()
		{
			return ModuleList.Values;
		}

		public void Clear()
		{
			ModuleList.Clear();
		}
	}
}
