namespace ILPatcher.Data
{
	public abstract class NamedElement
	{
		public string Name { get; set; }
		public abstract string Label { get; }
		public abstract string Description { get; }
	}
}
