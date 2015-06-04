using ILPatcher.Data;
using Mono.Cecil.Cil;
using System;
using System.Drawing;
using System.Xml;

namespace ILPatcher.Utility
{
	public static class ExtensionMethods
	{
		private static NameCompressor nc = NameCompressor.Instance;
		private const string abc = "abcdefghijklmnopqrstuvwxyz";

		// Instruction **************************************************************

		/// <summary>Creates a shallow copy of an Instruction</summary>
		/// <param name="instr">The instruction to be cloned</param>
		/// <returns>Returns the new Instruction if ILManager.GenInstruction was successful, otherwise null</returns>
		public static Instruction Clone(this Instruction instr)
		{
			return ILManager.GenInstruction(instr.OpCode, instr.Operand);
		}

		// XmlElement ***************************************************************

		/// <summary>Creates an attribute for a XmlElement and appends it</summary>
		/// <param name="xelem">The XmlElement where the attribute should be added</param>
		/// <param name="name">The SSD-ID of the element, which will be converted with the NameCompressor and used as the name for the attribute</param>
		/// <param name="value">The value for the new attribute</param>
		public static void CreateAttribute(this XmlElement xelem, SST name, string value)
		{
			XmlAttribute NewAttribute = xelem.OwnerDocument.CreateAttribute(nc[name]);
			NewAttribute.Value = value;
			xelem.Attributes.Append(NewAttribute);
		}

		/// <summary>Creates an attribute for a XmlElement and appends it</summary>
		/// <param name="xelem">The XmlElement where the attribute should be added</param>
		/// <param name="name">The ID of the element, which will be converted ToBaseAlph and used as the name for the attribute</param>
		/// <param name="value">The value for the new attribute</param>
		[ObsoleteAttribute("Dynamic attributes are dangerous!", false)]
		public static void CreateAttribute(this XmlElement xelem, int name, string value)
		{
			XmlAttribute NewAttribute = xelem.OwnerDocument.CreateAttribute(name.ToBaseAlph());
			NewAttribute.Value = value;
			xelem.Attributes.Append(NewAttribute);
		}

		/// <summary>Returns the value of an attribute in a XmlElement</summary>
		/// <param name="xelem">The XmlElement with the attribute</param>
		/// <param name="name">SST-Name of the attribute</param>
		/// <returns>Returns the value of the attribute if the attribute exist, otherwise string.Empty</returns>
		public static string GetAttribute(this XmlElement xelem, SST name)
		{
			XmlAttribute res = xelem.Attributes[nc[name]];
			if (res != null)
				return res.Value;
			else
				return string.Empty;
		}

		/// <summary>Reads the value of an attribute in a XmlElement</summary>
		/// <param name="xelem">The XmlElement with the attribute</param>
		/// <param name="name">SST-Name of the attribute</param>
		/// <param name="value">The read value (can be string.Empty)</param>
		/// <returns>Returns true if the attribute exists, otherwise false</returns>
		public static bool GetAttribute(this XmlElement xelem, SST name, out string value)
		{
			XmlAttribute res = xelem.Attributes[nc[name]];
			if (res != null)
			{
				value = res.Value;
				return true;
			}
			else
			{
				value = string.Empty;
				return false;
			}
		}

		/// <summary>Creates a copy of the given XmlElement with all attributes and appends it. Subnodes won't be copied.
		/// This method can be used to copy a XmlElement from a another XmlDocument.</summary>
		/// <param name="xelem">The XmlElement where the copied XmlElement will be added</param>
		/// <param name="child">The XmlElement to be copied</param>
		public static void AppendClonedChild(this XmlElement xelem, XmlElement child)
		{
			XmlDocument xDoc = xelem.OwnerDocument;
			XmlElement xnew = xDoc.CreateElement(child.Name);
			foreach (XmlAttribute xatt in child.Attributes)
			{
				XmlAttribute NewAttribute = xDoc.CreateAttribute(xatt.Name);
				NewAttribute.Value = xatt.Value;
				xnew.Attributes.Append(NewAttribute);
			}
			xelem.AppendChild(xnew);
		}

		// XmlNode ******************************************************************

