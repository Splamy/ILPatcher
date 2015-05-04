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
		AssemblyDefinition parentAssembly = null;
		AssemblyDefinition compiledAssembly = null;
		bool isCompiled = false;

		private string code;
		public string Code
		{
			get { return code; }
			set
			{
				if (code != value)
				{
					isCompiled = false;
					code = value;
				}
			}
		}

		public CSCompiler(AssemblyDefinition pParentAssembly)
		{
			parentAssembly = pParentAssembly;
		}

		private bool CodeCompile()
		{
			if (isCompiled) return true;

			CompilerParameters cp = new CompilerParameters();

			if (parentAssembly != null)
			{
				DefaultAssemblyResolver dar = new DefaultAssemblyResolver();

				List<AssemblyDefinition> adList = new List<AssemblyDefinition>();
				adList.Add(parentAssembly);
				foreach (AssemblyNameReference anr in parentAssembly.MainModule.AssemblyReferences)
				{
					AssemblyDefinition adref = dar.Resolve(anr);
					//string fullname = adref.MainModule.FullyQualifiedName;
					if (!adList.Exists(adcomp => adcomp.MainModule.Name == adref.MainModule.Name &&
									   adcomp.Name.PublicKeyToken.SequenceEqual(adref.Name.PublicKeyToken)))
					{
						adList.Add(adref);
					}
					//if (!cp.ReferencedAssemblies.Contains(fullname))
					//.Add(fullname);
				}
				cp.ReferencedAssemblies.AddRange(adList.ConvertAll<string>(ad => ad.MainModule.FullyQualifiedName).ToArray());
			}

			cp.WarningLevel = 3;
			cp.CompilerOptions = "/target:library"; // /optimize
			cp.GenerateExecutable = false;
			cp.GenerateInMemory = false;
			cp.OutputAssembly = "tmpILData.dll";

			CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
			CompilerResults cr = provider.CompileAssemblyFromSource(cp, Code);

			if (cr.Errors.Count > 0)
			{
				StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
				foreach (CompilerError error in cr.Errors)
				{
					errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
				}
				Log.Write(Log.Level.Error, "Compiler Error: ", errors.ToString());
				if (cr.Errors.HasErrors)
					return false;
			}

			//FileStream fs
			//ModuleDefinition moddef = ModuleDefinition.ReadModule("tmpILData.dll");
			compiledAssembly = AssemblyDefinition.ReadAssembly("tmpILData.dll");
			isCompiled = true;
			return true;
		}

		public AssemblyDefinition GetAssemblyDefinition()
		{
			if (CodeCompile())
			{
				return compiledAssembly;
			}
			else
			{
				isCompiled = false;
				compiledAssembly = null;
				Log.Write(Log.Level.Error, "Generated code couldn't be read");
				return null;
			}
		}

		public TypeDefinition GetTypeDefinition(string typeName)
		{
			AssemblyDefinition ad = GetAssemblyDefinition();
			if (ad == null) return null;

			if (ad.MainModule.Types.Count != 2)
			{
				Log.Write(Log.Level.Error, "More then one or no Class defined");
				return null;
			}

			foreach (TypeDefinition td in ad.MainModule.Types)
			{
				if ((td.Name != "<Module>" && typeName == string.Empty) || typeName == td.Name)
				{
					return td;
				}
			}
			return null;
		}

		public MethodDefinition GetMethodDefinition(string typeName, string methodName)
		{
			TypeDefinition td = GetTypeDefinition(typeName);
			if (td == null) return null;

			if (td.Methods.Count != 2)
			{
				Log.Write(Log.Level.Error, "More then one or no Method defined");
				return null;
			}

			foreach (MethodDefinition md in td.Methods)
			{
				if ((md.Name != ".ctor" && methodName == string.Empty) || methodName == md.Name)
				{
					return md;
				}
			}
			return null;
		}

		public VariableDefinition GetVariableDefinition(string typeName, string methodName, string variableName)
		{
			MethodDefinition md = GetMethodDefinition(typeName, methodName);
			if (md == null) return null;

			if (md.Body.Variables.Count != 1)
			{
				Log.Write(Log.Level.Error, "More then one or no Variable defined");
				return null;
			}

			foreach (VariableDefinition vd in md.Body.Variables)
			{
				if (variableName == string.Empty || variableName == vd.Name)
				{
					return vd;
				}
			}
			return null;
		}
	}
}
