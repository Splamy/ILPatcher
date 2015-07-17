using ILPatcher.Data;
using ILPatcher.Utility;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LocRes = ILPatcher.Properties.Resources;

namespace ILPatcher.Interface.General
{
	public class StructureViewer : TreeView
	{
		[DefaultValue(StructureView.all)]
		public StructureView FilterElements { get; set; }

		[DefaultValue(false)]
		public bool UseFullName { get; set; }

		[DefaultValue(false)]
		public bool ContextAssemblyLoad { get; set; }

		private DataStruct dataStruct = null;
		private TreeNode ExtensionNode = new TreeNode("Extension Node");
		private ContextMenuStrip cxMenu = new ContextMenuStrip();

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

			MouseUp += StructureViewer_MouseUp;

			cxMenu.Items.Add("Load Assembly", null, ToolStripBar_LoadAssembly_Click);
			//Controls.Add(cxMenu);
		}

		public StructureViewer(DataStruct dataAssociation)
			: this()
		{
			SetDataAssociation(dataAssociation);
		}

		public void SetDataAssociation(DataStruct dataAssociation)
		{
			if (dataAssociation == null)
				throw new ArgumentNullException("dataAssociation");

			if (dataStruct != null)
			{
				dataAssociation.OnASMFileLoadedDelegate -= dataAssociation_OnASMFileLoadedDelegate;
			}

			dataAssociation.OnASMFileLoadedDelegate += dataAssociation_OnASMFileLoadedDelegate;

			dataStruct = dataAssociation;
		}

		// events

		void ToolStripBar_LoadAssembly_Click(object sender, EventArgs e)
		{
			AssemblyDefinition asmDefToLoad = SelectedNode.Tag as AssemblyDefinition;
			if (asmDefToLoad == null)
				throw new InvalidProgramException("The node does not contain an 'AssemblyDefinition'");

			InitTree(asmDefToLoad);
			RebuildHalfAsync();
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

		void dataAssociation_OnASMFileLoadedDelegate(object sender)
		{
			InitTreeHalfAsync(dataStruct.AssemblyDefinition);
			RebuildHalfAsync();
		}

		// rebuild interface

		public void RebuildHalfAsync(bool mainmodonly = false)
		{
			System.Threading.Thread t = new System.Threading.Thread(() => RebuildInternal(mainmodonly, true));
			Nodes.Clear();
			ExtensionNode.Nodes.Clear();
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
			{
				Nodes.Clear();
				ExtensionNode.Nodes.Clear();
			}
			foreach (ILNode iln in getAllNodes())
			{
				if ((iln.Flags & (StructureView.basestructure | FilterElements)) != StructureView.none)
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
				if ((iln.Flags & (StructureView.basestructure | FilterElements)) != StructureView.none)
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

		public void AddToolBoxNode(ILNode extNode)
		{
			// TODO: fix icon for special nodes (also genericparameter from EditoILPattern)
			if (!Nodes.Contains(ExtensionNode))
				Nodes.Insert(0, ExtensionNode);
			RecursiveRebuild(ExtensionNode, extNode);
		}

		// ILNode Methods

		/// <summary>Calls the InitTree method in a seperate Thread and waits for it to finish.
		/// This will prevent the interface from freezing.</summary>
		/// <param name="AssDef">The AssemblyDefinition which should be loaded into the searchlist</param>
		/// <param name="SubResolveDepth">When the given AssemblyDefinition uses references to other Assemblys
		/// the method will add then recursivly to the given depth</param>
		public void InitTreeHalfAsync(AssemblyDefinition AssDef, int SubResolveDepth = 0)
		{
			Thread t = new Thread(() => InitTree(AssDef, SubResolveDepth));
			t.Start();
			while (t.IsAlive)
			{
				Application.DoEvents();
				Thread.Sleep(1);
			}
		}

		/// <summary>Creates an ILNode-Tree representing the structure of the given Assembly
		/// and stores it in the ModuleList Dictionary with the AssemblyDefinition name as key.</summary>
		/// <param name="AssDef">The AssemblyDefinition which should be loaded into the searchlist</param>
		/// <param name="SubResolveDepth">When the given AssemblyDefinition uses references to other Assemblys
		/// the method will add them recursivly to the given depth</param>
		public void InitTree(AssemblyDefinition AssDef, int SubResolveDepth = 0)
		{
			if (AssDef == null) return;
			if (dataStruct.ModuleList.ContainsKey(AssDef.Name.Name)) return;

			ILNode ilParent = new ILNode(AssDef.Name.Name, AssDef.FullName, AssDef, StructureView.structure); // StructureView.Module
			dataStruct.ModuleList.Add(AssDef.Name.Name, ilParent);

			foreach (ModuleDefinition ModDef in AssDef.Modules)
			{
				ILNode tnModDef = ilParent.Add(ModDef.Name, ModDef.Name, ModDef, StructureView.structure);
				DefaultAssemblyResolver dar = ModDef.AssemblyResolver as DefaultAssemblyResolver;
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
							InitTree(AssSubRef, SubResolveDepth - 1);
					}
					catch { Log.Write(Log.Level.Warning, "AssemblyReference \"", anr.Name, "\" couldn't be found for \"", ModDef.Name, "\""); }
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

		/// <summary>Returns a collection of all loaded ILNode Assemblys</summary>
		/// <returns>Returns a ILNode Assembly collection</returns>
		public ICollection<ILNode> getAllNodes()
		{
			return dataStruct.ModuleList.Values;
		}

		/// <summary>Searches for the Cecil Typ/Met/Fld/... matching the seatch path in the loaded ILNode ModuleList</summary>
		/// <param name="path">A path of the form "asseblyname.namespace.class" or "-.namespace.class.method" to search all assemblys</param>
		/// <returns>Returns the Cecil object if found, otherwise null</returns>
		public object FindTypeByName(string path)
		{
			string[] pathbreaks = path.Split(new[] { '.', '/' });

			if (pathbreaks.Length == 0)
			{
				Log.Write(Log.Level.Warning, "FindTypeByName path is empty");
				return null;
			}

			if (pathbreaks[0] == "-")
			{
				foreach (ILNode child in dataStruct.ModuleList.Values)
				{
					object res = FindTypeByNameRecursive(child, pathbreaks, 1);
					if (res != null) return res;
				}
			}
			else if (dataStruct.ModuleList.ContainsKey(pathbreaks[0]))
			{
				return FindTypeByNameRecursive(dataStruct.ModuleList[pathbreaks[0]], pathbreaks, 1);
			}
			return null;
		}

		/// <summary>Traverses the loaded ILNode children and searches the current path index in its children</summary>
		/// <param name="searchnode">The ILNode with the children for the current path index</param>
		/// <param name="path">The array of all path parts</param>
		/// <param name="index">The current path index</param>
		/// <returns></returns>
		private object FindTypeByNameRecursive(ILNode searchnode, string[] path, int index)
		{
			if (index >= path.Length) return null;

			foreach (ILNode child in searchnode.Children)
			{
				if (child.Name == path[index])
				{
					if (index == path.Length - 1) return child.Value;
					else return FindTypeByNameRecursive(child, path, index + 1);
				}
			}
			return null;
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