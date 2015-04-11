using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	/// <summary>
	/// Taken from Reflexil, See https://github.com/sailro/reflexil
	/// </summary>
	public static class CecilHelper
	{
		private const System.Reflection.BindingFlags privateflags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

		public static TypeDefinition FindMatchingType(ICollection<TypeDefinition> types, string fulltypename)
		{
			foreach (var ttype in types)
			{
				if (fulltypename == ttype.FullName)
					return ttype;

				var ittype = FindMatchingType(ttype.NestedTypes, fulltypename);
				if (ittype != null)
					return ittype;
			}
			return null;
		}

		public static TypeDefinition FindMatchingType(ModuleDefinition mdef, string fulltypename)
		{
			return FindMatchingType(mdef.Types, fulltypename);
		}

		/// <summary>
		/// Find a similar field in the given type definition 
		/// </summary>
		/// <param name="tdef">Type definition</param>
		/// <param name="fref">Field reference</param>
		/// <returns>Field definition (or null if not found)</returns>
		public static FieldDefinition FindMatchingField(TypeDefinition tdef, FieldReference fref)
		{
			return tdef.Fields.FirstOrDefault(fdef => (fdef.Name == fref.Name) && (fdef.FieldType.FullName == fref.FieldType.FullName));
		}

		/// <summary>
		/// Determines if two assembly name references matches
		/// </summary>
		/// <param name="anref1">an assembly name reference</param>
		/// <param name="anref2">an assembly name reference to compare</param>
		/// <returns>true if matches</returns>
		public static bool ReferenceMatches(AssemblyNameReference anref1, AssemblyNameReference anref2)
		{
			// Skip Key
			return ((anref1.Name == anref2.Name) &&
					(String.Compare(anref1.Version.ToString(2), anref2.Version.ToString(2), StringComparison.Ordinal) == 0) &&
					(anref1.Culture == anref2.Culture));
		}

		/// <summary>
		/// Determines if two methods matches
		/// </summary>
		/// <param name="mref1">a method</param>
		/// <param name="mref2">a method to compare</param>
		/// <returns>true if matches</returns>
		private static bool MethodMatches(MethodReference mref1, MethodReference mref2)
		{
			if ((mref1.Name != mref2.Name) || (mref1.Parameters.Count != mref2.Parameters.Count) ||
				(mref1.ReturnType.FullName != mref2.ReturnType.FullName))
				return false;

			for (var i = 0; i <= mref1.Parameters.Count - 1; i++)
			{
				if (mref1.Parameters[i].ParameterType.FullName != mref2.Parameters[i].ParameterType.FullName)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Find a similar method in the given type definition 
		/// </summary>
		/// <param name="tdef">Type definition</param>
		/// <param name="mref">Method reference</param>
		/// <returns>Method definition (or null if not found)</returns>
		public static MethodDefinition FindMatchingMethod(TypeDefinition tdef, MethodReference mref)
		{
			return tdef.Methods.FirstOrDefault(mdef => MethodMatches(mdef, mref));
		}

		internal static Instruction GetInstruction(MethodBody oldBody, MethodBody newBody, Instruction i)
		{
			int pos = oldBody.Instructions.IndexOf(i);
			if (pos > -1 && pos < newBody.Instructions.Count)
				return newBody.Instructions[pos];

			Instruction ni = Instruction.Create(OpCodes.Nop);
			typeof(Instruction).GetField("offset", privateflags).SetValue(ni, int.MaxValue);
			return ni;
		}

		private static TypeReference FixTypeImport(ModuleDefinition context, MethodDefinition source, MethodDefinition target,
			TypeReference type)
		{
			if (type.FullName == source.DeclaringType.FullName)
				return target.DeclaringType;

			return context.Import(type);
		}

		private static FieldReference FixFieldImport(ModuleDefinition context, MethodDefinition source,
			MethodDefinition target, FieldReference field)
		{
			if (field.DeclaringType.FullName == source.DeclaringType.FullName)
				return FindMatchingField(target.DeclaringType, field);

			return context.Import(field);
		}

		private static MethodReference FixMethodImport(ModuleDefinition context, MethodDefinition source,
			MethodDefinition target, MethodReference method)
		{
			if (method.DeclaringType.FullName == source.DeclaringType.FullName)
				return FindMatchingMethod(target.DeclaringType, method);

			return context.Import(method);
		}

		private static MethodBody CloneMethodBody(MethodBody body, MethodDefinition source, MethodDefinition target)
		{
			var context = target.DeclaringType.Module;
			var nb = new MethodBody(target)
			{
				MaxStackSize = body.MaxStackSize,
				InitLocals = body.InitLocals,
			};
			typeof(MethodBody).GetField("code_size", privateflags).SetValue(nb, body.CodeSize);

			var worker = nb.GetILProcessor();

			foreach (var var in body.Variables)
				nb.Variables.Add(new VariableDefinition(
					var.Name, FixTypeImport(context, source, target, var.VariableType)));

			foreach (var instr in body.Instructions)
			{
				Instruction ni = Instruction.Create(OpCodes.Nop);
				ni.OpCode = instr.OpCode;
				ni.Operand = OpCodes.Nop;

				switch (instr.OpCode.OperandType)
				{
				case OperandType.InlineArg:
				case OperandType.ShortInlineArg:
					if (instr.Operand == body.ThisParameter)
						ni.Operand = nb.ThisParameter;
					else
					{
						var param = body.Method.Parameters.IndexOf((ParameterDefinition)instr.Operand);
						ni.Operand = target.Parameters[param];
					}
					break;
				case OperandType.InlineVar:
				case OperandType.ShortInlineVar:
					var var = body.Variables.IndexOf((VariableDefinition)instr.Operand);
					ni.Operand = nb.Variables[var];
					break;
				case OperandType.InlineField:
					ni.Operand = FixFieldImport(context, source, target, (FieldReference)instr.Operand);
					break;
				case OperandType.InlineMethod:
					ni.Operand = FixMethodImport(context, source, target, (MethodReference)instr.Operand);
					break;
				case OperandType.InlineType:
					ni.Operand = FixTypeImport(context, source, target, (TypeReference)instr.Operand);
					break;
				case OperandType.InlineTok:
					if ((instr.Operand) is TypeReference)
						ni.Operand = FixTypeImport(context, source, target, (TypeReference)instr.Operand);
					else if ((instr.Operand) is FieldReference)
						ni.Operand = FixFieldImport(context, source, target, (FieldReference)instr.Operand);
					else if ((instr.Operand) is MethodReference)
						ni.Operand = FixMethodImport(context, source, target, (MethodReference)instr.Operand);
					break;
				case OperandType.ShortInlineBrTarget:
				case OperandType.InlineBrTarget:
				case OperandType.InlineSwitch:
					break;
				default:
					ni.Operand = instr.Operand;
					break;
				}

				worker.Append(ni);
			}

			for (var i = 0; i < body.Instructions.Count; i++)
			{
				var instr = nb.Instructions[i];
				var oldi = body.Instructions[i];

				switch (instr.OpCode.OperandType)
				{
				case OperandType.InlineSwitch:
					{
						var olds = (Instruction[])oldi.Operand;
						var targets = new Instruction[olds.Length];

						for (var j = 0; j < targets.Length; j++)
							targets[j] = GetInstruction(body, nb, olds[j]);

						instr.Operand = targets;
					}
					break;
				case OperandType.InlineBrTarget:
				case OperandType.ShortInlineBrTarget:
					instr.Operand = GetInstruction(body, nb, (Instruction)oldi.Operand);
					break;
				}
			}

			foreach (var eh in body.ExceptionHandlers)
			{
				var neh = new ExceptionHandler(eh.HandlerType)
				{
					TryStart = GetInstruction(body, nb, eh.TryStart),
					TryEnd = GetInstruction(body, nb, eh.TryEnd),
					HandlerStart = GetInstruction(body, nb, eh.HandlerStart),
					HandlerEnd = GetInstruction(body, nb, eh.HandlerEnd)
				};

				switch (eh.HandlerType)
				{
				case ExceptionHandlerType.Catch:
					neh.CatchType = FixTypeImport(context, source, target, eh.CatchType);
					break;
				case ExceptionHandlerType.Filter:
					neh.FilterStart = GetInstruction(body, nb, eh.FilterStart);
					break;
				}

				nb.ExceptionHandlers.Add(neh);
			}

			return nb;
		}

		/// <summary>
		/// Clone a source method body to a target method definition.
		/// Field/Method/Type references are corrected
		/// </summary>
		/// <param name="source">Source method definition</param>
		/// <param name="target">Target method definition</param>
		public static void CloneMethodBody(MethodDefinition source, MethodDefinition target)
		{
			var newBody = CloneMethodBody(source.Body, source, target);
			target.Body = newBody;
		}
	}
}
