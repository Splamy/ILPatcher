using ILPatcher.Data;
using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using LocRes = ILPatcher.Properties.Resources;

namespace ILPatcher.Interface
{
	public class StructureViewer : TreeView
	{
		[DefaultValue(StructureView.All)]
		public StructureView FilterElements { get; set; } = StructureView.All;

		[DefaultValue(false)]
		public bool UseFullName { get; set; } = false;

		[DefaultValue(true)]
		public bool ContextAssemblyLoad { get; set; } = true;

		private DataStruct dataStruct = null;
		private TreeNode ExtensionNode = new TreeNode("Extension Node");
		private ContextMenuStrip cxMenu = new ContextMenuStrip();
		private Thread rebuildThread = null;

		public StructureViewer(DataStruct dataAssociation)
		{
			PathSeparator = ".";

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

			MouseUp += StructureViewer_MouseUp;

			cxMenu.Items.Add("Load Assembly", null, ToolStripBar_LoadAssembly_Click);

			SetDataAssociation(dataAssociation);
		}

		protected override void Dispose(bool disposing)
		{
			ClearEventRegsters();
			base.Dispose(disposing);
		}

		public void SetDataAssociation(DataStruct dataAssociation)
		{
			if (dataAssociation == null)
				throw new ArgumentNullException(nameof(dataAssociation));

			ClearEventRegsters();
			dataAssociation.ILNodeManager.OnModuleChanged += ILNodeManager_OnModuleChanged;

			dataStruct = dataAssociation;
		}

		private void ClearEventRegsters()
		{
			if (dataStruct != null)
				dataStruct.ILNodeManager.OnModuleChanged -= ILNodeManager_OnModuleChanged;
		}

		// events

		void ToolStripBar_LoadAssembly_Click(object sender, EventArgs e)
		{
			AssemblyDefinition asmDefToLoad = SelectedNode.Tag as AssemblyDefinition;
			if (asmDefToLoad == null)
				throw new InvalidOperationException("The node does not contain an 'AssemblyDefinition'");

			dataStruct.ILNodeManager.LoadAssembly(asmDefToLoad);
		}

		private void StructureViewer_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && ContextAssemblyLoad)
			{
				Point p = new Point(e.X, e.Y);
				TreeNode node = GetNodeAt(p);
				if (node != null)
				{
					SelectedNode = node;
					if (node.Tag is AssemblyDefinition && node.Level > 0)
					{
						cxMenu.Show(this, p);
					}
				}
			}
		}

		void ILNodeManager_OnModuleChanged(object sender)
		{
			RebuildAsync();
		}

		// rebuild interface

		public void RebuildAsync(bool mainmodonly = false)
		{
			if (rebuildThread != null)
			{
				while (rebuildThread.IsAlive)
				{
					Application.DoEvents();
					Thread.Sleep(1);
				}
			}

			rebuildThread = new Thread(() => RebuildInternal(mainmodonly, true));
			Nodes.Clear();
			ExtensionNode.Nodes.Clear();
			if (!IsHandleCreated)
			{
				HandleCreated += (s, e) => { rebuildThread.Start(); };
				return;
			}
			rebuildThread.Start();
		}

		// TODO: Improve Rebuild technic
		public void Rebuild(bool mainmodonly = false)
		{
			RebuildInternal(mainmodonly, false);
		}

		private void RebuildInternal(bool mainmodonly, bool invoke)
		{
			if (!invoke)
			{
				Nodes.Clear();
				ExtensionNode.Nodes.Clear();
			}
			foreach (ILNode iln in dataStruct.ILNodeManager.AllModules)
			{
				if ((iln.VisibleMembers & (StructureView.Basestructure | FilterElements)) != StructureView.None)
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

		private void RecursiveRebuild(TreeNode tnParent, ILNode ilParent)
		{
			foreach (ILNode iln in ilParent.Children)
				if ((iln.VisibleMembers & (StructureView.Basestructure | FilterElements)) != StructureView.None)
				{
					TreeNode tnSub = new TreeNode(UseFullName ? iln.FullName : iln.Name);
					tnSub.Tag = iln.Value;
					tnSub.ImageIndex = tnSub.SelectedImageIndex = GetImage(iln.Value);
					RecursiveRebuild(tnSub, iln);
					tnParent.Nodes.Add(tnSub);
				}
		}

		// rebuild tools

		private delegate void _AddNode(TreeNode tn);
		private void AddNode(TreeNode tn)
		{
			Nodes.Add(tn);
		}

		private static int GetImage(object MemRef)
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

		public void AddToolboxNode(ILNode extNode)
		{
			// TODO: fix icon for special nodes (also genericparameter from EditoILPattern)
			if (!Nodes.Contains(ExtensionNode))
				Nodes.Insert(0, ExtensionNode);
			RecursiveRebuild(ExtensionNode, extNode);
		}
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