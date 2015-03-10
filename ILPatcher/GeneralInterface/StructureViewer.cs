using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Reflection;
using LocRes = ILPatcher.Properties.Resources;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public class StructureViewer : TreeView
	{
		[DefaultValue(StructureView.all)]
		public StructureView FilterElements { get; set; }

		[DefaultValue(false)]
		public bool UseFullName { get; set; }

		public StructureViewer()
		{
			FilterElements = StructureView.all;

			ImageList imgl = new ImageList();
			imgl.Images.Add(LocRes.Array);
			imgl.Images.Add(LocRes.Assembly);
			imgl.Images.Add(LocRes.Class);
			imgl.Images.Add(LocRes.Constant);
			imgl.Images.Add(LocRes.Enum);
			imgl.Images.Add(LocRes.Field);
			imgl.Images.Add(LocRes.Function);
			imgl.Images.Add(LocRes.Interface);
			imgl.Images.Add(LocRes.Module);
			imgl.Images.Add(LocRes.Namespace);
			imgl.Images.Add(LocRes.Operator);
			imgl.Images.Add(LocRes.Property);
			this.ImageList = imgl;
		}

		public void RebuildHalfAsync(bool mainmodonly = false)
		{
			System.Threading.Thread t = new System.Threading.Thread(() => RebuildInternal(mainmodonly, true));
			Nodes.Clear();
			t.Start();
			while (t.IsAlive)
			{
				Application.DoEvents();
				System.Threading.Thread.Sleep(1);
			}
		}

		public void Rebuild(bool mainmodonly = false)
		{
			RebuildInternal(mainmodonly, false);
		}

		private void RebuildInternal(bool mainmodonly, bool invoke)
		{
			if (!invoke)
				Nodes.Clear();
			foreach (ILNode iln in ILManager.Instance.getAllNodes())
			{
				if (FilterElements.HasFlag(iln.Flags))
				{
					TreeNode nMain = new TreeNode(UseFullName ? iln.FullName : iln.Name);
					nMain.Tag = iln.Value;
					nMain.ImageIndex = nMain.SelectedImageIndex = GetImage(iln.Value);
					RecursiveRebuild(nMain, iln);

					if (invoke)
						Invoke(new _AddNode(AddNode), new object[] { nMain });
					else
						Nodes.Add(nMain);
				}
				if (mainmodonly) break;
			}
		}

		private delegate void _AddNode(TreeNode tn);
		private void AddNode(TreeNode tn)
		{
			Nodes.Add(tn);
		}

		private void RecursiveRebuild(TreeNode tnParent, ILNode ilParent)
		{
			foreach (ILNode iln in ilParent.Children)
				if (FilterElements.HasFlag(iln.Flags))
				{
					TreeNode tnSub = new TreeNode(UseFullName ? iln.FullName : iln.Name);
					tnSub.Tag = iln.Value;
					tnSub.ImageIndex = tnSub.SelectedImageIndex = GetImage(iln.Value);
					RecursiveRebuild(tnSub, iln);
					tnParent.Nodes.Add(tnSub);
				}
		}

		private int GetImage(object MemRef)
		{
			if (MemRef == null)
				return (int)ImageMap.Assembly; // TMP
			if (MemRef is FieldReference) //isField
			{
				FieldReference FR = (FieldReference)MemRef;
				if (FR.FieldType.IsArray)
					return (int)ImageMap.Array;
				return (int)ImageMap.Field;
			}
			if (MemRef is MethodReference) //isMethod
				return (int)ImageMap.Function;
			if (MemRef is TypeDefinition) //isClassOrSub
			{
				TypeDefinition TD = (TypeDefinition)MemRef;
				if (TD.IsEnum)
					return (int)ImageMap.Enum;
				if (TD.IsInterface)
					return (int)ImageMap.Interface;
				return (int)ImageMap.Class;
			}
			if (MemRef is TypeReference) //isClass
				return (int)ImageMap.Class;
			if (MemRef is NamespaceHolder)
				return (int)ImageMap.Namespace;

			return (int)ImageMap.Assembly;
		}
	}

	public class NamespaceHolder
	{
		public string Namespace;

		public NamespaceHolder(string _nns)
		{
			Namespace = _nns;
		}
	}

	[Flags]
	public enum StructureView
	{
		none = 0,
		classes = 1 << 0,
		functions = 1 << 1,
		fields = 1 << 2,
		all = classes | functions | fields
	}

	public enum ImageMap : int
	{
		Array,
		Assembly,
		Class,
		Constant,
		Enum,
		Field,
		Function,
		Interface,
		Module,
		Namespace,
		Operator,
		Property
	}
}