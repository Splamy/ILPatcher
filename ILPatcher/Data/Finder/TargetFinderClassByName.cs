using ILPatcher.Utility;
using Mono.Cecil;
using System;
using System.Xml;

namespace ILPatcher.Data.Finder
{
	class TargetFinderClassByName : TargetFinder
	{
		//I: NamedElement
		public override string Label => ILNodePath;
		public override string Description => "Finds a class descripted with the ILNodePath.";

		//I: TargetFinder
		public override TargetFinderType TargetFinderType => TargetFinderType.ClassByName;
		public override bool HasFixedOutput => true;
		public override bool HasMultipleResults => false;
		public override Type TInput => typeof(AssemblyDefinition);
		public override Type TOutput => typeof(TypeDefinition);

		//Own:
		public string ILNodePath { get; set; }

		public TargetFinderClassByName(DataStruct dataStruct) : base(dataStruct)
		{

		}

		public override object FilterInput(object input)
		{
			object targetElement = dataStruct.ILNodeManager.FindMemberByPath(ILNodePath);
			TypeDefinition optionalTypeDefinition = targetElement as TypeDefinition;
			if (optionalTypeDefinition == null)
				throw new TargetNotFoundException(this);
			else
				return optionalTypeDefinition;
		}

		public override bool Save(XmlNode output)
		{
			var xmlPathElement = output.InsertCompressedElement(SST.ILNodePath);
			xmlPathElement.Value = ILNodePath;
			return true;
		}

		public override bool Load(XmlNode input)
		{
			Validator val = new Validator();
			var xmlPathElement = input.GetChildNode(SST.ILNodePath, 0);
			if (!val.ValidateSet(xmlPathElement, () => "No ILNodePath element found!")) return false;

			ILNodePath = xmlPathElement.Value;
			return val.Ok;
		}
	}
}
