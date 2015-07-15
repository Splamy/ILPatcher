using Mono.Cecil;
using System.Xml;

namespace ILPatcher.Data.Finder
{
	public abstract class TargetFinder : ISaveToFile
	{
		public abstract string Description { get; }

		public abstract void FindTargets(AssemblyDefinition assemblyDefinition);

		public abstract bool GetTarget<T>(out T target);

		public abstract bool Save(XmlNode output);
		public abstract bool Load(XmlNode input);
	}

	public class PatchTarget
	{
		public bool IsArray { get; set; }
		public bool IsExplicit { get; set; }

		public object Target { get; set; }
	}
}
