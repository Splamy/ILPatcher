using System;
using System.Text;
using System.Xml;
using System.IO;

namespace ILPatcher
{
	class XMLUtility
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
				using (XmlWriter writer = XmlWriter.Create(fs, settings))
				{
					doc.Save(writer);
				}
			}
		}

		public static XmlDocument ReadFromFile(string FileName)
		{
			using (FileStream fs = new FileStream(FileName, FileMode.Open))
			{
				using (XmlReader xReader = XmlReader.Create(fs))
				{
					XmlDocument xDoc = new XmlDocument();
					xDoc.Load(xReader);
					return xDoc;
				}
			}
		}
	}
}
