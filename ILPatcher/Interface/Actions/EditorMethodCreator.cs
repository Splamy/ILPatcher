using ILPatcher.Data;
using ILPatcher.Data.Actions;
using MetroObjects;
using Mono.Cecil;
using System;
using System.Drawing;

namespace ILPatcher.Interface.Actions
{
	[EditorAttributes("Method-Creator")]
	public partial class EditorMethodCreator : EditorPatchAction<PatchActionMethodCreator>
	{
		private MethodDefinition blankMethodDefinition;
		private TypeDefinition insertClass;

		public override bool FixedHeight => false;
		public override int DefaultHeight => 300;

		//https://github.com/PavelTorgashov/FastColoredTextBox

		public EditorMethodCreator(DataStruct dataStruct)
			: base(dataStruct)
		{
			InitializeComponent();

			Action<TypeReference> addMethod = tr => { lbxParameter.AddItem(new DefaultDragItem<TypeReference>(tr)); };

			arcParameter.OnAdd = () =>
			{
				using (CreateTypeForm ctf = new CreateTypeForm(dataStruct, addMethod))
				{
					ctf.Show();
				}
			};
			arcParameter.OnEdit = () =>
			{
				using (CreateTypeForm ctf = new CreateTypeForm(dataStruct, addMethod))
				{
					ctf.SetTypeReference(((DefaultDragItem<TypeReference>)lbxParameter.SelectedElement).Item);
					ctf.Show();
				}
			};
			arcParameter.OnRemove = () => lbxParameter.RemoveSelected();
		}

		private void btnPickMethod_Click(object sender, EventArgs e)
		{

		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (myData == null)
			{
				myData = new PatchActionMethodCreator(dataStruct);
				myData.Name = txtPatchActionName.Text;
				dataStruct.PatchActionList.Add(myData);
			}
			SwooshBack();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//CSCompiler csc = new CSCompiler(assemblyDefinition);
			//MethodDefinition md = csc.InjectCode(txtInjectCode.Text);
		}

		protected override void OnPatchDataSet()
		{
			// TODO: visualize
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
