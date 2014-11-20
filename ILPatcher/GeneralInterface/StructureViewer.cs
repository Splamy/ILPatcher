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

		public void RebuildHalfAsync()
		{
			System.Threading.Thread t = new System.Threading.Thread(() => Rebuild(true));
			Nodes.Clear();
			t.Start();
			while (t.IsAlive)
			{
				Application.DoEvents();
				System.Threading.Thread.Sleep(1);
			}
		}

		public void Rebuild(bool invoke = false)
		{
			if(!invoke)
				Nodes.Clear();
			foreach (ILNode iln in ILManager.Instance.getAllNodes())
			{
				if (ViewElements.HasFlag(iln.Flags))
				{
					TreeNode nMain = new TreeNode(UseFullName ? iln.FullName : iln.Name);
					nMain.Tag = iln.Value;
					TypeDefinition td = iln.Value as TypeDefinition;
					if (td != null) nMain.ImageIndex = nMain.SelectedImageIndex = GetImage(td);
					RecursiveRebuild(nMain, iln);

					if (invoke)
						Invoke(new _AddNode(AddNode), new object[] { nMain });
					else
						Nodes.Add(nMain);
				}
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
					TypeDefinition td = iln.Value as TypeDefinition;
					if (td != null) tnSub.ImageIndex = tnSub.SelectedImageIndex = GetImage(td);
					RecursiveRebuild(tnSub, iln);
					tnParent.Nodes.Add(tnSub);
				}
		}

		private int GetImage(TypeDefinition TypeDef)
		{
			if (TypeDef.IsArray)
				return 0;
			//if (TypeDef.IsAbstract)
			//	return 1;
			if (TypeDef.IsClass)
				return 1;
			if (TypeDef.IsEnum)
				return 2;
			if (TypeDef.IsInterface)
				return 5;
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