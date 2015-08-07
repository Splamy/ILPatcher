using ILPatcher.Utility;
using Mono.Cecil;
using System;
using System.Xml;

namespace ILPatcher.Data.Finder
{
	class TargetFinderClassByName : TargetFinder
	{
		//I: NamedElement
		public override string Label { get { return ILNodePath; } }
		public override string Description { get { return "Finds a class descripted with the ILNodePath."; } }

		//I: TargetFinder
		public override TargetFinderType TargetFinderType { get { return TargetFinderType.ClassByName; } }
		public override bool HasFixedOutput { get { return true; } }
		public override bool HasMultipleResults { get { return false; } }
		public override Type TInput { get { return typeof(AssemblyDefinition); } }
		public override Type TOutput { get { return typeof(TypeDefinition); } }

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
			NameCompressor nc = NameCompressor.Instance;
			var xmlPathElement = output.InsertCompressedElement(SST.ILNodePath);
			xmlPathElement.Value = ILNodePath;
			return true;
		}

		public override bool Load(XmlNode input)
		{
			Validator val = new Validator();
			NameCompressor nc = NameCompressor.Instance;
			var xmlPathElement = input.GetChildNode(SST.ILNodePath, 0);
			val.ValidateSet(xmlPathElement, () => "No ILNodePath element found!");
			if (!val.Ok) return false;
			ILNodePath = xmlPathElement.Value;
			return val.Ok;
		}
	}
}
