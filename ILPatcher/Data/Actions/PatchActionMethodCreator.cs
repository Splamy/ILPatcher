using Mono.Cecil;
using System;
using System.Xml;

namespace ILPatcher.Data.Actions
{
	public class PatchActionMethodCreator : PatchAction // TODO move to Finder
	{
		//I: NamedElement
		public override string Label { get { return ((FillAction == null) ? "<?>" : FillAction.Name) + " -> " + ((insertClass == null) ? "<?>" : insertClass.FullName); } }
		public override string Description { get { throw new NotImplementedException(); } }

		//I: PatchAction
		public override PatchActionType PatchActionType { get { return PatchActionType.ILMethodCreator; } }
		public override bool RequiresFixedOutput { get { return false; } }
		public override Type TInput { get { return typeof(TypeDefinition); } }

		//Own:
		public PatchAction FillAction { get; set; }
		private MethodDefinition blankMethodDefinition;
		private TypeDefinition insertClass;

		public PatchActionMethodCreator(DataStruct dataStruct)
			: base(dataStruct)
		{

		}

		public override void PassTarget(object target)
		{
			insertClass = (TypeDefinition)target;
		}

		public override bool Execute(object target)
		{
			throw new NotImplementedException();
		}

		public override bool Save(XmlNode output)
		{
			throw new NotImplementedException();
		}

		public override bool Load(XmlNode input)
		{
			throw new NotImplementedException();
		}
	}
}
