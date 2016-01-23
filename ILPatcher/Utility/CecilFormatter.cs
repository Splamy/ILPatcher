using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Text;

namespace ILPatcher.Utility
{
	internal static class CecilFormatter
	{
		private static string zeroformat = string.Empty;

		public static string TryFormat(object objref)
		{
			if (objref == null)
				return string.Empty;

			VariableReference vr = objref as VariableReference;
			if (vr != null) return Format((VariableReference)objref);

			ParameterReference pr = objref as ParameterReference;
			if (pr != null) return Format((ParameterReference)objref);

			ParameterReference instr = objref as ParameterReference;
			if (instr != null) return Format((Instruction)objref);

			if (objref is MemberReference) return Format((MemberReference)objref);

			return objref.ToString();
		}

		public static string Format(VariableReference varref)
		{
			StringBuilder strb = new StringBuilder();
			strb.Append("Var");
			strb.Append(varref.Index);
			if (string.IsNullOrEmpty(varref.Name))
			{
				strb.Append(" (");
				strb.Append(varref.Name);
				strb.Append(')');
			}
			strb.Append(" : ");
			strb.Append(varref.VariableType.FullName);
			return strb.ToString();
		}

		public static string Format(ParameterReference varref)
		{
			StringBuilder strb = new StringBuilder();
			strb.Append(varref.Name);
			strb.Append(" : ");
			strb.Append(varref.ParameterType.FullName);
			return strb.ToString();
		}

		public static string Format(Instruction instr, int pos = -1)
		{
			StringBuilder strb = new StringBuilder();
			if (pos != -1)
			{
				strb.Append("# ");
				if (string.IsNullOrEmpty(zeroformat))
					strb.Append(pos);
				else
					strb.Append(pos.ToString(zeroformat));
				strb.Append(" | ");
			}
			strb.Append(instr.OpCode.Name);
			if (instr.Operand != null)
			{
				strb.Append(" -> ");
				if (instr.Operand is Instruction)
					strb.Append(instr.Operand.ToString());
				else
					strb.Append(TryFormat(instr.Operand));
			}
			return strb.ToString();
		}

		public static string Format(MemberReference memref)
		{
			return memref.FullName;
		}

		public static void SetMaxNumer(int max)
		{
			zeroformat = 'D' + max.ToString().Length.ToString();
		}

		public static void ClearMaxNumer()
		{
			zeroformat = string.Empty;
		}
	}
}
