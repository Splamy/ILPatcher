using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ILPatcher.Data
{
	public class ILNode
	{
		private readonly List<ILNode> children = new List<ILNode>();

		public string Name { get; private set; }
		public string FullName { get; private set; }
		public object Value { get; private set; }
		public StructureView VisibleMembers { get; private set; }

		public ILNode(string name, string fullname, object value, StructureView visibleMembers)
		{
			Value = value;
			Name = name;
			FullName = fullname;
			VisibleMembers = visibleMembers;
		}

		public ILNode this[int childIndex]
		{
			get { return children[childIndex]; }
		}

		public ILNode Parent { get; private set; }

		public ReadOnlyCollection<ILNode> Children
		{
			get { return children.AsReadOnly(); }
		}

		public ILNode Add(string name, string fullname, object value, StructureView visibleMembers)
		{
			var node = new ILNode(name, fullname, value, visibleMembers) { Parent = this };
			children.Add(node);
			return node;
		}

		public bool Remove(ILNode node)
		{
			return children.Remove(node);
		}

		public override string ToString()
		{
			return FullName;
		}

		public void Sort()
		{
			children.Sort((x, y) =>
			{
				var strucdiff = x.VisibleMembers - y.VisibleMembers;
				return strucdiff != 0 ? strucdiff : x.Name.CompareTo(y.Name);
			});
			children.ForEach(x => x.Sort());
		}
	}

	[Flags]
	public enum StructureView : int
	{
		None = 0,
		Structure = 1 << 0,
		Namesp = 1 << 1,
		Methods = 1 << 2,
		Fields = 1 << 3,
		Classes = 1 << 4,
		Basestructure = Structure | Namesp | Classes,
		All = Structure | Methods | Namesp | Classes | Fields,
	}
}
