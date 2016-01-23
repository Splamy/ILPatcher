using Mono.Cecil;
using System;
using System.Xml;

namespace ILPatcher.Data.Actions
{
	public class PatchActionMethodCreator : PatchAction // TODO move to Finder
	{
		//I: NamedElement
		public override string Label => (FillAction?.Name ?? "<?>") + " -> " + (insertClass?.FullName ?? "<?>");
		public override string Description => "Creates and inserts a new method with the option to implement it";

		//I: PatchAction
		public override PatchActionType PatchActionType => PatchActionType.ILMethodCreator;
		public override bool RequiresFixedOutput => false;
		public override Type TInput => typeof(TypeDefinition);

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
