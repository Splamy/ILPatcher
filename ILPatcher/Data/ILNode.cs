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
		public StructureView Flags { get; private set; }

		public ILNode(string name, string fullname, object value, StructureView viewFlag)
		{
			Value = value;
			Name = name;
			FullName = fullname;
			Flags = viewFlag;
		}

		public ILNode this[int i]
		{
			get { return children[i]; }
		}

		public ILNode Parent { get; private set; }

		public ReadOnlyCollection<ILNode> Children
		{
			get { return children.AsReadOnly(); }
		}

		public ILNode Add(string name, string fullname, object value, StructureView flags)
		{
			var node = new ILNode(name, fullname, value, flags) { Parent = this };
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
				var strucdiff = x.Flags - y.Flags;
				return strucdiff != 0 ? strucdiff : x.Name.CompareTo(y.Name);
			});
			children.ForEach(x => x.Sort());
		}
	}

	[Flags]
	public enum StructureView : int
	{
		none = 0,
		structure = 1 << 0,
		namesp = 1 << 1,
		methods = 1 << 2,
		fields = 1 << 3,
		classes = 1 << 4,
		basestructure = structure | namesp | classes,
		all = structure | methods | namesp | classes | fields,
	}
}
