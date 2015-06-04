using System.Xml;

namespace ILPatcher.Data
{
	/// <summary>Defines a class to be a write-/read-able segment of a file, as a parent or subinfo node</summary>
	interface ISaveToFile
	{
		/// <summary>Saves the own data into the XMLnode as Childnodes</summary>
		/// <returns>Return true if no errors occoured during the write</returns>
		bool Save(XmlNode output);
		/// <summary>Reads the requierd data from the XMLnode's Childnodes</summary>
		/// <returns>Return true if no errors occoured during the read</returns>
		bool Load(XmlNode input);
	}
}
