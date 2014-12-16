using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Reflection;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public class StructureViewer : TreeView
	{
		[DefaultValue(StructureView.all)]
		public StructureView ViewElements { get; set; }

		[DefaultValue(false)]
		public bool UseFullName { get; set; }

		public StructureViewer()
		{
			ViewElements = StructureView.all;

			ImageList imgl = new ImageList();
			imgl.Images.Add(ILPatcher.Properties.Resources.Array);
			imgl.Images.Add(ILPatcher.Properties.Resources.Class);
			imgl.Images.Add(ILPatcher.Properties.Resources.Enum);
			imgl.Images.Add(ILPatcher.Properties.Resources.Field);
			imgl.Images.Add(ILPatcher.Properties.Resources.Function);
			imgl.Images.Add(ILPatcher.Properties.Resources.Interface);
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
				if (ViewElements.HasFlag(iln.Flags))
				{
					TreeNode nMain = new TreeNode(UseFullName ? iln.FullName : iln.Name);
					nMain.Tag = iln.Value;
					MemberReference mr = iln.Value as MemberReference;
					if (mr != null) nMain.ImageIndex = nMain.SelectedImageIndex = GetImage(mr);
					RecursiveRebuild(nMain, iln);

					if (invoke)
						Invoke(new _AddNode(AddNode), new object[] { nMain });
					else
						Nodes.Add(nMain);
				}
				if(mainmodonly) break;
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
				if (ViewElements.HasFlag(iln.Flags))
				{
					TreeNode tnSub = new TreeNode(UseFullName ? iln.FullName : iln.Name);
					tnSub.Tag = iln.Value;
					MemberReference mr = iln.Value as MemberReference;
					if (mr != null) tnSub.ImageIndex = tnSub.SelectedImageIndex = GetImage(mr);
					RecursiveRebuild(tnSub, iln);
					tnParent.Nodes.Add(tnSub);
				}
		}

		private int GetImage(MemberReference MemRef)
		{
			if ((MemRef as FieldReference) != null) //isField
				return 3;
			if ((MemRef as MethodReference) != null) //isMethod
				return 4;
			if ((MemRef as TypeDefinition) != null) //isClassOrSub
			{
				TypeDefinition TD = MemRef as TypeDefinition;
				if (TD.IsEnum)
					return 2;
				return 1;
			}
			if ((MemRef as TypeReference) != null) //isClass
				return 1;
			return 3;
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
}