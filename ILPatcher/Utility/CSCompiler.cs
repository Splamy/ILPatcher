using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System.Windows.Forms;
using System.Reflection;
using System.CodeDom.Compiler;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public class CSCompiler
	{
		AssemblyDefinition AssDef = null;

		public CSCompiler(AssemblyDefinition mainAssDef)
		{
			AssDef = mainAssDef;
		}

		public MethodDefinition InjectCode(string code)
		{
			CompilerParameters cp = new CompilerParameters();

			if (AssDef != null)
			{
				DefaultAssemblyResolver dar = new DefaultAssemblyResolver();
				foreach (AssemblyNameReference anr in AssDef.MainModule.AssemblyReferences)
				{
					AssemblyDefinition adref = dar.Resolve(anr);
					cp.ReferencedAssemblies.Add(adref.MainModule.FullyQualifiedName);
				}
			}

			cp.WarningLevel = 3;
			cp.CompilerOptions = "/target:library"; // /optimize
			cp.GenerateExecutable = false;
			cp.GenerateInMemory = false;
			cp.OutputAssembly = "tmpILData.dll";

			CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
			CompilerResults cr = provider.CompileAssemblyFromSource(cp, code);

			if (cr.Errors.Count > 0)
			{
				StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
				foreach (CompilerError error in cr.Errors)
				{
					errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
				}
				Log.Write(Log.Level.Error, "Compiler Error: ", errors.ToString());
				return null;
			}

			AssemblyDefinition assdefnew = AssemblyDefinition.ReadAssembly("tmpILData.dll");
			if (assdefnew.MainModule.Types.Count != 2)
			{
				Log.Write(Log.Level.Error, "More then one or no Class defined");
				return null;
			}
			foreach (TypeDefinition td in assdefnew.MainModule.Types)
			{
				if (td.Name != "<Module>")
				{
					if (td.Methods.Count != 2)
					{
						Log.Write(Log.Level.Error, "More then one or no Method defined");
						return null;
					}
					foreach (MethodDefinition md in td.Methods)
					{
						if (md.Name != ".ctor")
						{
							return md;
						}
					}
				}
			}
			Log.Write(Log.Level.Error, "Generated code couldn't be read");
			return null;
		}

		public struct MInject
		{
			public string code;
		}
	}
}
