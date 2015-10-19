using System.Xml;

namespace ILPatcher.Data
{
	public abstract class EntryBase : NamedElement, ISaveToFile
	{
		public DataStruct DataStruct { get; }

		public abstract EntryKind EntryKind { get; }

		public abstract bool Save(XmlNode output);
		public abstract bool Load(XmlNode input);

		protected EntryBase(DataStruct dataStruct)
		{
			this.DataStruct = dataStruct;
		}
	}

	public enum EntryKind
	{
		Unknown,
		PatchEntry,
		PatchAction,
		TargetFinder,
	}
}
