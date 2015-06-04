using Mono.Cecil;
using System;

namespace ILPatcher.Data.Actions
{
	public class PatchActionMethodCreator : PatchAction // TODO move to Finder
	{
		public override PatchActionType PatchActionType { get { return PatchActionType.ILMethodCreator; } protected set { } }

		public PatchAction FillAction { get; set; }

		private MethodDefinition blankMethodDefinition;
		private TypeDefinition insertClass;

		public override bool Execute()
		{
			throw new NotImplementedException();
		}

		public override bool Load(System.Xml.XmlNode input)
		{
			throw new NotImplementedException();
		}

		public override bool Save(System.Xml.XmlNode output)
		{
			throw new NotImplementedException();
		}
	}
}
