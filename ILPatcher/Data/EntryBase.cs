using System.Xml;

namespace ILPatcher.Data
{
	public abstract class EntryBase : NamedElement, ISaveToFile
	{
		public DataStruct dataStruct { get; }

		public abstract EntryKind EntryKind { get; }

		public abstract bool Save(XmlNode output);
		public abstract bool Load(XmlNode input);

		public EntryBase(DataStruct dataStruct)
		{
			this.dataStruct = dataStruct;
		}
	}

	public enum EntryKind
	{
		Unknown,
		PatchAction,
		TargetFinder,
	}
}
