using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace ILPatcher
{
	public class Log
	{
		private static Log instance;
		private static Log Instance
		{
			get { if (instance == null) instance = new Log(); return instance; }
			set { }
		}

		public static bool Active = true;
		private static int level = 2;
		public static Action<ErrorLoggerItem> callback { get; set; }

		private string[] spaceup;

		public Log()
		{
			string[] earr = Enum.GetNames(typeof(Level));
			int longestelem = 0;
			for (int i = 0; i < earr.Length; i++)
				if (earr[i].Length > longestelem)
					longestelem = earr[i].Length;
			spaceup = new string[earr.Length];
			StringBuilder strb = new StringBuilder(longestelem + 1);
			for (int i = 0; i < earr.Length; i++)
			{
				strb.Append(' ', longestelem - earr[i].Length);
				strb.Append(earr[i]);
				strb.Append(": ");
				spaceup[i] = strb.ToString();
				strb.Clear();
			}
		}

		public static void Write(Level lvl, string errText, params string[] infos)
		{
			if (!Active) return;
			StringBuilder strb = new StringBuilder();
			strb.Append(errText);
			foreach (string s in infos)
				strb.Append(s);
			string inputbuffer = strb.ToString();
			strb.Clear();

			strb.Append(Instance.spaceup[(int)lvl]);
			for (int i = level; i >= 1; i--)
			{
				StackFrame frame = new StackFrame(i);
				var method = frame.GetMethod();
				var type = method.DeclaringType;
				strb.Append(type.Name);
				strb.Append(".");
				strb.Append(method.Name);
				if (i > 1)
					strb.Append(">");
				else
					strb.Append(": ");
			}
			string stackbuffer = strb.ToString();
			strb.Append(inputbuffer);
			strb.Append("\r\n");
			if (callback != null) callback(new ErrorLoggerItem(lvl, inputbuffer));
			File.AppendAllText("Output.log", strb.ToString(), Encoding.UTF8);
		}

		public enum Level : int
		{
			Info,
			Careful,
			Warning,
			Error,
		}
	}
}