		/// <summary>Creates a new XmlElement with the NameCompressor SST Entry as the name.
		/// If name compression is enabled in the NameCompressor it will use the short name, otherwise the long one</summary>
		/// <param name="xelem">Any XmlNode from the used XmlDocument or the XmlDocument itself.
		/// Since the new XmlElement won't be added it doesn't matter</param>
		/// <param name="name">The SSD-ID of the element, which will be converted with the NameCompressor and used as the name for the XmlElement</param>
		/// <returns>Returns a new XmlElement with the give values</returns>
		public static XmlElement CreateCompressedElement(this XmlNode xelem, SST name)
		{
			if (xelem.NodeType == XmlNodeType.Document)
				return ((XmlDocument)xelem).CreateElement(nc[name]);
			else	//if (xelem.NodeType == XmlNodeType.Element)
				return xelem.OwnerDocument.CreateElement(nc[name]);
		}

		/// <summary>Creates a new XmlElement with the NameCompressor SST Entry as the name and appends it.
		/// If name compression is enabled in the NameCompressor it will use the short name, otherwise the long one</summary>
		/// <param name="xelem">A XmlNode or XmlDocument where the new XmlElement should be added</param>
		/// <param name="name">The SSD-ID of the element, which will be converted with the NameCompressor and used as the name for the XmlElement</param>
		/// <returns>Returns the new XmlElement with the give values</returns>
		public static XmlElement InsertCompressedElement(this XmlNode xelem, SST name)
		{
			XmlElement tmpnode;
			if (xelem.NodeType == XmlNodeType.Document)
				tmpnode = ((XmlDocument)xelem).CreateElement(nc[name]);
			else	//if (xelem.NodeType == XmlNodeType.Element)
				tmpnode = xelem.OwnerDocument.CreateElement(nc[name]);
			xelem.AppendChild(tmpnode);
			return tmpnode;
		}

		/// <summary>Creates a new XmlElement with the given ID and appends it.</summary>
		/// <param name="xelem">A XmlNode or XmlDocument where the new XmlElement should be added</param>
		/// <param name="name">The ID of the element, which will be converted ToBaseAlph and used as the name for the XmlElement</param>
		/// <returns>Returns the new XmlElement with the give values</returns>
		public static XmlElement InsertCompressedElement(this XmlNode xelem, int name)
		{
			XmlElement tmpnode;
			if (xelem.NodeType == XmlNodeType.Document)
				tmpnode = ((XmlDocument)xelem).CreateElement(name.ToBaseAlph());
			else	//if (xelem.NodeType == XmlNodeType.Element)
				tmpnode = xelem.OwnerDocument.CreateElement(name.ToBaseAlph());
			xelem.AppendChild(tmpnode);
			return tmpnode;
		}

		// Point ********************************************************************

		/// <summary>Adds the Points memberwise and returns a new Point</summary>
		/// <param name="p">The first Point</param>
		/// <param name="add">The Point to be added</param>
		/// <returns>Returns a new Point with the sum</returns>
		public static Point Add(this Point p, Point add)
		{
			return new Point(p.X + add.X, p.Y + add.Y);
		}

		// int & string *************************************************************

		/// <summary>Converts a non-negative number into an alphabetical string.</summary>
		/// <param name="n">The number to convert</param>
		/// <returns>Resturns the converted number if successful, othersiwe the given number converted with ToString</returns>
		public static string ToBaseAlph(this int n)
		{
			if (n < 0) { Log.Write(Log.Level.Error, "Couldn't convert number"); return n.ToString(); }
			string res = string.Empty;
			do
			{
				res = abc[n % 26] + res;
				n = (n / 26);
			} while (n > 0);
			return res;
		}

		/// <summary>Converts an alphabetical string into a number</summary>
		/// <param name="n">The string to convert</param>
		/// <returns>Returns the converted number</returns>
		public static int ToBaseInt(this string n)
		{
			int x = 0;
			for (int i = n.Length - 1; i >= 0; i--)
				x += Pow(26, (n.Length - 1) - i) * (n[i] - 'a');
			return x;
		}

		/// <summary>Calculates the power of two non-negative integers</summary>
		/// <param name="a">Base number</param>
		/// <param name="b">Exponent number</param>
		/// <returns>Returns the calculated value in any case; be careful with your params</returns>
		public static int Pow(int a, int b)
		{
			int ret = 1;
			while (b != 0)
			{
				if ((b & 1) == 1)
					ret *= a;
				a *= a;
				b >>= 1;
			}
			return ret;
		}

	}
}
