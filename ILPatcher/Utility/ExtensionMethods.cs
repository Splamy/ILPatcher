using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public static class ExtensionMethods
	{
		public static Instruction Clone(this Instruction instr)
		{
			return ILManager.GenInstruction(instr.OpCode, instr.Operand);
		}

		private static NameCompressor nc = NameCompressor.Instance;
		public static void CreateAttribute(this XmlElement xelem, SST name, string value)
		{
			XmlAttribute NewAttribute = xelem.OwnerDocument.CreateAttribute(nc[name]);
			NewAttribute.Value = value;
			xelem.Attributes.Append(NewAttribute);
		}

		public static void CreateAttribute(this XmlElement xelem, int name, string value)
		{
			XmlAttribute NewAttribute = xelem.OwnerDocument.CreateAttribute(name.ToBaseAlph());
			NewAttribute.Value = value;
			xelem.Attributes.Append(NewAttribute);
		}

		public static string GetAttribute(this XmlElement xelem, SST name)
		{
			XmlAttribute res = xelem.Attributes[nc[name]];
			if (res != null)
				return res.Value;
			else
				return string.Empty;
			//Log.Write(Log.Level.Warning, "Attribute (", name.ToString(), ") not found in ", xelem.Name);
		}

		public static Point Add(this Point p, Point add)
		{
			return new Point(p.X + add.X, p.Y + add.Y);
		}

		public static XmlElement CreateCompressedElement(this XmlNode xelem, SST name)
		{
			if (xelem.NodeType == XmlNodeType.Document)
				return ((XmlDocument)xelem).CreateElement(nc[name]);
			else	//if (xelem.NodeType == XmlNodeType.Element)
				return xelem.OwnerDocument.CreateElement(nc[name]);
		}

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

		private const string abc = "abcdefghijklmnopqrstuvwxyz";
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

		public static int ToBaseInt(this string n)
		{
			int x = 0;
			for (int i = n.Length - 1; i >= 0; i--)
				x += Pow(26, (n.Length - 1) - i) * (n[i] - 'a');
			return x;
		}

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
