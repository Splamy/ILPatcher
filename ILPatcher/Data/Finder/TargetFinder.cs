using System.Xml;
using System;

namespace ILPatcher.Data.Finder
{
	public abstract class TargetFinder : NamedElement, ISaveToFile
	{
		protected DataStruct dataStruct;

		public abstract TargetFinderType TargetFinderType { get; }
		/// <summary><para>True if the output is already known during patch creation.</para>
		/// <para>False if the result contains multiple locations, is not needed more precise or similar.</para></summary>
		public abstract bool HasFixedOutput { get; }
		/// <summary><para>True implies that the resulttype will be returned as IList&lt;TOutput&gt;.</para>
		/// <para>False implies that the result will returned as-is.</para></summary>
		public abstract bool HasMultipleResults { get; }
		public abstract Type TInput { get; }
		public abstract Type TOutput { get; }

		public abstract object FilterInput(object input);

		public abstract bool Save(XmlNode output);
		public abstract bool Load(XmlNode input);

		public TargetFinder(DataStruct dataStruct)
		{
			this.dataStruct = dataStruct;
		}
	}

	public enum TargetFinderType
	{
		ClassByName,
	}
}
