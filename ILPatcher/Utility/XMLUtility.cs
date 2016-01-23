using System.IO;
using System.Text;
using System.Xml;

namespace ILPatcher.Utility
{
	static class XMLUtility
	{
		public static void SaveToFile(XmlDocument doc, string FileName)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = !NameCompressor.Compress;
			settings.IndentChars = "\t";
			settings.NewLineChars = "\r\n";
			settings.NewLineHandling = NewLineHandling.Replace;
			settings.Encoding = Encoding.UTF8;
			using (FileStream fs = new FileStream(FileName, FileMode.Create))
			{
				XmlWriter xWriter = XmlWriter.Create(fs, settings);
				doc.Save(xWriter);
			}
		}

		public static XmlDocument ReadFromFile(string FileName)
		{
			using (FileStream fs = new FileStream(FileName, FileMode.Open))
			{
				XmlReader xReader = XmlReader.Create(fs);
				XmlDocument xDoc = new XmlDocument();
				xDoc.Load(xReader);
				return xDoc;
			}
		}
	}
}
