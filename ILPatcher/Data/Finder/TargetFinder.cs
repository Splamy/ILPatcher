using Mono.Cecil;

namespace ILPatcher.Data.Finder
{
	public abstract class TargetFinder
	{
		public abstract string Description { get; }

		public abstract void FindTargets(AssemblyDefinition assemblyDefinition);

		public abstract bool GetTarget<T>(out T target);
	}

	public class PatchTarget
	{
		public bool IsArray { get; set; }
		public bool IsExplicit { get; set; }

		public object Target { get; set; }
	}
}
