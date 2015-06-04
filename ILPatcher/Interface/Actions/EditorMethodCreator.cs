using ILPatcher.Data.Actions;
using ILPatcher.Interface.General;
using MetroObjects;
using Mono.Cecil;
using System;
using System.Drawing;

namespace ILPatcher.Interface.Actions
{
	public partial class EditorMethodCreator : EditorPatchAction
	{
		public override string PanelName { get { return "PatchAction: ILMethodCreator"; } }

		private PatchActionMethodCreator patchAction;
		private MethodDefinition blankMethodDefinition;
		private TypeDefinition insertClass;
		private AssemblyDefinition assemblyDefinition;

		//https://github.com/PavelTorgashov/FastColoredTextBox

		public EditorMethodCreator(Action<PatchAction> pParentAddCallback, AssemblyDefinition pAssemblyDefinition)
			: base(pParentAddCallback)
		{
			InitializeComponent();

			assemblyDefinition = pAssemblyDefinition;

			arcParameter.OnAdd = () =>
			{
				CreateTypeForm ctf = new CreateTypeForm(assemblyDefinition, tr =>
				{
					lbxParameter.AddItem(new DefaultDragItem<TypeReference>(tr));
				});
				ctf.Show();
			};
			arcParameter.OnEdit = () =>
			{
				CreateTypeForm ctf = new CreateTypeForm(assemblyDefinition, tr =>
				{
					lbxParameter.AddItem(new DefaultDragItem<TypeReference>(tr));
				});
				ctf.SetTypeReference(((DefaultDragItem<TypeReference>)lbxParameter.SelectedElement).Item);
				ctf.Show();
			};
			arcParameter.OnRemove = () => lbxParameter.RemoveSelected();
		}

		private void btnPickMethod_Click(object sender, EventArgs e)
		{

		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (patchAction == null)
			{
				patchAction = new PatchActionMethodCreator();
				patchAction.ActionName = string.Format("MethodCreator: <{0}> inserts {1} -> {2}",
					txtPatchActionName.Text,
					blankMethodDefinition == null ? "<>" : blankMethodDefinition.Name,
					insertClass == null ? "<>" : insertClass.Name);
				ParentAddCallback(patchAction);
			}
			((SwooshPanel)Parent).SwooshBack();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//CSCompiler csc = new CSCompiler(assemblyDefinition);
			//MethodDefinition md = csc.InjectCode(txtInjectCode.Text);
		}

		public void SetAssDef(AssemblyDefinition myAssDef)
		{
			assemblyDefinition = myAssDef;
		}

		public override void SetPatchAction(PatchAction pPatchAction)
		{
			PatchActionMethodCreator pamc = (PatchActionMethodCreator)pPatchAction;
			patchAction = pamc;
		}

		private void button2_Click(object sender, EventArgs e)
		{

		}
	}

	class DefaultDragItem<T> : DragItem where T : class
	{
		public T Item { get; set; }

		public DefaultDragItem(T pItem)
		{
			Item = pItem;
		}

		protected override void DrawBuffer(Graphics g)
		{
			g.DrawString(Item.ToString(), Font, Brushes.Black, new PointF(1, 1));
		}

		protected override int GetHeightFromWidth(int width)
		{
			Graphics g = this.GetBufferGraphics();
			return (int)g.MeasureString(Item.ToString(), Font, width).Height;
		}
	}
}
