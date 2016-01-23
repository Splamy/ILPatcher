using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ILPatcher.Utility
{
	public static class Log
	{
		public static bool Active { get; set; }
		public static int StackLevel { get; set; }
		public static Action<ErrorLoggerItem> callback { get; set; } // TODO make callback -> event

		private static string[] spaceup;

		static Log()
		{
			StackLevel = 2;
			Active = true;

			CalcSpaceLength();
		}

		private static void CalcSpaceLength()
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

		public static void Write(Level lvl, string errText)
		{
			if (!Active) return;
			StringBuilder strb = new StringBuilder();
			strb.Append(spaceup[(int)lvl]);
			for (int i = StackLevel; i >= 1; i--)
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
			strb.Append(errText);
			strb.Append("\r\n");
			callback?.Invoke(new ErrorLoggerItem(lvl, errText));
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
