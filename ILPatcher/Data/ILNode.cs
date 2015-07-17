using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ILPatcher.Data
{
	public class ILNode
	{
		private readonly List<ILNode> _children = new List<ILNode>();

		public string Name { private set; get; }
		public string FullName { private set; get; }
		public object Value { private set; get; }
		public StructureView Flags { private set; get; }

		public ILNode(string name, string fullname, object value, StructureView viewFlag)
		{
			Value = value;
			Name = name;
			FullName = fullname;
			Flags = viewFlag;
		}

		public ILNode this[int i]
		{
			get { return _children[i]; }
		}

		public ILNode Parent { get; private set; }

		public ReadOnlyCollection<ILNode> Children
		{
			get { return _children.AsReadOnly(); }
		}

		public ILNode Add(string name, string fullname, object value, StructureView flags)
		{
			var node = new ILNode(name, fullname, value, flags) { Parent = this };
			_children.Add(node);
			return node;
		}

		public bool Remove(ILNode node)
		{
			return _children.Remove(node);
		}

		public override string ToString()
		{
			return FullName;
		}

		public void Sort()
		{
			_children.Sort((x, y) =>
			{
				var strucdiff = x.Flags - y.Flags;
				return strucdiff != 0 ? strucdiff : x.Name.CompareTo(y.Name);
			});
			_children.ForEach(x => x.Sort());
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
